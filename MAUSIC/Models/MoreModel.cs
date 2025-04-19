using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;

namespace MAUSIC.Models;

public partial class MoreModel
{
    private readonly Action<object?>? _callback;
    public MoreModel(Action<object?>? callback)
    {
        _callback = callback;

        var models = new List<MoreOptionModel>
        {
            new() { Title = MoreMenuConstants.AddToPlaylist, CloseAction = OptionSelected},
            new() { Title = MoreMenuConstants.RemoveFromPlaylist, CloseAction = OptionSelected},
            new() { Title = MoreMenuConstants.Favourite, CloseAction = OptionSelected},
            new() { Title = MoreMenuConstants.AddToQueue, CloseAction = OptionSelected},
            new() { Title = MoreMenuConstants.RemoveFromQueue, CloseAction = OptionSelected},
        };

        MoreOptions = models;
    }


    public List<MoreOptionModel> MoreOptions { get; set; }

    private void OptionSelected(string title)
    {
        MainThread.BeginInvokeOnMainThread(async void () =>
        {
            try
            {
                _callback?.Invoke(title);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }
}