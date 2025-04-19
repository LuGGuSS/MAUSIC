using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.Models;

namespace MAUSIC.Views;

public partial class SongView : ContentView
{
    public SongView()
    {
        InitializeComponent();
    }

    private async void OnMoreClicked(object? sender, EventArgs e)
    {
        try
        {
            if (BindingContext is SongModel model)
            {
                await model.OpenMoreAsync();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}