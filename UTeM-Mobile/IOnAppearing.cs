using UTeM_Mobile.Models;

namespace UTeM_Mobile
{
    public interface IOnAppearing
    {
        void OnAppearing();
        Task GetTokenAsync();
    }
}
