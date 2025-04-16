using BasePageModel = MAUSIC.PageModels.Abstract.BasePageModel;

namespace MAUSIC.Pages.Abstract;

public class BasePage<TPageModel> : ContentPage
    where TPageModel : BasePageModel
{
    protected TPageModel PageModel { get; }

    protected BasePage(TPageModel vm)
    {
        PageModel = vm;
        BindingContext = vm;

        PageModel.InitializeCommand.Execute(null);
    }
}