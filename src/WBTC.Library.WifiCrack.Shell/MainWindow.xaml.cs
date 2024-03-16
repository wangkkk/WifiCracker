using SimpleWifi;
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
using System.Windows.Shapes;

namespace WBTC.Library.WifiCrack.Shell
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Wifi wifi=new Wifi();
            wifi.ConnectionStatusChanged += Wifi_ConnectionStatusChanged;
            //获取所有wifi列表，遍历找到
            wifi.GetAccessPoints().ForEach(item =>
            {
                wifiTb.Text += $"WIFI:{item.Name}\r\n";
                if (item.Name.Contains("OPPO"))
                {
                    AuthRequest authRequest = new AuthRequest(item);
                    authRequest.Password = "111111ap";
                    //异步连接
                    //item.ConnectAsync(authRequest);
                    bool result = item.Connect(authRequest);
                    Console.WriteLine(result);
                }
            });
        }

        private void Wifi_ConnectionStatusChanged(object sender, WifiStatusEventArgs e)
        {
            Dispatcher.Invoke(() => 
            {
                wifiList.Text += "status:" + sender.ToString();
            });
            
        }
    }
}
