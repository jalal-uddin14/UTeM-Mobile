using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(
            string title,
            string message,
            string cancel = "OK")
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current!.Windows[0].Page!
                    .DisplayAlert(title, message, cancel));
        }

        public Task<bool> ShowConfirmationAsync(
            string title,
            string message,
            string accept = "Yes",
            string cancel = "No")
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current!.Windows[0].Page!
                    .DisplayAlert(title, message, accept, cancel));
        }
    }
}
