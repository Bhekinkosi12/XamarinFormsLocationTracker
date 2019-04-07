using SQLite;

namespace LocationTracker.SQLite
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}