SQLite Client For WP
====================

This client use Sqlite Client for Windows Phone (http://sqlitewindowsphone.codeplex.com/releases) rel 2 v.0.6.1
with little changes.

I wrote a Database Wrapper class ( in Classes folder ) that encapsulates some database operations such as copying the .sqlite file from the project to isolated storage.

Pay attention that a file must be created with the same name as the database file with the suffix -journal otherwise the library go in exception.

Changes to default library
==========================

**Add support to DateTime column null value.**

    public DateTime ToDateTime(string dateText)
    {
        switch (dateFormat)
        {
            case SQLiteDateFormats.Ticks:
                return new DateTime(Convert.ToInt64(dateText, CultureInfo.InvariantCulture));
            case SQLiteDateFormats.JulianDay:
                return ToDateTime(Convert.ToDouble(dateText, CultureInfo.InvariantCulture));
            default:
                {
                    if (dateText != null)
                    {
                        return DateTime.ParseExact(dateText, _datetimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
        }
    }


**Add sqlite custom function as gps distance between two positions**

In database wrapper, when open database add custom function:

        private void Open()
        {
            db.Open();
            Sqlite3.sqlite3_create_function(db.SqliteDb, "distanceGPSInKM", 4, Sqlite3.SQLITE_UTF8, null, SQLite_distanceGPSFunc, null, null);
        }
        
Define in database wrapper static method SQLite_distanceGPSFunc

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
