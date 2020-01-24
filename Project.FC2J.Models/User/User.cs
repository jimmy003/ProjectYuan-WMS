using System;
using System.Collections.Generic;

namespace Project.FC2J.Models.User
{
    public class User
    {
        public Int64 Id { get; set; }
        public string UserName { get; set; }
        public string PasswordX { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Primary { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public bool Deleted { get; set; }
        public bool Locked { get; set; }
        public int Tries { get; set; }

        public string UserRoles { get; set; }
        public int UserRoleId { get; set; }
        public Role UserRole { get; set; } = new Role();
        public List<Module> UserModules { get; set; } = new List<Module>();
    }
}
