using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace LocationTracker.Model
{
    public class GroupClass
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Group { get; set; }
    }
}
