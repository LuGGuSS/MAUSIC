using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;
using MAUSIC.Views;
#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#endif

namespace MAUSIC.PageModels;

public partial class FoldersPageModel : BasePageModel
{
    private readonly StorageManager _storageManager;
    private readonly QueueManager _queueManager;
    private readonly SongsManager _songsManager;
    private readonly PlaylistManager _playlistManager;

    private readonly FolderModel _initialModel = new ();

    [ObservableProperty] private FolderModel _selectedFolderModel;

    [ObservableProperty] private bool _shouldShowFooter;

    public FoldersPageModel(
        StorageManager storageManager,
        QueueManager queueManager,
        SongsManager songsManager,
        PlaylistManager playlistManager)
    {
        _storageManager = storageManager;
        _queueManager = queueManager;
        _songsManager = songsManager;
        _playlistManager = playlistManager;
    }

    protected override async Task InitializeAsync()
    {
        var folderEntities = await _storageManager.GetAllFolders();

        if (folderEntities == null || folderEntities.Count == 0)
        {
            return;
        }

        await TryPopulateWithFolders(folderEntities);
    }

    private async Task TryPopulateWithFolders(List<FolderEntity> folderEntities)
    {
        foreach (var folderModel in folderEntities.Select(entity => entity.Map()))
        {
            if (folderModel == null)
            {
                continue;
            }

            var models = await _storageManager.GetFolderContents(folderModel, GetPopupTask);

            foreach (var model in models)
            {
                if (model is FolderModel initialFolderModel)
                {
                    initialFolderModel.Parent = _initialModel;
                }

                _initialModel.InnerItems.Add(model);
            }
        };

        SelectedFolderModel = _initialModel;
        UpdateShouldShowFooter();
    }


    [RelayCommand]
    public async Task FolderSelected(FolderModel model)
    {
        if (model.InnerItems.Count == 0)
        {
            await _storageManager.PopulateFolderWithChildren(model, GetPopupTask);
        }

        SelectedFolderModel = model;
        UpdateShouldShowFooter();
    }

    [RelayCommand]
    public async Task SongSelected(SongModel model)
    {
        var songs = SelectedFolderModel.InnerItems.OfType<SongModel>().ToObservableCollection();

        var queue = _queueManager.GetCurrentSongsQueue?.Invoke();

        if (queue == null)
        {
            return;
        }

        queue.Songs = songs;
        queue.CurrentSongIndex = songs.IndexOf(model);

        await Shell.Current.GoToAsync($"///PlayerPage?StartPlaying={true}");
    }

    [RelayCommand]
    private void NavigateBack()
    {
        if (SelectedFolderModel.Parent == null)
        {
            return;
        }

        SelectedFolderModel = SelectedFolderModel.Parent;
        UpdateShouldShowFooter();
    }

    public async Task RequestFiles()
    {
#if ANDROID
        if ((int)Build.VERSION.SdkInt >= 33) // Android 13+
        {
            // For Android 13+, use Media permissions
#pragma warning disable CA1416
            var permission = Manifest.Permission.ReadMediaAudio;
#pragma warning restore CA1416
            if (ContextCompat.CheckSelfPermission(Platform.CurrentActivity, permission) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { permission }, 0);
            }
        }
        else
        {
            var permission = Manifest.Permission.ReadExternalStorage;
            if (ContextCompat.CheckSelfPermission(Platform.CurrentActivity, permission) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { permission }, 0);
            }
        }
