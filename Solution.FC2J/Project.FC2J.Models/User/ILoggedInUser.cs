using Project.FC2J.Models.Token;

namespace Project.FC2J.Models.User
{
    public interface ILoggedInUser
    {
        User User { get; set; }
        string Token { get; set; }
        Setting Setting { get; set; }
        void ResetUserModel();
    }
}