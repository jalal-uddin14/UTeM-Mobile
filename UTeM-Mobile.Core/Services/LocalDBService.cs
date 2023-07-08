using SQLite;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services
{
    public class LocalDBService
    {
        static ApplicationUser user;
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
                await CheckPermission();
                await db.CreateTableAsync<ApplicationUser>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async static Task InsertToken(ApplicationUser t)
        {
            try
            {
                user = t;
                await InitDB();
                var table = await db.GetTableInfoAsync("User");
                if (table.Count <= 0)
                {
                    await db.CreateTableAsync<ApplicationUser>();
                }
                await db.InsertOrReplaceAsync(t);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async static Task<ApplicationUser> GetToken()
        {
            await InitDB();
            try
            {
                var query = db.Table<ApplicationUser>();

                var result = await query.ToListAsync();

                foreach (var s in result)
                {
                    user = s;
                }
            }
            catch (Exception e)
            {

            }

            return user;
        }

        public async static Task RemoveToken()
        {
            try
            {
                await InitDB();
                await db.DeleteAllAsync<ApplicationUser>();
                user = null;
            }
            catch (Exception e)
            {

            }
        }

        private async static Task CheckPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status == PermissionStatus.Denied)
            {
                status = await MainThread.InvokeOnMainThreadAsync(() => Permissions.RequestAsync<Permissions.StorageWrite>()); ;
            }
        }
    }
}
