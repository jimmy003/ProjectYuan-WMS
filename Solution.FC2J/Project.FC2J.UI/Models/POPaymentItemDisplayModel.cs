using System;

namespace Project.FC2J.UI.Models
{
    public class POPaymentItemDisplayModel : BaseDisplayModel
    {
        public long Id { get; set; }
        public long OrderHeaderId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string UserName { get; set; }


        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                CallPropertyChanged(nameof(Amount));
            }
        }

        private string _invoiceNo;
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                _invoiceNo = value;
                CallPropertyChanged(nameof(InvoiceNo));
            }
        }

    }
}
