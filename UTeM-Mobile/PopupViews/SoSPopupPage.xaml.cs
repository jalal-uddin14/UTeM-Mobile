using UTeM_Mobile.PopupViewModels;

namespace UTeM_Mobile.PopupViews;

public partial class SoSPopupPage : ContentPage
{
	private SoSPopupViewModel viewModel;
	public SoSPopupPage()
	{
		InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.9f);
    }

	public SoSPopupPage(Dictionary<string, string> notification)
	{
        InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
		viewModel = BindingContext as SoSPopupViewModel;
		viewModel.Notification = notification;
    }
}