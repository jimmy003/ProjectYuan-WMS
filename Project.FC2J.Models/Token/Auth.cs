using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Token
{
    public class Auth
    {
        public string Token { get; set; }
        public User.User User { get; set; }
        public Setting Setting { get; set; }
    }
}
