using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using LocationTracker.SQLite;
using LocationTracker.UWP.SQLite;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_UWP))]
namespace LocationTracker.UWP.SQLite
{
    public class SQLite_UWP:ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "LocationTracker.db3";
            string documentsPath = ApplicationData.Current.LocalFolder.Path;
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(path);
            // Return the database connection
            return conn;
        }
    }
}
