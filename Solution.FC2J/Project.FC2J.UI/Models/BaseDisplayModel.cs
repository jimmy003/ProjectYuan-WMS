using System;
using System.ComponentModel;
using Project.FC2J.UI.Helpers;

namespace Project.FC2J.UI.Models
{
    
    public class BaseDisplayModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
