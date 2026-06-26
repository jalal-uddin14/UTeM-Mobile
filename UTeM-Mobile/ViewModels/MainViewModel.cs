using MvvmHelpers;
using System.Collections.ObjectModel;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.ViewModels
{
    public class MainViewModel : BaseViewModel, IInternetConnection
    {
        private AuthToken token;
        private bool isSuccessMessage;
        private bool isErrorMessage;
        private string message;
        private int errorHeight;
        private bool isNotConnected;
        public bool IsSuccessMessage { get => isSuccessMessage; set => SetProperty(ref isSuccessMessage, value); }
        public bool IsErrorMessage
        {
            get => isErrorMessage;
            set
            {
                SetProperty(ref isErrorMessage, value);
                IsSuccessMessage = !value;
            }
        }
        public string Message { get => message; set => SetProperty(ref message, value); }
        public ObservableCollection<ErrorView> Message_list { get; }
        public int ErrorHeight { get => errorHeight; set => SetProperty(ref errorHeight, value); }
        public bool IsNotConnected { get => isNotConnected; set => SetProperty(ref isNotConnected, value); }
        protected AuthToken Token
        {
            get => token;
            set
            {
                SetProperty(ref token, value);
                if (value.ValidTo <= DateTime.Now)
                {
                    Task.Run(async () =>
                    {
                        await LogoutService.LogoutAsync();
                    });
                }
            }
        }

        public MainViewModel()
        {
            Message_list = new ObservableCollection<ErrorView>();
            ErrorHeight = 0;
            IsErrorMessage = false;
            CheckConnectivity();
        }



        public void CheckConnectivity()
        {
            try
            {
                IsErrorMessage = false;
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                IsNotConnected = Connectivity.NetworkAccess != NetworkAccess.Internet;
                StaticMessage.InternetNotConnected = IsNotConnected;
                if (IsNotConnected)
                {
                    Shell.Current.GoToAsync("NoInternetPage");
                    SetErrorMessage("Check your internet connection!");
                }
            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }


        public void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsErrorMessage = false;
            IsNotConnected = e.NetworkAccess != NetworkAccess.Internet;
            StaticMessage.InternetNotConnected = IsNotConnected;
            if (IsNotConnected)
            {
                Shell.Current.GoToAsync("NoInternetPage");
                SetErrorMessage("Check your internet connection!");
            }
            else
            {
                Shell.Current.Navigation.PopToRootAsync();
            }
        }

        protected void SetErrorMessage(string message, Dictionary<string, List<string>> errors = null)
        {
            IsErrorMessage = true;
            Message_list.Clear();
            if (message != null)
            {
                Message_list.Add(new ErrorView { Message = message, Color = "Red" });
                ErrorHeight += 60;
            }
            if (errors != null && errors.Count > 0)
            {
                foreach (var m in errors)
                {
                    Message_list.Add(new ErrorView { Message = m.Value[0], Color = "Red" });
                }
                ErrorHeight += errors.Count * 20;
            }
        }
    }
}
