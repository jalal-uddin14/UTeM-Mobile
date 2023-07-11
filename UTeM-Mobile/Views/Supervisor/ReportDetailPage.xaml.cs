using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class ReportDetailPage : ContentPage
{
	private ReportDetailViewModel viewModel;
	public ReportDetailPage()
	{
		InitializeComponent();
		viewModel = BindingContext as ReportDetailViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.OnAppearing();
    }
}