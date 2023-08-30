using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fooocus;
public class GradioClient
{
    JObject json;

    public async Task<string> SendAsync(params object[] parameters)
    {
        string hash = Guid.NewGuid().ToString("N")[..12];

        ClientWebSocket webSocket = new ClientWebSocket();
        await webSocket.ConnectAsync(new Uri("ws://127.0.0.1:7860/queue/join"), CancellationToken.None);

        //等待回信
        string result = string.Empty;

        while (result != "send_hash") result = await GetJsonValue(webSocket, "msg");

        await SendJson(webSocket, new { fn_index = 4, session_hash = hash });
        while (result != "send_data") result = await GetJsonValue(webSocket, "msg");

        var payload = new
        {
            data = parameters,
            fn_index = 4,
            session_hash = hash
        };

        await SendJson(webSocket, payload);

        while (result != "process_completed") result = await GetJsonValue(webSocket, "msg");


        string s = json["output"]["data"][3]["value"][0]["name"].ToString();

        return s;

    }

    private async Task SendJson(ClientWebSocket webSocket, object o)
    {
        string jsonPayload = JsonConvert.SerializeObject(o);
        byte[] sendBuffer = Encoding.UTF8.GetBytes(jsonPayload);
        await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> GetJsonValue(ClientWebSocket webSocket, string key)
    {
        byte[] data = await GetBytes(webSocket);

        string result = Encoding.UTF8.GetString(data);

        json = JObject.Parse(result);

        Debug.WriteLine(json[key].ToString());

        return json[key].ToString();
    }

    private async Task<byte[]> GetBytes(ClientWebSocket webSocket)
    {
        byte[] receiveBuffer = new byte[4096];  // 增加缓冲区大小

        using MemoryStream ms = new MemoryStream();
        WebSocketReceiveResult result;
        Debug.WriteLine("1:" + webSocket.State);
        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            ms.Write(receiveBuffer, 0, result.Count);  // 写入MemoryStream
        }
        while (!result.EndOfMessage);
        Debug.WriteLine("2:" + webSocket.State);

        byte[] data = ms.ToArray();

        return data;
    }
}
