using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViewModels;

namespace UTeM_Mobile.PopupViews;

public partial class MessagePopupPage : ContentPage
{
	private MessagePopupViewModel viewModel;
	public MessagePopupPage()
	{
		InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.9f);
    }

    public MessagePopupPage(PopMessage popMessage)
    {
        InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
        viewModel = BindingContext as MessagePopupViewModel;
        viewModel.PopMessage = popMessage;
    }
}