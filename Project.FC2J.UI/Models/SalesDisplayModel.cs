using System;

namespace Project.FC2J.UI.Models
{
    public class SalesDisplayModel : BaseDisplayModel
    {
        public long Id { get; set; }
        public string SONo { get; set; }
        public string UserName { get; set; }
        public string InvoiceNo { get; set; }
        public string PONo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                CallPropertyChanged(nameof(DueDate));
                CallPropertyChanged(nameof(IsOverdue));
                CallPropertyChanged(nameof(IsNeardue));
            }
        }

        public long OrderTypeId { get; set; }
        public string OrderType { get; set; }

        public DateTime CollectedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ReceivedUser { get; set; }
        public string CollectedUser { get; set; }
        public long PriceListId { get; set; }
        

        public decimal UnpaidAmount => TotalPrice - PaidAmount;
        public bool IsOverdue => Convert.ToDateTime(DueDate.ToString("MMM-dd-yyyy")) < Convert.ToDateTime(DateTime.Now.ToString("MMM-dd-yyyy"));
        public int NearDueDays { get; set; }
        public bool IsNeardue => Convert.ToDateTime(DueDate.AddDays(NearDueDays * -1).ToString("MMM-dd-yyyy")) <= Convert.ToDateTime(DateTime.Now.ToString("MMM-dd-yyyy"))
                    && IsOverdue == false;

        private decimal _paidAmount;
        public decimal PaidAmount
        {
            get { return _paidAmount; }
            set
            {
                _paidAmount = value;
                CallPropertyChanged(nameof(PaidAmount));
                CallPropertyChanged(nameof(UnpaidAmount));
            }
        }
        private bool _isVatable;
        public bool IsVatable
        {
            get { return _isVatable; }
            set
            {
                _isVatable = value;
                CallPropertyChanged(nameof(IsVatable));
            }
        }

        private float _totalOrderQuantityUOMComputed;
        public float TotalOrderQuantityUOMComputed
        {
            get { return _totalOrderQuantityUOMComputed; }
            set
            {
                _totalOrderQuantityUOMComputed = value;
                CallPropertyChanged(nameof(TotalOrderQuantityUOMComputed));
            }

        }
        private float _totalOrderQuantity;
        public float TotalOrderQuantity
        {
            get { return _totalOrderQuantity; }
            set
            {
                _totalOrderQuantity = value;
                CallPropertyChanged(nameof(TotalOrderQuantity));
            }
        }

        private decimal _totalProductSalePrice;

        public decimal TotalProductSalePrice
        {
            get { return _totalProductSalePrice; }
            set
            {
                _totalProductSalePrice = value;
                CallPropertyChanged(nameof(TotalProductSalePrice));
            }
        }

        private decimal _totalProductTaxPrice;
        public decimal TotalProductTaxPrice
        {
            get { return _totalProductTaxPrice; }
            set
            {
                _totalProductTaxPrice = value;
                CallPropertyChanged(nameof(TotalProductTaxPrice));
            }
        }

        private decimal _totalDeductionPrice;
        public decimal TotalDeductionPrice
        {
            get { return _totalDeductionPrice; }
            set
            {
                _totalDeductionPrice = value;
                CallPropertyChanged(nameof(TotalDeductionPrice));
            }
        }

        private decimal _pickupDiscount;
        public decimal PickUpDiscount
        {
            get { return _pickupDiscount; }
            set
            {
                _pickupDiscount = value;
                CallPropertyChanged(nameof(PickUpDiscount));
            }
        }

        private decimal _outright;
        public decimal Outright
        {
            get { return _outright; }
            set
            {
                _outright = value;
                CallPropertyChanged(nameof(Outright));
            }
        }
        private decimal _cashDiscount;
        public decimal CashDiscount
        {
            get { return _cashDiscount; }
            set
            {
                _cashDiscount = value;
                CallPropertyChanged(nameof(CashDiscount));
            }
        }
        private decimal _promoDiscount;
        public decimal PromoDiscount
        {
            get { return _promoDiscount; }
            set
            {
                _promoDiscount = value;
                CallPropertyChanged(nameof(PromoDiscount));
            }
        }

        private decimal _lessPrice;

        public decimal LessPrice
        {
            get { return _lessPrice; }
            set
            {
                _lessPrice = value;
                CallPropertyChanged(nameof(LessPrice));
            }
        }


        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                CallPropertyChanged(nameof(TotalPrice));
                CallPropertyChanged(nameof(UnpaidAmount));
            }
        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                CallPropertyChanged(nameof(Total));
            }
        }

        public long CustomerId { get; set; }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                CallPropertyChanged(nameof(CustomerName));
            }
        }

        private string _customerAddress1;
        public string CustomerAddress1
        {
            get { return _customerAddress1; }
            set
            {
                _customerAddress1 = value;
                CallPropertyChanged(nameof(CustomerAddress1));
            }
        }
        private string _customerAddress2;

        public string CustomerAddress2
        {
            get { return _customerAddress2; }
            set
            {
                _customerAddress2 = value;
                CallPropertyChanged(nameof(CustomerAddress2));
            }
        }

        public string CustomerAddress => $"{CustomerAddress1} {CustomerAddress2}".Trim();

        public string MobileNo { get; set; }
        public string TelNo { get; set; }
        public string TIN { get; set; }
        public string SFAReferenceNo { get; set; }
        public long OrderStatusId { get; set; }
        private string _orderStatus;

        public string OrderStatus
        {
            get { return _orderStatus; }
            set
            {
                _orderStatus = value;
                CallPropertyChanged(nameof(OrderStatus));
            }
        }

        public long SelectedPaymentTypeId { get; set; }
        private string _selectedPaymentType;

        public string SelectedPaymentType
        {
            get { return _selectedPaymentType; }
            set
            {
                _selectedPaymentType = value;
                CallPropertyChanged(nameof(SelectedPaymentType));
            }
        }

        public string SelectedPriceList { get; set; }
        public string OverrideUser { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime ValidatedDate { get; set; }
        public DateTime CancelledDate { get; set; }
        public bool Deleted { get; set; }
    }
}
