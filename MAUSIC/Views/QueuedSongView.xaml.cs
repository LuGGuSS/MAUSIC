using MAUSIC.Models;

namespace MAUSIC.Views;

public partial class QueuedSongView : ContentView
{
    private SongModel? _model;

    public QueuedSongView()
    {
        InitializeComponent();

        _model = BindingContext as SongModel;
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