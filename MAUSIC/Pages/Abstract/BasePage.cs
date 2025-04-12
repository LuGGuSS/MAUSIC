using BasePageModel = MAUSIC.PageModels.Abstract.BasePageModel;

namespace MAUSIC.Pages.Abstract;

public class BasePage<TPageModel> : ContentPage
    where TPageModel : BasePageModel
{
    private bool _isInitialized;
    protected TPageModel PageModel { get; }

    protected BasePage(TPageModel vm)
    {
        PageModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_isInitialized)
        {
            return;
        }

        PageModel.InitializeCommand.Execute(null);
        _isInitialized = true;
    }
}