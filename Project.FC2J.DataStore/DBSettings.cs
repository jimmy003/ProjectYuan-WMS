using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.FC2J.DataStore
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
