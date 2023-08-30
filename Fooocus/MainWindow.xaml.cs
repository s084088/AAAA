using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fooocus;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

    }



    private async Task TTT()
    {
        GradioClient client = new GradioClient();
        int seed = new Random().Next(0, 1024 * 1024 * 1024);
        try
        {
            string path = await client.SendAsync(
                "2 boy chatting on the road",      //提示词
                "",                                     //反向提示词
                "sai-comic book",                                 //样式
                "Quality",                              //速度还是质量  Speed  Quality
                "1344×768",                             //分辨率
                1,                                      //图片数量
                seed,                                      //种子
                2,                                      //清晰度 0-40
                "sd_xl_base_1.0_0.9vae.safetensors",    //文生图模型
                "sd_xl_refiner_1.0_0.9vae.safetensors", //图生图模型
                "sd_xl_offset_example-lora_1.0.safetensors",
                0.5,
                "None",
                0.5,
                "None",
                0.5,
                "None",
                0.5,
                "None",
                0.5
            );


            I1.Source = new BitmapImage(new Uri(path));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        await TTT();
    }
}
