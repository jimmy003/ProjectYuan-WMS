using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Project.FC2J.UI.Models
{
    public class CustomerDisplayModel : BaseDisplayModel
    {
        public Int64 Id { get; set; }
        
        private string _referenceNo;
        public string ReferenceNo
        {
            get { return _referenceNo; }
            set
            {
                _referenceNo = value;
                CallPropertyChanged(nameof(ReferenceNo));
            }
        }
        private long _farmId;
        public long FarmId
        {
            get { return _farmId; }
            set
            {
                _farmId = value;
                CallPropertyChanged(nameof(FarmId));
            }
        }
        public string DisplayName
        {
            get
            {
                return $"{Id}-{Name}";
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                CallPropertyChanged(nameof(Name));
            }
        }
        private string _address1;
        public string Address1
        {
            get { return _address1; }
            set
            {
                _address1 = value;
                CallPropertyChanged(nameof(Address1));
            }
        }

        private string _address2;
        public string Address2
        {
            get { return _address2; }
            set
            {
                _address2 = value;
                CallPropertyChanged(nameof(Address2));
            }
        }
        private string _mobileNo;
        public string MobileNo
        {
            get { return _mobileNo; }
            set
            {
                _mobileNo = value;
                CallPropertyChanged(nameof(MobileNo));
            }
        }
        private string _telNo;
        public string TelNo
        {
            get { return _telNo; }
            set
            {
                _telNo = value;
                CallPropertyChanged(nameof(TelNo));
            }
        }
        private string _tin;
        public string TIN
        {
            get { return _tin; }
            set
            {
                _tin = value;
                CallPropertyChanged(nameof(TIN));
            }
        }

        private string _paymentType;
        public string PaymentType
        {
            get { return _paymentType; }
            set
            {
                _paymentType = value;
                CallPropertyChanged(nameof(PaymentType));
            }
        }

        private Int64 _paymentTypeId;
        public Int64 PaymentTypeId
        {
            get { return _paymentTypeId; }
            set
            {
                _paymentTypeId = value;
                CallPropertyChanged(nameof(PaymentTypeId));
            }
        }

        private string _priceList;
        public string PriceList
        {
            get { return _priceList; }
            set
            {
                _priceList = value;
                CallPropertyChanged(nameof(PriceList));
            }
        }

        private long _priceListId;
        public long PriceListId
        {
            get { return _priceListId; }
            set
            {
                _priceListId = value;
                CallPropertyChanged(nameof(PriceListId));
            }
        }

        public bool Deleted { get; set; }

        public List<Payment> PaymentDetails { get; set; } = new List<Payment>();
        public List<PriceList> PriceListDetails { get; set; } = new List<PriceList>();
        public List<Product> ProductDetails { get; set; } = new List<Product>();
        public List<ShippingAddress> ShipTo { get; set; } = new List<ShippingAddress>();

    }
}
