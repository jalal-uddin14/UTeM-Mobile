namespace UTeM_Mobile.Interfaces
{
    public interface IInternetConnection
    {
        void CheckConnectivity();
        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e);
    }
}
