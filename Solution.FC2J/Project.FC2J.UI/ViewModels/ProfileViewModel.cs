using Caliburn.Micro;
using Microsoft.Win32;
using Project.FC2J.Models;
using Project.FC2J.UI.EventModels;
using Project.FC2J.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.ViewModels
{
    public class ProfileViewModel : Screen
    {
        private IProfileData _profileData;
        private User _user;
        private string _address2;
        private IUserEndpoint _userEndpoint;
        private IEventAggregator _events;

        public ProfileViewModel(IProfileData profileData, IUserEndpoint userEndpoint, IEventAggregator events, IApiAppSetting apiAppSetting)
        {
            _profileData = profileData;
            _userEndpoint = userEndpoint;
            _events = events;
            _apiAppSetting = apiAppSetting;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await OnSetProfile();
        }

        private const string defaultImage = "pack://application:,,,/Resources/320x240.png"; //  @"../Images/320x240.png";
        private object _previewUrl; //= GetPhoto(defaultImage);

        public object PreviewUrl { 
            get 
            { 
                return _previewUrl; 
            }
            set
            {
                _previewUrl = value;
                NotifyOfPropertyChange(() => PreviewUrl);
            }
        }

        private static byte[] GetPhoto(string filePath)
        {
            FileStream stream = new FileStream(
            filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] photo = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return photo;
        }
        public void OpenImageFile()
        {
            FileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Title = "Insert Image";
            OpenFileDialog1.InitialDirectory = "c:\\";
            // Below code will filter all file in open folder as per given file extension   
            OpenFileDialog1.Filter = "JPEG (*.jpg;*.jpeg;*.jpe)|*.jpg;*.jpeg;*.jpe|PNG (*.png)|*.png|TIFF (*.tiff)|*.tiff|GIF (*.gif)|*.gif|All Fi      les (*.*)|*.*";
            if (OpenFileDialog1.ShowDialog() == true)
            {
                PreviewUrl = GetPhoto(OpenFileDialog1.FileName);
            }
        }
        private async Task OnSetProfile()
        {
            _user = await _profileData.GetUserByUserNameAsync(_profileData.UserName);
            Role = _user.UserRole.RoleName;
            UserName = _user.UserName;
            Password = _profileData.PasswordX;
            Confirm = _profileData.PasswordX;
            FirstName = _user.FirstName;
            MiddleName = _user.MiddleName;
            LastName = _user.LastName;
            Email = _user.Email;
            ContactNo = _user.ContactNo;
            Address1 = _user.Address1;
            Address2 = _user.Address2;
            if (string.IsNullOrWhiteSpace(_user.Primary) == false)
            {
                PreviewUrl = Convert.FromBase64String (_user.Primary);
            }
            else 
            {
                PreviewUrl = ImageToByte(Properties.Resources._320x240);
            }
            
       }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public bool IsFirstLogon
        {
            get
            {
                return _profileData.PasswordX == _apiAppSetting.DefaultPassword;
            }
        }
        public string FullName
        {
            get { return $"{FirstName} {MiddleName} {LastName}".Trim(); }
        }

        private string _role;
        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;
                NotifyOfPropertyChange(() => Role);
            }
        }
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);                
            }
        }

        public bool UserNameEnabled
        {
            get { return false; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanSave);                
            }
        }
        private string _confirm;
        public string Confirm
        {
            get { return _confirm; }
            set
            {
                _confirm = value;
                NotifyOfPropertyChange(() => Confirm);
                NotifyOfPropertyChange(() => CanSave);
            }
        }
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => FullName);
            }
        }
        private string _middleName;
        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                NotifyOfPropertyChange(() => MiddleName);
                NotifyOfPropertyChange(() => FullName);
            }
        }
        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName);
            }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }
        private string _contactNo;
        public string ContactNo
        {
            get { return _contactNo; }
            set
            {
                _contactNo = value;
                NotifyOfPropertyChange(() => ContactNo);
            }
        }
        private string _address1;
        private IApiAppSetting _apiAppSetting;

        public string Address1
        {
            get { return _address1; }
            set
            {
                _address1 = value;
                NotifyOfPropertyChange(() => Address1);
            }
        }

        public string Address2
        {
            get { return _address2; }
            set
            {
                _address2 = value;
                NotifyOfPropertyChange(() => Address2);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CanSave
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(Password) == false
                    && string.IsNullOrWhiteSpace(Confirm) == false
                    && Password == Confirm )
                {
                    if (IsFirstLogon )
                    {
                        if (_profileData.PasswordX != Password)                        
                        {
                            output = true;
                        }
                    }
                    else
                    {
                        output = true;
                    }
                }
                return output;
            }
        }


        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var user = new User();
                user.UserName = UserName;
                user.Address1 = Address1;
                user.Address2 = Address2;
                user.ContactNo = ContactNo;
                user.Email = Email;
                user.LastName = LastName;
                user.FirstName = FirstName;
                user.MiddleName = MiddleName;
                user.Locked = false;
                user.UserRoles = _user.UserRole.RoleName;
                user.UserRole = _user.UserRole;
                user.Primary = Convert.ToBase64String((byte[])PreviewUrl);

                user.Id = _user.Id;
                user.PasswordHash = _user.PasswordHash;
                user.PasswordSalt = _user.PasswordSalt;

                if (_profileData.PasswordX != Password)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(Password, out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                //if (!VerifyPasswordHash(Password, user.PasswordHash, user.PasswordSalt))
                //{

                //}

                await _userEndpoint.Update(user);

                if (IsFirstLogon)
                {
                    _events.PublishOnUIThread(new UpdatePasswordEvent());
                }

                _profileData.SetPasswordX(Password);
                NotifyOfPropertyChange(() => IsFirstLogon);

                MessageBox.Show("Successfully Saved!", "Confirmation", MessageBoxButton.OK);

            }
        }

        public void Close()
        {
            TryClose();
        }


    }
}
