using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using MAUSIC.ViewModels;

namespace MAUSIC.Views;

public partial class PlayerView
{
    public PlayerView(PlayerViewModel vm)
        :base(vm)
    {
        InitializeComponent();
    }

    private async Task StartPlayingSong()
    {
        if (ViewModel.CurrentSongPath == null)
        {
            await ViewModel.RequestFiles();
        }

        if (MusicPlayer.CurrentState != MediaElementState.Stopped &&
            MusicPlayer.CurrentState != MediaElementState.Paused)
        {
            MusicPlayer.Pause();
        }

        else if (MusicPlayer.CurrentState != MediaElementState.Playing)
        {
            if (MusicPlayer.Position >= MusicPlayer.Duration)
            {
                await MusicPlayer.SeekTo(ViewModel.CurrentTime);
            }

            MusicPlayer.Play();
        }
    }

    private void DragCompleted(object? sender, EventArgs e)
    {
        var newPosition = TimeSpan.FromSeconds((int)Slider.Value);
        MusicPlayer.SeekTo(newPosition);
    }

    private void OnPlayButtonClicked(object? sender, EventArgs e)
    {
        _ = StartPlayingSong();
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        ViewModel.CurrentSongIndex++;
        ViewModel.EnqueueNextSongCommand.Execute(null);
        MusicPlayer.SeekTo(TimeSpan.Zero);
        MusicPlayer.Play();
    }

    private void OnPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        ViewModel.CurrentTime = e.Position;
        ViewModel.CurrentTimeSliderValue = e.Position.TotalSeconds;
        ViewModel.CurrentTimeStringRepresentation = e.Position.ToString(@"mm\:ss");
    }
}