using CommunityToolkit.Maui.Views;
using MAUSIC.Views;
using BasePageModel = MAUSIC.PageModels.Abstract.BasePageModel;

namespace MAUSIC.Pages.Abstract;

public class BasePage<TPageModel> : ContentPage
    where TPageModel : BasePageModel
{
    protected TPageModel PageModel { get; }

    protected BasePage(TPageModel pageModel)
    {
        PageModel = pageModel;
        BindingContext = pageModel;

        PageModel.InitializeCommand.Execute(null);

        PageModel.ShowPopupAsync = ShowPopup;
    }

    private void ShowPopup(Popup popup)
    {
        MainThread.BeginInvokeOnMainThread(async void () =>
        {
            try
            {
                await this.ShowPopupAsync(popup);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }
}