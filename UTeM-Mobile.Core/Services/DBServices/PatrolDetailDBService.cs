using SQLite;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Microsoft.Maui.ApplicationModel;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class PatrolDetailDBService
    {
        static DBPatrolDetail patrolDetail;
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
                await db.CreateTableAsync<DBPatrolDetail>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task Insert(DBPatrolDetail p)
        {
            try
            {
                patrolDetail = p;
                await InitDB();
                var table = await db.GetTableInfoAsync(nameof(DBPatrolDetail));
                if (table.Count <= 0)
                {
                    await CheckPermission();
                    await db.CreateTableAsync<DBPatrolDetail>();
                }
                await db.InsertOrReplaceAsync(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<DBPatrolDetail> Get()
        {
            await InitDB();
            try
            {
                var query = db.Table<DBPatrolDetail>();

                var result = await query.ToListAsync();

                foreach (var s in result)
                {
                    patrolDetail = s;
                }
            }
            catch (Exception e)
            {

            }

            return patrolDetail;
        }

        public async static Task Delete()
        {
            try
            {
                await InitDB();
                await db.DeleteAllAsync<DBPatrolDetail>();
                patrolDetail = null;
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
