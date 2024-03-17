using System.Windows;
using System.Windows.Input;
using WBTC.Library.WifiCrack.Shell.ViewModel;

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
            this.DataContext = new MainWindowViewModel();
        }
        /// <summary>
        /// 文本框只允许输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JustNum_PreKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
        }
    }
}
