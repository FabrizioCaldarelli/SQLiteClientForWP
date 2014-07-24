using Community.CsharpSqlite;
using SQLiteClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Resources;
using System.IO.IsolatedStorage;
using System.Windows;
using System.IO;

namespace SQLiteClientForWP.Classes
{
    public class DatabaseWrapper
    {
        SQLiteClient.SQLiteConnection db;

        #region Database
        public string CopiaDatabaseNellIsolatedStorage()
        {
            string pathIsolatedStorage = "database1.sqlite";
            string pathIsolatedStorageJournal = pathIsolatedStorage + "-journal";
            string pathInput = "database1.sqlite";

            StreamResourceInfo streamResourceInfo = Application.GetResourceStream(new Uri(pathInput, UriKind.Relative));

            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (false == isolatedStorage.FileExists(pathIsolatedStorage))
                {
                    using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(pathIsolatedStorage, FileMode.Create, isolatedStorage))
                    {
                        using (BinaryWriter writer = new BinaryWriter(fileStream))
                        {
                            Stream resourceStream = streamResourceInfo.Stream;
                            long length = resourceStream.Length;
                            byte[] buffer = new byte[4096];
                            int readCount = 0;
                            using (BinaryReader reader = new BinaryReader(streamResourceInfo.Stream))
                            {
                                // read file in chunks in order to reduce memory consumption and increase performance
                                while (readCount < length)
                                {
                                    int actual = reader.Read(buffer, 0, buffer.Length);
                                    readCount += actual;
                                    writer.Write(buffer, 0, actual);
                                }
                            }
                        }
                    }
                }

                // It is important that you create -journal file otherwise sqlite library will crash
                if (false == isolatedStorage.FileExists(pathIsolatedStorageJournal))
                {
                    IsolatedStorageFileStream dest = new IsolatedStorageFileStream(pathIsolatedStorageJournal, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, isolatedStorage);
                    dest.Flush();
                    dest.Close();
                    dest.Dispose();
                }
            }


            return pathIsolatedStorage;
        }
        public string DatabasePathFile()
        {
            string filePath = this.CopiaDatabaseNellIsolatedStorage();
            return filePath;
        }
        #endregion

        #region SQLite Extensions
        static double DEG2RAD(double degrees)
        {
            return (degrees * 0.01745327);
        }
        public static void SQLite_distanceGPSFunc(
            Sqlite3.sqlite3_context ctx, int argc, Sqlite3.Mem[] argv
        )
        {
            // check that all four arguments are non-null
            if (Sqlite3.sqlite3_value_type(argv[0]) == Sqlite3.SQLITE_NULL || Sqlite3.sqlite3_value_type(argv[1]) == Sqlite3.SQLITE_NULL || Sqlite3.sqlite3_value_type(argv[2]) == Sqlite3.SQLITE_NULL || Sqlite3.sqlite3_value_type(argv[3]) == Sqlite3.SQLITE_NULL)
            {
                Sqlite3.sqlite3_result_null(ctx);
                return;
            }
            // get the four argument values
            double lat1 = Sqlite3.sqlite3_value_double(argv[0]);
            double lon1 = Sqlite3.sqlite3_value_double(argv[1]);
            double lat2 = Sqlite3.sqlite3_value_double(argv[2]);
            double lon2 = Sqlite3.sqlite3_value_double(argv[3]);
            // convert lat1 and lat2 into radians now, to avoid doing it twice below
            double lat1rad = DEG2RAD(lat1);
            double lat2rad = DEG2RAD(lat2);
            // apply the spherical law of cosines to our latitudes and longitudes, and set the result appropriately
            // 6378.1 is the approximate radius of the earth in kilometres
            Sqlite3.sqlite3_result_double(ctx, Math.Acos(Math.Sin(lat1rad) * Math.Sin(lat2rad) + Math.Cos(lat1rad) * Math.Cos(lat2rad) * Math.Cos(DEG2RAD(lon2) - DEG2RAD(lon1))) * 6378.1);

        }
        #endregion


        public Object ExecuteScalar(string sql)
        {
            SQLiteCommand cmd = db.CreateCommand(sql);
            object obj = cmd.ExecuteScalar();
            return obj;
        }

        public List<T> ExecuteQueryObject<T>(string sql) where T : new()
        {
            SQLiteCommand cmd = db.CreateCommand(sql);
            IEnumerable<T> enList = cmd.ExecuteQuery<T>();
            List<T> lst = enList.ToList<T>();
            return lst;
        }

        public void Dispose()
        {
            if(db!=null)db.Dispose();
            db = null;
        }

        ~DatabaseWrapper()
        {
            this.Dispose();
        }

        private void Open()
        {
            db.Open();
            Sqlite3.sqlite3_create_function(db.SqliteDb, "distanceGPSInKM", 4, Sqlite3.SQLITE_UTF8, null, SQLite_distanceGPSFunc, null, null);
        }

        public DatabaseWrapper()
        {
            db = new SQLiteClient.SQLiteConnection(DatabasePathFile());
            Open();
        }
    }
}
