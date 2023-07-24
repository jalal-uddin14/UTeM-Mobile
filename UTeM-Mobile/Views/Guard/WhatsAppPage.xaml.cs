namespace UTeM_Mobile.Views.Guard;

public partial class WhatsAppPage : ContentPage
{
	public WhatsAppPage()
	{
		InitializeComponent();
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