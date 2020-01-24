using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Models
{
    public class InvoiceDisplayModel : BaseDisplayModel
    {
        public long Id { get; set; }
        private string _pONo;

        public string PONo
        {
            get { return _pONo; }
            set { _pONo = value; CallPropertyChanged(nameof(PONo)); }
        }
        public long CustomerId { get; set; }
        public long IsReceived { get; set; }
        public long WithReturns { get; set; }
    }
}
