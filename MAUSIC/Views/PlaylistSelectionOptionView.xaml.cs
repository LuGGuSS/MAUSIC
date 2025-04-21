using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUSIC.Views;

public partial class PlaylistSelectionOptionView : ContentView
{
    public PlaylistSelectionOptionView()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => Button.Text;
        set => Button.Text = value;
    }

    public Action Callback { get; set; }

    private void OnClick(object? sender, EventArgs e)
    {
        Callback?.Invoke();
    }
}