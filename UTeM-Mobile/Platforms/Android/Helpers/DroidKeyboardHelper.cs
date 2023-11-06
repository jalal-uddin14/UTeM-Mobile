using Android.Content;
using Android.Views.InputMethods;
using UTeM_Mobile.Interfaces;
using Application = Android.App.Application;

namespace UTeM_Mobile.Platforms.Android.Helpers
{
    public class DroidKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            try
            {
                Context context = Application.Context;
                var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                if (inputMethodManager != null)
                {
                    var activity = Platform.CurrentActivity;
                    var token = activity.Window.CurrentFocus.WindowToken;
                    inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                    activity.Window.DecorView.ClearFocus();
                }
            }
            catch (Exception) { }
        }
    }
}
