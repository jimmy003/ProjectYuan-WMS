using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Models
{
    public class DeductionDisplayModel : BaseDisplayModel
    {
        public long CustomerId { get; set; }
        public long Id { get; set; }
        //public string Name { get; set; }
        private string _particular;
        public string Particular
        {
            get { return _particular; }
            set
            {
                _particular = value;
                CallPropertyChanged(nameof(Particular));
                CallPropertyChanged(nameof(DisplayDescription));
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                CallPropertyChanged(nameof(Amount));
                CallPropertyChanged(nameof(DisplayDescription));
            }
        }

        private string _pONo;
        public string PONo
        {
            get { return _pONo; }
            set
            {
                _pONo = value;
                CallPropertyChanged(nameof(PONo));
            }
        }

        private decimal _usedAmount;
        public decimal UsedAmount
        {
            get { return _usedAmount; }
            set
            {
                _usedAmount = value;
                CallPropertyChanged(nameof(UsedAmount));
            }
        }

        private DateTime _updatedDate;
        public DateTime UpdatedDate
        {
            get { return _updatedDate; }
            set
            {
                _updatedDate = value;
                CallPropertyChanged(nameof(UpdatedDate));
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                CallPropertyChanged(nameof(IsChecked));
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                CallPropertyChanged(nameof(IsEnabled));
            }
        }

        public string DisplayDescription => $"{Particular} (P {Amount.ToString("C").Substring(1)})";


    }
}
