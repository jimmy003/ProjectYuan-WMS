using Project.FC2J.Models;
using System;
using Project.FC2J.Models.User;

namespace Project.FC2J.UI.Models
{
    public class UserDisplayModel : BaseDisplayModel
    {
        public Int64 Id { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                CallPropertyChanged(nameof(UserName));
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; CallPropertyChanged(nameof(Email)); }
        }

        private string _contactNo;

        public string ContactNo
        {
            get { return _contactNo; }
            set { _contactNo = value; CallPropertyChanged(nameof(ContactNo)); }
        }


        public string FullName
        {
            get
            {
                return $"{FirstName} {MiddleName} {LastName}";
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; CallPropertyChanged(nameof(LastName)); }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; CallPropertyChanged(nameof(FirstName)); }
        }

        private string _middleName;

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; CallPropertyChanged(nameof(MiddleName)); }
        }

        private string _address1;

        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; CallPropertyChanged(nameof(Address1)); }
        }

        private string _address2;

        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; CallPropertyChanged(nameof(Address2)); }
        }

        private string _userRoles;

        public string UserRoles
        {
            get { return _userRoles; }
            set { _userRoles = value; CallPropertyChanged(nameof(UserRoles)); }
        }

        private Role _role;

        public Role Role
        {
            get { return _role; }
            set { _role = value; CallPropertyChanged(nameof(Role)); }
        }
        private bool _locked;
        public bool Locked
        {
            get { return !_locked; }
            set { _locked = value; CallPropertyChanged(nameof(Locked)); }
        }
    }
}
