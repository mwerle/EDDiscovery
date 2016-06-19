using EDDiscovery.DB;
using System;
using System.Collections.Generic;

namespace EDDiscovery
{
    public enum SystemInfoSource
    {
        RW = 1,
        EDSC = 2,
        EDDB = 4,
        EDSM = 5

    }


    public class SystemData
    {

        static public List<SystemClass> SystemList
        {
            get
            {
                return SQLiteDBClass.globalSystems;
            }
        }

        static public SystemClass GetSystem(string name)
        {
            SystemClass rv = null;
            try
            {
                string lname = name.Trim().ToLower();
                rv = SQLiteDBClass.dictSystems[lname];
            } catch(Exception ex)
            {
                // Do nothing - we expect either a null-pointer exception if name == null, or a KeyNotFoundException
            }
            return rv;
        }

        static public void AddSystem(SystemClass system)
        {
            if (!SQLiteDBClass.dictSystems.ContainsKey(system.SearchName))
            {
                SQLiteDBClass.dictSystems[system.SearchName] = system;
                SQLiteDBClass.globalSystems.Add(system);
            }            
        }

        public static double Distance(SystemClass s1, SystemClass s2)
        {
            if (s1 == null || s2== null)
                return -1;

            return Math.Sqrt((s1.x - s2.x) * (s1.x - s2.x) + (s1.y - s2.y) * (s1.y - s2.y) + (s1.z - s2.z) * (s1.z - s2.z));
        }
    }
}
