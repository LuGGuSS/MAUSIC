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

        model?.CloseAction?.Invoke(model.ResultItem);
    }
}