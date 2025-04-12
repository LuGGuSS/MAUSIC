using CommunityToolkit.Maui.Core.Primitives;
using PlayerPageModel = MAUSIC.PageModels.PlayerPageModel;

namespace MAUSIC.Pages;

public partial class PlayerPage
{
    private bool _isDragging;

    public PlayerPage(PlayerPageModel vm)
        :base(vm)
    {
        InitializeComponent();
    }

    private void StartPlayingSong()
    {
        if (PageModel.CurrentSongPath == null)
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
        if (PageModel.CurrentSongPath == null)
        {
            return;
        }

        MusicPlayer.SeekTo(TimeSpan.Zero);
        PageModel.CurrentTimeSliderValue = 0;
        PageModel.EnqueueNextSongCommand.Execute(null);
    }

    private void PlayPreviousSong()
    {
        if (PageModel.CurrentSongPath == null)
        {
            return;
        }

        MusicPlayer.SeekTo(TimeSpan.Zero);
        PageModel.CurrentTimeSliderValue = 0;
        PageModel.EnqueuePreviousSongCommand.Execute(null);
    }

    private void OnPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        if (_isDragging)
        {
            return;
        }

        PageModel.CurrentTimeSliderValue = e.Position.TotalSeconds;
        PageModel.CurrentTimeStringRepresentation = e.Position.ToString(@"mm\:ss");
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        var newTime = TimeSpan.FromSeconds(Slider.Value);
        PageModel.CurrentTimeStringRepresentation = newTime.ToString(@"mm\:ss");
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
        if (PageModel.CurrentSongPath == null)
        {
            _ = PageModel.RequestFiles();
        }
    }
}