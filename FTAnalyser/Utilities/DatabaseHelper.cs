using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTAnalyzer.Utilities
{
    public class DatabaseHelper : IDisposable
    {
        public string DatabaseFile { get; private set; }
        public string CurrentFilename { get; private set; }
        public string DatabasePath { get; private set; }
        static DatabaseHelper instance;
        static string connectionString;
        static SQLiteConnection InstanceConnection { get; set; }
        Version ProgramVersion { get; set; }
        bool restoring;

        #region Constructor/Destructor
        DatabaseHelper()
        {
            DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer");
            CurrentFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer\FTA-RestoreTemp.s3db");
            if (CheckDatabaseConnection())
            {
                InstanceConnection = new SQLiteConnection(connectionString);
                restoring = false;
            }
        }

        public static DatabaseHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseHelper();
                }
                return instance;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (InstanceConnection?.State == ConnectionState.Open)
                        InstanceConnection.Close();
                    InstanceConnection?.Dispose();
                    // dispose of things here
                }
                catch (Exception) { }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool CheckDatabaseConnection()
        {
            try
            {
                DatabaseFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer\Geocodes.s3db");
                if (!File.Exists(DatabaseFile))
                {
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    File.Copy(Path.Combine(Application.StartupPath, @"Resources\Geocodes-Empty.s3db"), DatabaseFile);
                }
                connectionString = $"Data Source={DatabaseFile};Version=3;";
                return true;
            }
            catch (Exception ex)
            {
                UIHelpers.ShowMessage($"Error opening database - Filename: {DatabaseFile}. Error is :{ex.Message}", "FTAnalyzer");
                return false;
            }
        }
        #endregion

        #region Database Update Functions
        static Version GetDatabaseVersion()
        {
            string db = null;
            try
            {
                if (InstanceConnection.State != ConnectionState.Open)
                    InstanceConnection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("select Database from versions where platform='PC'", InstanceConnection))
                {
                    db = (string)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {  // use old method if current method fails
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("select Database from versions", InstanceConnection))
                    {
                        db = (string)cmd.ExecuteScalar();
                    }
                }
                catch { }
            }
            finally
            {
                InstanceConnection?.Close();
            }
            Version dbVersion = db == null ? new Version("0.0.0.0") : new Version(db);
            if (dbVersion == new Version("7.3.0.0"))
                return new Version("7.0.0.0"); // force old version so it updates after beta fix on v7.3.0.0
            return dbVersion;
        }

        #endregion

        #region Custom Facts

        public static bool IgnoreCustomFact(string factType)
        {
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            bool result = false;
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT EXISTS(SELECT ignore FROM CustomFacts where FactType=?)", InstanceConnection))
            {
                SQLiteParameter param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                cmd.Prepare();
                cmd.Parameters[0].Value = factType;
                using (SQLiteDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    if (reader.Read())
                        result = reader[0].ToString() == "1";
                }
            }
            return result;
        }

        public static void IgnoreCustomFact(string factType, bool ignore)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("insert or replace into CustomFacts(FactType,Ignore) values(?,?)", InstanceConnection))
            {
                SQLiteParameter param = cmd.CreateParameter();
                param.DbType = DbType.String;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                param.DbType = DbType.Boolean;
                cmd.Parameters.Add(param);
                param = cmd.CreateParameter();
                cmd.Prepare();
                cmd.Parameters[0].Value = factType;
                cmd.Parameters[1].Value = ignore;
                int rowsaffected = cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Cursor Queries

        public static void AddEmptyLocationsToQueue(ConcurrentQueue<FactLocation> queue)
        {
            if (queue is null) return;
            if (InstanceConnection.State != ConnectionState.Open)
                InstanceConnection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand("select location from geocode where foundlocation='' and geocodestatus in (3, 8, 9) order by level", InstanceConnection))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FactLocation loc = FactLocation.LookupLocation(reader[0].ToString());
                        if (!queue.Contains(loc) && loc.Latitude != 0 && loc.Longitude != 0)
                            queue.Enqueue(loc);
                    }
                }
            }
            InstanceConnection.Close();
        }

        //public SQLiteCommand NeedsReverseGeocode()
        //{
        //    return new SQLiteCommand("select location from geocode where foundlocation='' and geocodestatus in (3, 8, 9)", xxx);
        //}

        #endregion

        #region BackupRestore
        public static bool StartBackupRestoreDatabase()
        {
            string tempFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Family Tree Analyzer\Geocodes.s3db.tmp");
            try
            {
                GC.Collect(); // needed to force a cleanup of connections prior to replacing the file.
                if (File.Exists(tempFilename))
                    File.Delete(tempFilename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region EventHandler
        public static event EventHandler GeoLocationUpdated;
        protected static void OnGeoLocationUpdated(FactLocation loc)
        {
            GeoLocationUpdated?.Invoke(loc, EventArgs.Empty);
        }
        #endregion
    }
}
