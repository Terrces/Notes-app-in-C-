using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practicka.Modules
{
    class DataBaseModule
    {
        static string databaseName = "IvanHarin";
        //public string connectionString = @"Data Source=KLABSQLW19S1, 49172;Initial Catalog=IvanHarin;Integrated Security=True;";
        public string connectionString = $@"Data Source=DESKTOP-S3ODS04; Initial Catalog={databaseName}; Integrated Security=SSPI;";

    }
}
