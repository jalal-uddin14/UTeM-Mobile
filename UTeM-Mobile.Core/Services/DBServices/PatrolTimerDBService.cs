using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.StaticCredentials;
using Xamarin.Essentials;

namespace UTeM_Mobile.Core.Services.DBServices
{
    public class PatrolTimerDBService
    {
        public class TimerDBService
        {
            static PatrolTimer patrolTimer;
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
                    await db.CreateTableAsync<PatrolTimer>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            public async static Task Insert(PatrolTimer p)
            {
                try
                {
                    patrolTimer = p;
                    await InitDB();
                    var table = await db.GetTableInfoAsync(nameof(PatrolTimer));
                    if (table.Count <= 0)
                    {
                        await CheckPermission();
                        await db.CreateTableAsync<PatrolTimer>();
                    }
                    await db.InsertOrReplaceAsync(p);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            public async static Task<PatrolTimer> Get()
            {
                await InitDB();
                try
                {
                    var query = db.Table<PatrolTimer>();

                    var result = await query.ToListAsync();

                    foreach (var s in result)
                    {
                        patrolTimer = s;
                    }
                }
                catch (Exception e)
                {

                }

                return patrolTimer;
            }

            public async static Task Delete()
            {
                try
                {
                    await InitDB();
                    await db.DeleteAllAsync<PatrolTimer>();
                    patrolTimer = null;
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
}
