using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.StaticProperties
{
    public static class StaticMessage
    {
        public static bool HasModal { get; set; } = false;
        public static bool InternetNotConnected { get; set; } = false;
        public static async Task<bool> ShowInternetMessage()
        {
            if (InternetNotConnected)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetInternetMessage()))
                );
                return false;
            }
            return true;
        }
    }
}
