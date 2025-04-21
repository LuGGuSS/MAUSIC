using CommunityToolkit.Maui.Core.Primitives;
using MAUSIC.Data.Constants;
using PlayerPageModel = MAUSIC.PageModels.PlayerPageModel;

namespace MAUSIC.Pages;

[QueryProperty(nameof(StartPlayingNavigationProperty), "StartPlaying")]
public partial class PlayerPage
{
    private bool _isDragging;

    public PlayerPage(PlayerPageModel pageModel)
        :base(pageModel)
    {
        InitializeComponent();
    }

    public bool StartPlayingNavigationProperty
    {
        set
        {
            if (value)
            {
                MusicPlayer.Stop();

                StartPlayingSong();
            }
        }
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
        _isDragging = false;

        if (PageModel.CurrentSongPath == null)
        {
            Slider.Value = 0;
            return;
        }

        var newTime = TimeSpan.FromSeconds(Slider.Value);
        MusicPlayer.SeekTo(newTime);
    }

    private void OnRepeatButtonClicked(object? sender, EventArgs e)
    {
        PageModel.IsOnRepeat = !PageModel.IsOnRepeat;
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

    private void OnShuffleButtonClicked(object? sender, EventArgs e)
    {
        PageModel.IsShuffled = !PageModel.IsShuffled;
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

    private void OnDislikeClicked(object? sender, EventArgs e)
    {
        PageModel.ChangeRecommendationWeightCommand.ExecuteAsync(RecommendationConstants.UserDisliked);
    }

    private void OnLikeClicked(object? sender, EventArgs e)
    {
        PageModel.ChangeRecommendationWeightCommand.ExecuteAsync(RecommendationConstants.UserLiked);
    }

    private void OnFavouriteClicked(object? sender, EventArgs e)
    {
        PageModel.ToggleFavouriteCommand.ExecuteAsync(null);
    }
}