namespace Project.FC2J.Batch
{

    //Singleton class 
    public class DBSettings
    {
        static DBSettings dBSettings = null;

        private DBSettings()
        {
        }

        public static DBSettings GetDBSettingsInstance()
        {
            if(dBSettings==null)
            {
                dBSettings = new DBSettings();                
            }
            return dBSettings;
        }

        public string Connection { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }

    }
}
