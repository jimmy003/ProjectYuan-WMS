namespace Project.FC2J.Models.User
{
    public class Module
    {
        public int Id { get; set; }
        public bool Access { get; set; }
        public string ModuleName { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int Arrangement { get; set; }
        public string HeaderMenu { get; set; }
    }
}
