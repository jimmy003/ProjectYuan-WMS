using Project.FC2J.Models.Token;

namespace Project.FC2J.Models.User
{
    public class LoggedInUser : ILoggedInUser
    {
        public User User { get; set; } 
        public string Token { get; set; }
        public Setting Setting { get; set; }
        public void ResetUserModel()
        {
            Token = string.Empty;
            User = new User();
            Setting = new Setting();
        }
       
    }
}