#endif

        var files = await _storageManager.PickFolder();

        if (files == null || files.Count == 0)
        {
            return;
        }

        var folderEntities = await _storageManager.GetAllFolders();

        if (folderEntities == null || folderEntities.Count == 0)
        {
            return;
        }

        await TryPopulateWithFolders(folderEntities);
    }

    private void UpdateShouldShowFooter()
    {
        ShouldShowFooter = SelectedFolderModel.Parent != null;
    }

    private Task GetPopupTask(SongModel model)
    {
        return Task.Run(async void () =>
        {
            try
            {
                if (ShowPopupAsync == null)
                {
                    return;
                }
                var models = new List<MoreOptionModel>
                {
                    new() { Title = MoreMenuConstants.AddToPlaylist, ResultItem = MoreMenuConstants.AddToPlaylist},
                    new() { Title = MoreMenuConstants.RemoveFromPlaylist, ResultItem = MoreMenuConstants.RemoveFromPlaylist},
                    new() { Title = MoreMenuConstants.Favourite,  ResultItem = MoreMenuConstants.Favourite},
                    new() { Title = MoreMenuConstants.AddToQueue, ResultItem = MoreMenuConstants.AddToQueue},
                    new() { Title = MoreMenuConstants.RemoveFromQueue, ResultItem = MoreMenuConstants.RemoveFromQueue},
                };

                ShowPopupAsync.Invoke(new MoreView( models, (o) => HandlePopup(o, model.Id)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    private async void AddToPlaylistCallback(object? playlistId, SongEntity song)
    {
        if (playlistId is not int id)
        {
            return;
        }

        var addToTestPlaylistEntity = await _playlistManager.GetPlaylistById(id);
        if (addToTestPlaylistEntity != null)
        {
            await _playlistManager.AddSong(addToTestPlaylistEntity, song);
        }
    }

    private async void RemoveFromPlaylistCallback(object? playlistId, SongEntity song)
    {
        if (playlistId is not int id)
        {
            return;
        }

        var addToTestPlaylistEntity = await _playlistManager.GetPlaylistById(id);
        if (addToTestPlaylistEntity != null)
        {
            await _playlistManager.RemoveSong(addToTestPlaylistEntity, song);
        }
    }

    private async void HandlePopup(object? result, int songId)
    {
        if (result is not string selectedOption)
        {
            return;
        }

        var queue = _queueManager.GetCurrentSongsQueue?.Invoke();

        var song = await _songsManager.GetSongFromId(songId);

        var songFromQueue = queue?.Songs?.FirstOrDefault(model => model.Id == songId).Map();

        if (song is null)
        {
            return;
        }

        var allPlaylists = await _playlistManager.GetAllPlaylists();
        allPlaylists.RemoveRange(0, 2);

        var playlistOptionModels = allPlaylists
            .Select((playlist) =>
                new MoreOptionModel()
                {
                    Title = playlist.Title,
                    ResultItem = playlist.Id
                }).ToList();

        switch (selectedOption)
        {
            case MoreMenuConstants.AddToPlaylist:
                ShowPopupAsync?.Invoke(new MoreView(
                    playlistOptionModels,
                    (playlistId) => AddToPlaylistCallback(playlistId, song)));

                break;
            case MoreMenuConstants.RemoveFromPlaylist:
                ShowPopupAsync?.Invoke(new MoreView(
                    playlistOptionModels,
                    (playlistId) => RemoveFromPlaylistCallback(playlistId, song)));
                break;
            case MoreMenuConstants.Favourite:
                var favouritePlaylist = await _playlistManager.GetPlaylistByTitle(PlaylistsConstants.FavoriteSongs);
                if (favouritePlaylist != null)
                {
                    var newFavouriteState = await _playlistManager.ToggleFavourite(favouritePlaylist, song);
                    song.IsFavorite = newFavouriteState;

                    await _songsManager.SaveSong(song);

                    var existingQueueSong = queue?.Songs?.FirstOrDefault((queuedSong) => song.Id == queuedSong.Id);

                    if (existingQueueSong != null)
                    {
                        existingQueueSong.IsFavorite = newFavouriteState;
                    }
                }
                break;
            case MoreMenuConstants.AddToQueue:
                _queueManager.AddSongToQueue(queue, song.Map());
                break;
            case MoreMenuConstants.RemoveFromQueue:
                _queueManager.RemoveSongFromQueue(queue, songFromQueue.Map());
                break;
        }
    }
}