SQLite Client For WP
====================

SQLite Client for Windows Phone ( WP7 compatibile )

This client use Sqlite Client for Windows Phone (http://sqlitewindowsphone.codeplex.com/releases) rel 2 v.0.6.1

I wrote a Database Wrapper class ( in Classes folder ) that encapsulates some database operations such as copying the .sqlite file from the project to isolated storage.

Pay attention that a file must be created with the same name as the database file with the suffix -journal otherwise the library go in exception.

Changes to default library
==========================

I've also made a change to default library to handle DateTime null value.

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
