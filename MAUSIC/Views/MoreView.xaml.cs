using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using MAUSIC.Models;

namespace MAUSIC.Views;

public partial class MoreView : Popup
{
    private readonly Action<object?>? _callback;

    public MoreView(List<MoreOptionModel> models, Action<object?>? callback)
    {
        InitializeComponent();

        _callback = callback;

        foreach (var model in models)
        {
            model.CloseAction = CloseAction;
        }

        BindingContext = new MoreModel(models);
    }

    private void CloseAction(object? result)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                _callback?.Invoke(result);

                Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }
}