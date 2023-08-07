using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.PopupViewModels;

namespace UTeM_Mobile.PopupViews;

public partial class TimeoutPopupPage : ContentPage
{
    private TimeoutPopupViewModel viewModel;
	public TimeoutPopupPage()
	{
		InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
    }

	public TimeoutPopupPage(PatrolCheckpoint patrolCheckpoint)
	{
        InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
        viewModel = BindingContext as TimeoutPopupViewModel;
        viewModel.PatrolCheckpoint = patrolCheckpoint;
    }

    public TimeoutPopupPage(CheckpointTimer checkpointTimer)
    {
        InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
        viewModel = BindingContext as TimeoutPopupViewModel;
        viewModel.CheckpointTimer = checkpointTimer;
    }
}