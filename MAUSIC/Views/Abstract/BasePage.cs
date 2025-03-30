using MAUSIC.ViewModels.Abstract;
using Microsoft.Maui.Controls;

namespace MAUSIC.Views.Abstract;

public class BasePage<TViewModel> : ContentPage
    where TViewModel : BaseViewModel
{
    private bool _isInitialized;
    protected TViewModel ViewModel { get; }

    protected BasePage(TViewModel vm)
    {
        ViewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_isInitialized)
        {
            return;
        }

        ViewModel.InitializeCommand.Execute(null);
        _isInitialized = true;
    }
}