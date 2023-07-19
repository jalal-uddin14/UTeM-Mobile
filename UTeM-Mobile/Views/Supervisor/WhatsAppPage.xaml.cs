namespace UTeM_Mobile.Views.Supervisor;

public partial class WhatsAppPage : ContentPage
{
	public WhatsAppPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var phoneNumber = "01924241969";
        try
        {
            await Launcher.Default.OpenAsync($"whatsapp://send?phone=+88{phoneNumber}");
        }
        catch (Exception ex)
        {

        }

    }
}