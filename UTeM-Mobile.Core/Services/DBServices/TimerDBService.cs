using SQLite;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class TimerDBService
    {
        static CheckpointTimer checkpointTimer;
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
                await db.CreateTableAsync<CheckpointTimer>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task Insert(CheckpointTimer p)
        {
            try
            {
                checkpointTimer = p;
                await InitDB();
                var table = await db.GetTableInfoAsync(nameof(CheckpointTimer));
                if (table.Count <= 0)
                {
                    await CheckPermission();
                    await db.CreateTableAsync<CheckpointTimer>();
                }
                await db.InsertOrReplaceAsync(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<CheckpointTimer> Get()
        {
            await InitDB();
            try
            {
                var query = db.Table<CheckpointTimer>();

                var result = await query.ToListAsync();

                foreach (var s in result)
                {
                    checkpointTimer = s;
                }
            }
            catch (Exception e)
            {

            }

            return checkpointTimer;
        }

        public async static Task Delete()
        {
            try
            {
                await InitDB();
                await db.DeleteAllAsync<CheckpointTimer>();
                checkpointTimer = null;
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
