using Android.App;
using Android.Runtime;
using UTeM_Mobile.Platforms.Android.Helpers;

namespace UTeM_Mobile;

#if DEBUG                                   // connect to local service on the
[Application(UsesCleartextTraffic = true)]  // emulator's host for debugging,
#else                                       // access via http://10.0.2.2
[Application(UsesCleartextTraffic = true)]
#endif
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        DependencyService.Register<DroidKeyboardHelper>();
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
