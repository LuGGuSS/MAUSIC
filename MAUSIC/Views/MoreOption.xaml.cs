using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.Models;

namespace MAUSIC.Views;

public partial class MoreOption : ContentView
{
    public MoreOption()
    {
        InitializeComponent();
    }

    private void OnClick(object? sender, EventArgs e)
    {
        var model = BindingContext as MoreOptionModel;

        model?.CloseAction?.Invoke(model.Title);
    }
}