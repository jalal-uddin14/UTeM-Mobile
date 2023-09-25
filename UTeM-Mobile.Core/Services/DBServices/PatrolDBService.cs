using SQLite;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class PatrolDBService
    {
        static Patrol patrol;
        static SQLiteAsyncConnection patrolDb;
        public async static Task InitDB()
        {
            if (patrolDb != null)
            {
                return;
            }
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + LocalCredential.LocalDBName;
            patrolDb = new SQLiteAsyncConnection(databasePath);
            try
            {
                await patrolDb.CreateTableAsync<Patrol>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task Insert(Patrol p)
        {
            try
            {
                patrol = p;
                await InitDB();
                var table = await patrolDb.GetTableInfoAsync(nameof(Patrol));
                if (table.Count <= 0)
                {
                    await CheckPermission();
                    await patrolDb.CreateTableAsync<Patrol>();
                }
                await patrolDb.InsertOrReplaceAsync(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<Patrol> Get()
        {
            await InitDB();
            try
            {
                var query = patrolDb.Table<Patrol>();

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
                await patrolDb.DeleteAllAsync<Patrol>();
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
