using UTeM_Mobile.Interfaces;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class ReportSendPage : ContentPage
{
	public ReportSendPage(ReportSendViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IOnAppearing vm)
        {
            vm.OnAppearing();
        }
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