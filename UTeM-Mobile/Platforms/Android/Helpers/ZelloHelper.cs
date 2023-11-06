using Android.Content;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Platforms.Android.Helpers
{
    public class ZelloHelper : IZelloHelper
    {
        public void OpenZello()
        {
            Intent intent = new Intent("com.zello.ptt.down");
            intent.PutExtra("com.zello.stayHidden", true);
            var activity = Platform.CurrentActivity;
            activity.SendBroadcast(intent);
        }
        public void CloseZello()
        {
            Intent intent = new Intent("com.zello.ptt.up");
            intent.PutExtra("com.zello.stayHidden", true);
            var activity = Platform.CurrentActivity;
            activity.SendBroadcast(intent);
        }
    }
}
