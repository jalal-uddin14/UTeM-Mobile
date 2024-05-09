using Newtonsoft.Json;
using PusherClient;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolListPage : ContentPage
{
	private PatrolListViewModel viewModel;
	public PatrolListPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as PatrolListViewModel;
        viewModel.OnAppearing();
        await GuardTimeoutBinderAsync();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as Patrol;
        await Shell.Current.GoToAsync($"{nameof(PatrolDetailListPage)}?Id={Item.Id}");
    }

    private async Task GuardTimeoutBinderAsync()
    {
        Pusher pusher = new Pusher(PusherCredential.key, new PusherOptions
        {
            Cluster = PusherCredential.cluster,
            Encrypted = true
        });
        Channel PrivateChannel;
        try
        {
            await pusher.ConnectAsync().ConfigureAwait(false);
            PrivateChannel = await pusher.SubscribeAsync("UTeM-Guard").ConfigureAwait(false);
            PrivateChannel.Bind("guard.patrol-activities." + viewModel.User.Id, PusherListener);
        }
        catch (Exception)
        {

        }
    }

    private async void PusherListener(object sender)
    {
        try
        {
            if (viewModel.IsBusy)
            {
                return;
            }
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(sender));
            if (dictionary.ContainsKey("data"))
            {
                PatrolLocation patrolLocation = JsonConvert.DeserializeObject<PatrolLocation>(dictionary["data"]);
                if (patrolLocation != null && patrolLocation.User != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol Timer Expired.", string.Format("{0} failed to reach checkpoint.", patrolLocation.User.Name))))
                    );
                }
            }

        }
        catch (Exception)
        {

        }
    }
}