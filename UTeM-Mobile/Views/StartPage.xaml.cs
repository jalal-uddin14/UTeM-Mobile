using System.Threading.Tasks;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.Views;

public partial class StartPage : ContentPage
{
	public StartPage(StartViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IOnAppearing vm)
        {
            await vm.OnAppearing();
        }
    }
}