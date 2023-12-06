using SQLite;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class LocalDBService
    {
        static AuthToken token;
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
                await db.CreateTableAsync<AuthToken>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task InsertToken(AuthToken t)
        {
            try
            {
                token = t;
                await InitDB();
                var table = await db.GetTableInfoAsync(nameof(AuthToken));
                if (table.Count <= 0)
                {
                    await CheckPermission();
                    await db.CreateTableAsync<AuthToken>();
                }
                await db.InsertOrReplaceAsync(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<AuthToken> GetToken()
        {
            await InitDB();
            try
            {
                var query = db.Table<AuthToken>();

                var result = await query.ToListAsync();

                foreach (var s in result)
                {
                    token = s;
                }
            }
            catch (Exception)
            {

            }

            return token;
        }

        public async static Task RemoveToken()
        {
            try
            {
                await InitDB();
                await db.DeleteAllAsync<AuthToken>();
                token = null;
            }
            catch (Exception)
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
