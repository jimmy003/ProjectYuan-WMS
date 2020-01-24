using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.ViewModels
{
    public class UserViewModel : Screen
    {
        private BindingList<UserDisplayModel> _users;
        private readonly IMapper _mapper;
        private readonly IUserEndpoint _userEndpoint;
        private readonly IApiAppSetting _appSetting;

        public UserViewModel(IUserEndpoint userEndpoint, IMapper mapper, IApiAppSetting appSetting, IProfileData profileData)
        {
            _userEndpoint = userEndpoint;
            _mapper = mapper;
            _appSetting = appSetting;
            _loggedUser = profileData;
        }

        public void FilterCustomers(string message)
        {
            var users = new List<UserDisplayModel>();


            if (string.IsNullOrWhiteSpace(message))
            {
                if (Users.Count != _allUsers.Count)
                    users = _mapper.Map<List<UserDisplayModel>>(_allUsers);
                else
                {
                    return;
                }
            }
            else
            {
                users = _mapper.Map<List<UserDisplayModel>>(_allUsers.Where(c => c.UserName.ToLower().Contains(message.ToLower())));
                if(users.Count == 0)
                    users = _mapper.Map<List<UserDisplayModel>>(_allUsers.Where(c => c.FirstName.ToLower().Contains(message.ToLower())));
            }

            Users = new BindingList<UserDisplayModel>(users);
        }
        public BindingList<UserDisplayModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            IsGridVisible = false;
            await LoadUsers();

        }
        private List<User> _allUsers = new List<User>();

        private async Task LoadUsers()
        {
            try
            {
                _allUsers = await _userEndpoint.GetList();
                var users = _mapper.Map<List<UserDisplayModel>>(_allUsers);
                Users = new BindingList<UserDisplayModel>(users);
                Roles = new BindingList<Role>(await _userEndpoint.GetRoles());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            await Task.Run(() => IsGridVisible = _appSetting.SleepInSeconds.AndGridWillBeBack());
        }

        private BindingList<Role> _roles;

        public BindingList<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; NotifyOfPropertyChange(() => Roles); }
        }

        private Role _selectedRole;

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                NotifyOfPropertyChange(() => SelectedRole);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        private bool _isGridVisible = false;
        public bool IsGridVisible
        {
            get { return _isGridVisible; }
            set
            {
                _isGridVisible = value;
                NotifyOfPropertyChange(() => IsGridVisible);
                NotifyOfPropertyChange(() => IsLoadingVisible);
            }
        }

        private byte[] _passwordHash;

        public byte[] PasswordHash
        {
            get { return _passwordHash; }
            set 
            { 
                _passwordHash = value;
                NotifyOfPropertyChange(() => PasswordHash);
            }
        }

        private byte[] _passwordSalt;

        public byte[] PasswordSalt
        {
            get { return _passwordSalt; }
            set
            {
                _passwordSalt = value;
                NotifyOfPropertyChange(() => PasswordSalt);
            }
        }

        public bool IsLoadingVisible
        {
            get { return !_isGridVisible; }
        }

        private UserDisplayModel _selectedUser;
        public UserDisplayModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;

                if (_selectedUser != null)
                {
                    Id = _selectedUser.Id.ToString();
                    UserName = _selectedUser.UserName;
                    Brgy = _selectedUser.Address1;
                    City = _selectedUser.Address2;
                    ContactNo = _selectedUser.ContactNo;
                    Email = _selectedUser.Email;
                    LastName = _selectedUser.LastName;
                    FirstName = _selectedUser.FirstName;
                    MiddleName = _selectedUser.MiddleName;
                    PasswordHash = _selectedUser.PasswordHash;
                    PasswordSalt = _selectedUser.PasswordSalt;
                    Role existingRole = Roles.FirstOrDefault(x => x.RoleName == _selectedUser.UserRoles);

                    if (existingRole != null)
                    {
                        SelectedRole = existingRole;
                    }
                    else
                    {
                        SelectedRole = null;
                    }
                }
                
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set { 
                _id = value; 
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanClear);
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
                NotifyOfPropertyChange(() => UserNameEnabled);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public string FullName
        {
            get { return $"{FirstName} {MiddleName} {LastName}"; }            
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
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        private string _middleName;

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; NotifyOfPropertyChange(() => MiddleName); NotifyOfPropertyChange(() => FullName); NotifyOfPropertyChange(() => CanClear);
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
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value; NotifyOfPropertyChange(() => Email); NotifyOfPropertyChange(() => CanClear);
            }
        }
        private string _contactNo;

        public string ContactNo
        {
            get { return _contactNo; }
            set { _contactNo = value; NotifyOfPropertyChange(() => ContactNo); NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _brgy;

        public string Brgy
        {
            get { return _brgy; }
            set { _brgy = value; NotifyOfPropertyChange(() => Brgy); NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _city;

        public string City
        {
            get { return _city; }
            set { _city = value; NotifyOfPropertyChange(() => City); NotifyOfPropertyChange(() => CanClear);
            }
        }

        private bool _locked;
        private IProfileData _loggedUser;

        public bool Locked
        {
            get { return _locked; }
            set 
            { 
                _locked = value; 
                NotifyOfPropertyChange(() => Locked); 
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public bool UserNameEnabled
        {
            get
            {
                if (SelectedUser == null)
                {
                    return true;
                }
                return _loggedUser.UserName.ToLower() != UserName.ToLower();
            }
        }
        private void OnClear()
        {
            Id = string.Empty;
            UserName = string.Empty;
            Brgy = string.Empty;
            City = string.Empty;
            ContactNo = string.Empty;
            Email = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            SelectedRole = null;
            SelectedUser = null;
            Locked = false;            
            NotifyOfPropertyChange(() => CanDelete);
        }
        public bool CanClear
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(Id) == false
                    || string.IsNullOrWhiteSpace(UserName) == false
                    || string.IsNullOrWhiteSpace(LastName) == false
                    || string.IsNullOrWhiteSpace(FirstName) == false
                    || string.IsNullOrWhiteSpace(MiddleName) == false
                    || string.IsNullOrWhiteSpace(Brgy) == false
                    || string.IsNullOrWhiteSpace(City) == false
                    || string.IsNullOrWhiteSpace(Email) == false
                    || string.IsNullOrWhiteSpace(ContactNo) == false
                    || Locked == true
                    )
                {
                    output = true;
                }
                return output;
            }
        }
        public void Clear()
        {
            if (MessageBox.Show("Are you sure?", "Clear Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OnClear();
            }
        }

        public bool CanDelete
        {
            get
            {
                bool output = false;

                var isUserName = true;
                if (UserName != null)
                {
                    isUserName = _loggedUser.UserName.ToLower() != UserName.ToLower();
                }                     

                if (string.IsNullOrWhiteSpace(Id) == false && isUserName)
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task Delete()
        {
            if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _userEndpoint.Remove(Convert.ToInt64(Id));
                Users.Remove(SelectedUser);
                OnClear();
            }
        }

        public bool CanSave
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(UserName) == false
                    && string.IsNullOrWhiteSpace(LastName) == false
                    && string.IsNullOrWhiteSpace(FirstName) == false
                    && SelectedRole != null
                )
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var user = new User
                {
                    UserName = UserName,
                    Address1 = Brgy,
                    Address2 = City,
                    ContactNo = ContactNo,
                    Email = Email,
                    LastName = LastName,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    Locked = Locked,
                    UserRoles = SelectedRole.RoleName,
                    UserRole = SelectedRole
                };

                if (SelectedUser == null)
                {
                    var result = await _userEndpoint.Save(user);
                    var newUser = _mapper.Map<UserDisplayModel>(result);
                    _allUsers.Add(result);
                    Users.Add(newUser);
                }
                else
                {
                    user.Id = Convert.ToInt64(SelectedUser.Id);
                    user.PasswordHash = SelectedUser.PasswordHash;
                    user.PasswordSalt = SelectedUser.PasswordSalt;

                    await _userEndpoint.Update(user);
                    SelectedUser.UserName = UserName;
                    SelectedUser.Address1 = Brgy;
                    SelectedUser.Address2 = City;
                    SelectedUser.ContactNo = ContactNo;
                    SelectedUser.Email = Email;
                    SelectedUser.FirstName = FirstName;
                    SelectedUser.MiddleName = MiddleName;
                    SelectedUser.LastName = LastName;
                    SelectedUser.Locked = Locked;
                    SelectedUser.UserRoles = SelectedRole.RoleName;
                    SelectedUser.Role = SelectedRole;
                }
                OnClear();
            }
        }
        public void Close()
        {
            TryClose();
        }


    }
}
