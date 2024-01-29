using UTeM_Mobile.Data.Models;
using UTeM_Mobile.PopupViewModels;

namespace UTeM_Mobile.PopupViews;

public partial class NextPatrolPopupPage : ContentPage
{
    private NextPatrolPopupViewModel viewModel;
    public NextPatrolPopupPage()
	{
		InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
    }

    public NextPatrolPopupPage(PatrolDetail patrolDetail)
    {
        InitializeComponent();
        this.BackgroundColor = new Color(0f, 0f, 0f, 0.7f);
        viewModel = BindingContext as NextPatrolPopupViewModel;
        viewModel.PatrolDetail = patrolDetail;
    }
}