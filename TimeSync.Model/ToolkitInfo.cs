using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.DataAccess;

namespace TimeSync.Model
{
    public class ToolkitInfo : INotifyPropertyChanged
    {
        private string _url;
        private string _customerName;
        private int _userId;

        #region Properties Getters and Setters
        public string Url
        {
            get { return this._url; }
            set
            {
                this._url = value;
                OnPropertyChanged("Url");
            }
        }

        public string CustomerName
        {
            get { return this._customerName; }
            set
            {
                this._customerName = value;
                OnPropertyChanged("CustomerName");
            }
        }
        public int UserId
        {
            get { return this._userId; }
            set
            {
                this._userId = value;
                OnPropertyChanged("UserId");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public ToolkitInfo() {}

        public ToolkitInfo(string url, string customerName)
        {
            this.Url = url;
            this.CustomerName = customerName;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }



    }
}
