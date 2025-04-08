using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using MAUSIC.ViewModels;

namespace MAUSIC.Views;

public partial class PlayerView
{
    private bool _isDragging;

    public PlayerView(PlayerViewModel vm)
        :base(vm)
    {
        InitializeComponent();
    }

    private void StartPlayingSong()
    {
        if (ViewModel.CurrentSongPath == null)
        {
            return;
        }

        if (MusicPlayer.CurrentState != MediaElementState.Stopped &&
            MusicPlayer.CurrentState != MediaElementState.Paused)
        {
            MusicPlayer.Pause();
        }

        else if (MusicPlayer.CurrentState != MediaElementState.Playing)
        {
            MusicPlayer.Play();
        }
    }

    private void OnDragStarted(object? sender, EventArgs e)
    {
        _isDragging = true;
    }

    private void OnDragCompleted(object? sender, EventArgs e)
    {
        var newTime = TimeSpan.FromSeconds(Slider.Value);
        MusicPlayer.SeekTo(newTime);
        _isDragging = false;
    }

    private void OnSkipPreviousButtonClicked(object? sender, EventArgs e)
    {
        if (MusicPlayer.Position <= TimeSpan.FromSeconds(5))
        {
            PlayPreviousSong();
        }
        else
        {
            MusicPlayer.SeekTo(TimeSpan.Zero);
        }
    }

    private void OnPlayButtonClicked(object? sender, EventArgs e)
    {
        StartPlayingSong();
    }

    private void OnSkipNextButtonClicked(object? sender, EventArgs e)
    {
        PlayNextSong();
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        PlayNextSong();
    }

    private void PlayNextSong()
    {
        if (ViewModel.CurrentSongPath == null)
        {
            return;
        }

        MusicPlayer.SeekTo(TimeSpan.Zero);
        ViewModel.CurrentTimeSliderValue = 0;
        ViewModel.EnqueueNextSongCommand.Execute(null);
    }

    private void PlayPreviousSong()
    {
        if (ViewModel.CurrentSongPath == null)
        {
            return;
        }

        MusicPlayer.SeekTo(TimeSpan.Zero);
        ViewModel.CurrentTimeSliderValue = 0;
        ViewModel.EnqueuePreviousSongCommand.Execute(null);
    }

    private void OnPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        if (_isDragging)
        {
            return;
        }

        ViewModel.CurrentTimeSliderValue = e.Position.TotalSeconds;
        ViewModel.CurrentTimeStringRepresentation = e.Position.ToString(@"mm\:ss");
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        var newTime = TimeSpan.FromSeconds(Slider.Value);
        ViewModel.CurrentTimeStringRepresentation = newTime.ToString(@"mm\:ss");
    }

    private void OnMediaPlayerStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        PlayButton.ImageSource = ImageSource.FromFile(
            MusicPlayer.CurrentState == MediaElementState.Playing
                ? "pause_24dp.png"
                : "play_arrow_24dp.png");
    }

    private void RequestFilesButton(object? sender, EventArgs e)
    {
        if (ViewModel.CurrentSongPath == null)
        {
            _ = ViewModel.RequestFiles();
        }
    }
}