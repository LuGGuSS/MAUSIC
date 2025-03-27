using MAUSIC.ViewModels.Abstract;

namespace MAUSIC.Views.Abstract;

public class BaseView<TViewModel> : ContentPage
    where TViewModel : BaseViewModel
{
    public TViewModel ViewModel { get; }

    public BaseView(TViewModel vm)
    {
        ViewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ViewModel.InitializeCommand.Execute(null);
    }
}