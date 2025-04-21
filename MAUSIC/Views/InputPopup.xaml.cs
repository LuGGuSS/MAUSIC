using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUSIC.Views;

public partial class InputPopup
{
    private string _playlistName = String.Empty;

    public InputPopup()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        _playlistName = e.NewTextValue;
    }

    private void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        Close(String.Empty);
    }

    private void OnCreateButtonClicked(object? sender, EventArgs e)
    {
        Close(_playlistName);
    }
}