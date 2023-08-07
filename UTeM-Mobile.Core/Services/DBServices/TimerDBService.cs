using SQLite;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class TimerDBService
    {
        static CheckpointTimer checkpointTimer;
        static SQLiteAsyncConnection checkpointTimerDB;
        public async static Task InitDB()
        {
            if (checkpointTimerDB != null)
            {
                return;
            }
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + LocalCredential.LocalDBName;
            checkpointTimerDB = new SQLiteAsyncConnection(databasePath);
            try
            {
                await CheckPermission();
                await checkpointTimerDB.CreateTableAsync<CheckpointTimer>();
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
                var table = await checkpointTimerDB.GetTableInfoAsync("CheckpointTimerDB");
                if (table.Count <= 0)
                {
                    await checkpointTimerDB.CreateTableAsync<CheckpointTimer>();
                }
                await checkpointTimerDB.InsertOrReplaceAsync(p);
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
                var query = checkpointTimerDB.Table<CheckpointTimer>();

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
                await checkpointTimerDB.DeleteAllAsync<CheckpointTimer>();
                checkpointTimer = null;
            }
            catch (Exception e)
            {

            }
        }

        public async static Task CheckPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status == PermissionStatus.Denied)
            {
                status = await MainThread.InvokeOnMainThreadAsync(() => Permissions.RequestAsync<Permissions.StorageWrite>()); ;
            }
        }
    }
}
