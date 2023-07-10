using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;
        private string id;
        private Patrol patrol;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }

        public PatrolDetailViewModel()
        {
            Patrol = new Patrol { Guard = new ApplicationUser(), Route = new Route() };
            _genericService = new GenericService<Patrol>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetPatrolDetailAsync(); });
        }

        private async Task GetPatrolDetailAsync()
        {
            if (Id != null)
            {
                string url = "patrols/" + Id;
                ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url);
                Patrol = response.Data;
            }
            else
            {
                Console.WriteLine("Exception");
            }
        }
    }
}
