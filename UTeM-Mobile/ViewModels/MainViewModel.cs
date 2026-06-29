using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.ViewModels
{
    public partial class MainViewModel : ObservableObject, IInternetConnection
    {
        [ObservableProperty]
        protected bool isBusy;
        
        [ObservableProperty]
        protected bool isNotBusy;

        [ObservableProperty]
        private AuthToken token;

        [ObservableProperty]
        private bool isSuccessMessage;

        [ObservableProperty]
        private bool isErrorMessage;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private int errorHeight;

        [ObservableProperty]
        private bool isNotConnected;

        partial void OnIsErrorMessageChanged(bool value)
        {
            IsSuccessMessage = !value;
        }

        public ObservableCollection<ErrorView> Message_list { get; }


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
