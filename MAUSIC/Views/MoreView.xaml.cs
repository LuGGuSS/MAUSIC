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
    public MoreView(Action<object?>? callback)
    {
        InitializeComponent();

        _callback = callback;

        var model = new MoreModel(ClosePopupWithCallback);

        BindingContext = model;

    }

    private void ClosePopupWithCallback(object? obj)
    {
        _callback?.Invoke(obj);

        Close();
    }
}