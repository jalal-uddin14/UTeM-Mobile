using MvvmHelpers.Commands;
using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Guard
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : MainViewModel, IOnAppearing
    {
        static IDispatcherTimer patrolCheckTimer = Application.Current.Dispatcher.CreateTimer();
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;

        private readonly ITokenStorageService _tokenService;

        private DateTime? expectedChecktime;
        private string id;
        private Patrol patrol;
        private PatrolDetail patrolDetail;
        private string routeCheckTimeString;
        private bool hasTime;
        private bool timeExceeded;

        public string Id { get => id; set => id = value; }
        public DateTime? ExpectedChecktime { get => expectedChecktime; set => SetProperty(ref expectedChecktime, value); }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }
        public string RouteCheckTimeString { get => routeCheckTimeString; set => SetProperty(ref routeCheckTimeString, value); }
        public bool HasTime 
        { 
            get => hasTime;
            set
            {
                TimeExceeded = !value;
                SetProperty(ref hasTime, value);
            }
        }
        public bool TimeExceeded { get => timeExceeded; set => SetProperty(ref timeExceeded, value); }

        public PatrolDetailViewModel(ITokenStorageService tokenService)
        {
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
            Patrol = new Patrol();
            _tokenService = tokenService;
        }

        public async Task OnAppearing()
        {
            ExpectedChecktime = null;
            IsErrorMessage = false;
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (tokenJson != null)
                {
                    await GetPatrolDetailAsync();
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetPatrolDetailAsync()
        {
            try
            {
                IsBusy = true;
                if (Id != null)
                {
                    string url = "patrolDetails/" + Id;
                    ObjectResponse<PatrolDetail> response = await _genericPatrolDetailService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        PatrolDetail = response.Data;
                        if (PatrolDetail.Patrol != null)
                        {
                            Patrol = PatrolDetail.Patrol;
                            await CheckRouteCheckpointStatusAsync();
                        }
                    }
                    else
                    {
                        IsErrorMessage = false;
                        SetErrorMessage(response.Message, response.Errors);
                    }
                }
                else
                {
                    SetErrorMessage("Server error occured.");
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");

            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CheckRouteCheckpointStatusAsync()
        {
            try
            {
                string url = "";
                for (int i = 0; i < Patrol.Route.RouteCheckpoints.Count; i++)
                {
                    Patrol.Route.RouteCheckpoints[i].IsNotLast = i < Patrol.Route.RouteCheckpoints.Count - 1;
                    url = "patrolCheckpoints/check";
                    var content = new { patrolDetailId = PatrolDetail.Id, checkpointId = Patrol.Route.RouteCheckpoints[i].CheckpointId };
                    ObjectResponse<PatrolCheckpoint> res = await _patrolCheckpointService.PostAsync(url, content, Token);
                    if (res.IsSuccess && res.Data != null)
                    {
                        Patrol.Route.RouteCheckpoints[i].IsChecked = res.Data.Status == "Completed";
                        Patrol.Route.RouteCheckpoints[i].IsScheduled = res.Data.Status == "Scheduled";
                        Patrol.Route.RouteCheckpoints[i].IsMissed = res.Data.Status == "Missed";
                        Patrol.Route.RouteCheckpoints[i].NotFound = false;
                        if (Patrol.TimerEnabled)
                        {
                            if (res.Data.Status == "Scheduled")
                            {
                                Patrol.Route.RouteCheckpoints[i].HasSchedule = res.Data.ExpectedCheckedTime != null;
                                Patrol.Route.RouteCheckpoints[i].ExpectedCheckedTime = res.Data.ExpectedCheckedTime;
                                ExpectedChecktime = res.Data.ExpectedCheckedTime;
                                RunPatrolDetailTimer();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        public void RunPatrolDetailTimer()
        {
            patrolCheckTimer.Interval = TimeSpan.FromSeconds(1);
            patrolCheckTimer.Tick += (s, e) =>
            {
                try
                {
                    HasTime = DateTime.UtcNow.AddHours(8) < expectedChecktime;
                    RouteCheckTimeString = (expectedChecktime - DateTime.UtcNow.AddHours(8))?.ToString("c");
                    RouteCheckTimeString = expectedChecktime < DateTime.UtcNow.AddHours(8) ? RouteCheckTimeString[..9] : RouteCheckTimeString[..8];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            patrolCheckTimer.Start();
        }
    }
}
