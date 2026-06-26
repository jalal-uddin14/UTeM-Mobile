using SQLite;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Microsoft.Maui.ApplicationModel;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class PatrolDBService
    {
        static DBPatrol patrol;
        static SQLiteAsyncConnection db;
        public async static Task InitDB()
        {
            if (db != null)
            {
                return;
            }
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + LocalCredential.LocalDBName;
            db = new SQLiteAsyncConnection(databasePath);
            try
            {
                await db.CreateTableAsync<DBPatrol>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task Insert(DBPatrol p)
        {
            try
            {
                patrol = p;
                await InitDB();
                var table = await db.GetTableInfoAsync(nameof(DBPatrol));
                if (table.Count <= 0)
                {
                    await CheckPermission();
                    await db.CreateTableAsync<DBPatrol>();
                }
                await db.InsertOrReplaceAsync(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<DBPatrol> Get()
        {
            await InitDB();
            try
            {
                var query = db.Table<DBPatrol>();

                var result = await query.ToListAsync();

                foreach (var s in result)
                {
                    patrol = s;
                }
            }
            catch (Exception e)
            {

            }

            return patrol;
        }

        public async static Task Delete()
        {
            try
            {
                await db.DeleteAllAsync<DBPatrol>();
                patrol = null;
            }
            catch (Exception e)
            {

            }
        }

        public async static Task CheckPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await MainThread.InvokeOnMainThreadAsync(() => Permissions.RequestAsync<Permissions.StorageWrite>()); ;
            }
        }
    }
}
