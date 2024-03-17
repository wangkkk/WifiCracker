using Microsoft.Win32;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WBTC.Library.Utils.Helper;
using WBTC.Library.Utils.Model;
using WBTC.Library.Utils.MVVM;
using PasswordHelper = WBTC.Library.Utils.Helper.PasswordHelper;

namespace WBTC.Library.WifiCrack.Shell.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        #region Property
        private int _ThreadCount;
        /// <summary>
        /// 同时运行的线程数
        /// </summary>
        public int ThreadCount
        {
            get { return _ThreadCount; }
            set { _ThreadCount = value; base.RaisePropertyChanged("ThreadCount"); }
        }

        private bool _MainEnable;
        /// <summary>
        /// 整个界面是否可用
        /// </summary>
        public bool MainEnable
        {
            get { return _MainEnable; }
            set { _MainEnable = value; base.RaisePropertyChanged("MainEnable"); }
        }

        List<AccessPoint> AccessPointList { get; set; }

        private string _Tips;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tips
        {
            get { return _Tips; }
            set { _Tips = value; base.RaisePropertyChanged("Tips"); }
        }
        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; base.RaisePropertyChanged("FilePath"); }
        }
        private string _MainStr;
        public string MainStr
        {
            get { return _MainStr; }
            set { _MainStr = value; RaisePropertyChanged("MainStr"); }
        }
        private ICollectionView _wifiSortCollection;
        /// <summary>
        /// 排序后的数据列表
        /// </summary>
        public ICollectionView WifiSortCollection
        {
            get { return _wifiSortCollection; }
            set { _wifiSortCollection = value; RaisePropertyChanged("WifiSortCollection"); }
        }
        private ObservableCollection<WifiModel> _WifiList = new ObservableCollection<WifiModel>();
        public ObservableCollection<WifiModel> WifiList
        {
            get { return _WifiList; }
            set { _WifiList = value; base.RaisePropertyChanged("WifiList"); }
        }
        #endregion
        #region Command
        public ICommand _LoadFileCommand;
        public ICommand LoadFileCommand
        {
            get
            {
                if (this._LoadFileCommand == null)
                {
                    this._LoadFileCommand = new DelegateCommand(() =>
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();

                        // 设置文件过滤器（可选）
                        openFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

                        // 显示文件选择窗口并获取结果
                        bool? result = openFileDialog.ShowDialog();

                        // 处理用户的选择结果
                        if (result == true)
                        {
                            FilePath = openFileDialog.FileName;
                        }
                    });
                }
                return this._LoadFileCommand;

            }
        }
        private ICommand _ConnectCommand;
        public ICommand ConnectCommand
        {

            get
            {
                if (this._ConnectCommand == null)
                {
                    this._ConnectCommand = new DelegateCommand<WifiModel>((wifiModel) =>
                    {
                        var ap = AccessPointList.FirstOrDefault(m => m.Name == wifiModel.Name);
                        if (ap == null)
                        {
                            MessageBox.Show("没有找到对应的wifi");
                            return;
                        }
                        MainEnable = false;
                        Tips = $"正在连接{ap.Name}";
                        AuthRequest authRequest = new AuthRequest(ap);
                        Action<string> connectAction = new Action<string>((password) =>
                        {

                            authRequest.Password = password;
                            bool result = ap.Connect(authRequest);
                            string logResult = result ? "成功" : "失败";
                            string tips = $"连接Wifi{ap.Name},密码：{password},结果：{logResult}";
                            Tips = tips;
                            LogHelper.WriteLog(tips);
                            if (result)
                            {
                                Application.Current?.Dispatcher.Invoke(() =>
                                {
                                    wifiModel.IsConnected = true;
                                    SortData();
                                });
                            }
                            //else
                            //{
                            //    MessageBox.Show("连接失败");
                            //}
                        });
                        ReadFile(FilePath, connectAction);
                    });
                }
                return this._ConnectCommand;
            }
        }


        #endregion
        public MainWindowViewModel()
        {
            InitData();
        }


        #region Method
        public async Task ReadFile(string passwordFilePath, Action<string> connectAction)
        {
            try
            {
                if ((new FileInfo(passwordFilePath))?.Length < 1024 * 1024)
                {
                    ReadLittleFile(passwordFilePath, connectAction);
                }
                else
                {
                    PasswordHelper.GetPassword(passwordFilePath);
                    Task.Run(() =>
                    {
                        ReadBigFile(connectAction);
                    });
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog("",ex);
            }
            


        }

        private void ReadLittleFile(string passwordFilePath, Action<string> connectAction)
        {
            PasswordHelper.ReadLittleFileStr(passwordFilePath);
            Task.Run(() =>
            {
                Parallel.For(0, ThreadCount, _ =>
                {
                    while (PasswordHelper.PasswordQuqe?.Any() == true)
                    {
                        bool result = PasswordHelper.PasswordQuqe.TryDequeue(out string password);
                        if (result)
                        {
                            connectAction.Invoke(password);
                        }
                    }
                });

                MainEnable = true;
            });
        }

        private void InitData()
        {
            MainEnable = true;
            ThreadCount = 8;
            Wifi wifi = new Wifi();
            //获取所有wifi列表，遍历找到
            AccessPointList = wifi.GetAccessPoints();

            AutoMapperHelper.mapper.Map<List<AccessPoint>, ObservableCollection<WifiModel>>(AccessPointList, WifiList);
            WifiSortCollection = CollectionViewSource.GetDefaultView(WifiList);
            SortData();
        }
        private void ReadBigFile(Action<string> connectAction)
        {
            Parallel.For(0, ThreadCount, _ =>
            {
                while (PasswordHelper.ReadFileStatus.Item1 == false || PasswordHelper.PasswordQuqe?.Count != 0)
                {
                    if (PasswordHelper.PasswordQuqe?.Any() == true)
                    {
                        bool result = PasswordHelper.PasswordQuqe.TryDequeue(out string password);
                        if (result)
                        {
                            connectAction.Invoke(password);
                        }
                    }
                }
            });

            MainEnable = true;
        }
        /// <summary>
        /// 排序数据
        /// </summary>
        private void SortData()
        {

            //根据连接状态排序
            WifiSortCollection.SortDescriptions.Add(new SortDescription("SignalStrength", ListSortDirection.Descending));
            WifiSortCollection.SortDescriptions.Add(new SortDescription("IsConnected", ListSortDirection.Descending));
        }
        #endregion
    }

}
