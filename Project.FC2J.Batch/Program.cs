using System.Configuration;
using System.Threading.Tasks;
using Project.FC2J.Batch.Internal.DataAccess;

namespace Project.FC2J.Batch
{
    class Program
    {
        private static DBSettings _dbSettings = DBSettings.GetDBSettingsInstance();
        private static readonly string _sp = "CreateBeginningBalance";

        static void Main(string[] args)
        {
            _dbSettings.Connection =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            _sp.ExecuteNonQuery();
        }
    }
}
