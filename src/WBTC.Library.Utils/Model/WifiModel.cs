using WBTC.Library.Utils.MVVM;

namespace WBTC.Library.Utils.Model
{
    public class WifiModel : BindableBase
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; base.RaisePropertyChanged("Name"); }
        }
        private bool _IsConnected;
        public bool IsConnected
        {
            get { return _IsConnected; }
            set { _IsConnected = value; base.RaisePropertyChanged("IsConnected"); }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; base.RaisePropertyChanged("Password"); }
        }
        private uint _SignalStrength;
        /// <summary>
        /// 信号强度
        /// </summary>
        public uint SignalStrength
        {
            get { return _SignalStrength; }
            set { _SignalStrength = value; base.RaisePropertyChanged("SignalStrength"); }
        }
        
    }
}
