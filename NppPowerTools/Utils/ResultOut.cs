using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NppPowerTools.Utils
{
    public class ResultOut : INotifyPropertyChanged
    {
        public string Name { get; set; }

        [JsonIgnore]
        public Action<string> SetResult { get; set; }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
