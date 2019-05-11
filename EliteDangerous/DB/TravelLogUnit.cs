﻿/*
 * Copyright © 2016 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace EliteDangerousCore.DB
{
    public class TravelLogUnit
    {
        public const int NetLogType = 1;
        public const int EDSMType = 2;
        public const int JournalType = 3;
        public const int TypeMask = 0xff;
        public const int BetaMarker = 0x8000;

        public long id;
        public string Name;
        public int type;            // bit 15 = BETA.  Type = 2 EDSM log, 3 = Journal, 1 = Old pre 2.1 logs.
        public int Size;
        public string Path;
        public int? CommanderId;

        public TravelLogUnit()
        {
        }

        public TravelLogUnit(DataRow dr)
        {
            Object obj;
            id = (long)dr["id"];
            Name = (string)dr["Name"];
            type = (int)(long)dr["type"];
            Size = (int)(long)dr["size"];
            Path = (string)dr["Path"];
             obj = dr["CommanderId"];

            if (obj == DBNull.Value)
                CommanderId = null; 
            else
                CommanderId = (int)(long)dr["CommanderId"];

        }

        public TravelLogUnit(DbDataReader dr)
        {
            Object obj;
            id = (long)dr["id"];
            Name = (string)dr["Name"];
            type = (int)(long)dr["type"];
            Size = (int)(long)dr["size"];
            Path = (string)dr["Path"];
            obj =dr["CommanderId"];

            if (obj == DBNull.Value)
                CommanderId = null;  // TODO  use better default value?
            else
                CommanderId = (int)(long)dr["CommanderId"];

        }

        public bool Beta
        {
            get
            {
                if ((Path != null && Path.Contains("PUBLIC_TEST_SERVER")) || (type & BetaMarker) == BetaMarker)
                    return true;
                else
                    return false;
            }
        }


        public bool Add()
        {
            using (SQLiteConnectionUser cn = new SQLiteConnectionUser())
            {
                bool ret = Add(cn);
                return ret;
            }
        }

        private bool Add(SQLiteConnectionUser cn)
        {
            using (DbCommand cmd = cn.CreateCommand("Insert into TravelLogUnit (Name, type, size, Path, CommanderID) values (@name, @type, @size, @Path, @CommanderID)"))
            {
                cmd.AddParameterWithValue("@name", Name);
                cmd.AddParameterWithValue("@type", type);
                cmd.AddParameterWithValue("@size", Size);
                cmd.AddParameterWithValue("@Path", Path);
                cmd.AddParameterWithValue("@CommanderID", CommanderId);

                cmd.ExecuteNonQuery();

                using (DbCommand cmd2 = cn.CreateCommand("Select Max(id) as id from TravelLogUnit"))
                {
                    id = (long)cmd2.ExecuteScalar();
                }

                return true;
            }
        }

        public bool Update()
        {
            using (SQLiteConnectionUser cn = new SQLiteConnectionUser())
            {
                return Update(cn);
            }
        }

        public bool Update(SQLiteConnectionUser cn, DbTransaction tn = null)
        {
            using (DbCommand cmd = cn.CreateCommand("Update TravelLogUnit set Name=@Name, Type=@type, size=@size, Path=@Path, CommanderID=@CommanderID  where ID=@id", tn))
            {
                cmd.AddParameterWithValue("@ID", id);
                cmd.AddParameterWithValue("@Name", Name);
                cmd.AddParameterWithValue("@Type", type);
                cmd.AddParameterWithValue("@size", Size);
                cmd.AddParameterWithValue("@Path", Path);
                cmd.AddParameterWithValue("@CommanderID", CommanderId);

                cmd.ExecuteNonQuery();

                return true;
            }
        }
        
        static public List<TravelLogUnit> GetAll()
        {
            List<TravelLogUnit> list = new List<TravelLogUnit>();

            using (SQLiteConnectionUser cn = new SQLiteConnectionUser(mode: SQLLiteExtensions.SQLExtConnection.AccessMode.Reader))
            {
                using (DbCommand cmd = cn.CreateCommand("select * from TravelLogUnit"))
                {
                    using (DbDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            TravelLogUnit sys = new TravelLogUnit(rdr);
                            list.Add(sys);
                        }
                    }

                    return list;
                }
            }
        }

        public static List<string> GetAllNames()
        {
            List<string> names = new List<string>();
            using (SQLiteConnectionUser cn = new SQLiteConnectionUser(mode: SQLLiteExtensions.SQLExtConnection.AccessMode.Reader))
            {
                using (DbCommand cmd = cn.CreateCommand("SELECT DISTINCT Name FROM TravelLogUnit"))
                {
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add((string)reader["Name"]);
                        }
                    }
                }
            }
            return names;
        }

        public static TravelLogUnit Get(string name)
        {
            using (SQLiteConnectionUser cn = new SQLiteConnectionUser(mode: SQLLiteExtensions.SQLExtConnection.AccessMode.Reader))
            {
                using (DbCommand cmd = cn.CreateCommand("SELECT * FROM TravelLogUnit WHERE Name = @name ORDER BY Id DESC"))
                {
                    cmd.AddParameterWithValue("@name", name);
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TravelLogUnit(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static bool TryGet(string name, out TravelLogUnit tlu)
        {
            tlu = Get(name);
            return tlu != null;
        }

        public static TravelLogUnit Get(long id)
        {
            using (SQLiteConnectionUser cn = new SQLiteConnectionUser(mode: SQLLiteExtensions.SQLExtConnection.AccessMode.Reader))
            {
                using (DbCommand cmd = cn.CreateCommand("SELECT * FROM TravelLogUnit WHERE Id = @id ORDER BY Id DESC"))
                {
                    cmd.AddParameterWithValue("@id", id);
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TravelLogUnit(reader);
                        }
                    }
                }
            }

            return null;
        }

        public static bool TryGet(long id, out TravelLogUnit tlu)
        {
            tlu = Get(id);
            return tlu != null;
        }
    }
}

