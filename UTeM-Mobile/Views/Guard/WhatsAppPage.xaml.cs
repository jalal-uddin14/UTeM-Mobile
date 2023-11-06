using UTeM_Mobile.Interfaces;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class WhatsAppPage : ContentPage
{
    private WhatsAppViewModel viewModel;
	public WhatsAppPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as WhatsAppViewModel;
        viewModel.OnAppearing();
    }

    private async void Button_Pressed(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Microphone permission is required to use this feature.", "OK");
                return;
            }
        }
        talkFrame.BackgroundColor = Colors.Red;
        talkLabel.Text = "You are now talking...";
        DependencyService.Get<IZelloHelper>().OpenZello();
    }

    private void Button_Released(object sender, EventArgs e)
    {
        talkLabel.Text = "Press and hold on microphone to talk";
        talkFrame.BackgroundColor = Colors.Blue;
        DependencyService.Get<IZelloHelper>().CloseZello();
    }
}