using UTeM_Mobile.Interfaces;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class ReportSendPage : ContentPage
{
	private ReportSendViewModel viewModel;
	public ReportSendPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel = BindingContext as ReportSendViewModel;
		viewModel.OnAppearing();
    }

    private void Editor_Completed(object sender, EventArgs e)
    {
        try
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }
        catch (Exception)
        {

        }
    }
}