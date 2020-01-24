namespace Project.FC2J.UI.Models
{
    public class PricelistDisplayModel : BaseDisplayModel
    {
        public long Id { get; set; }
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

        private bool _discountPolicy;
        public bool DiscountPolicy
        {
            get { return _discountPolicy; }
            set
            {
                _discountPolicy = value;
                CallPropertyChanged(nameof(DiscountPolicy));
            }
        }
        private int _subscribed;
        public int Subscribed
        {
            get { return _subscribed; }
            set
            {
                _subscribed = value;
                CallPropertyChanged(nameof(Subscribed));
            }
        }
        public bool Deleted { get; set; }
        public bool IsForSalesOrder { get; set; }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                CallPropertyChanged(nameof(Email));
            }
        }
    }
}
