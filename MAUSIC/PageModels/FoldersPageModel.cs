using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;
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

    private readonly FolderModel _initialModel = new ();

    [ObservableProperty] private FolderModel _selectedFolderModel;

    [ObservableProperty] private bool _shouldShowFooter;

    public FoldersPageModel(
        StorageManager storageManager,
        QueueManager queueManager,
        SongsManager songsManager)
    {
        _storageManager = storageManager;
        _queueManager = queueManager;
        _songsManager = songsManager;
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

            var models = await _storageManager.GetFolderContents(folderModel);

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
            await _storageManager.PopulateFolderWithChildren(model);
        }

        SelectedFolderModel = model;
        UpdateShouldShowFooter();
    }

    [RelayCommand]
    public async Task SongSelected(SongModel model)
    {
        var songs = SelectedFolderModel.InnerItems.OfType<SongModel>().ToList();

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
}