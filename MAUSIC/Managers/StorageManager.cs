using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MAUSIC.Data.Entities;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.Models.Abstract;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class StorageManager
{
    private readonly string[] _musicFileExtensions = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".wma", ".alac", ".aiff", ".opus"];

    private readonly StorageService _storageService;
    private readonly DatabaseManager _databaseManager;
    private readonly SongsManager _songsManager;
    private readonly RecommendationManager _recommendationManager;

    public StorageManager(
        StorageService storageService,
        DatabaseManager databaseManager,
        SongsManager songsManager,
        RecommendationManager recommendationManager)
    {
        _storageService = storageService;
        _databaseManager = databaseManager;
        _songsManager = songsManager;
        _recommendationManager = recommendationManager;
    }

    public async Task<List<string>?> PickFolder()
    {
        var result = await _storageService.PickFolder();

        if (result == null || !result.IsSuccessful)
        {
            return null;
        }

        await _databaseManager.SaveItemAsync(new FolderEntity { Path = result.Folder.Path });

        var files = await GetMusicFiles(result.Folder.Path);

        await SaveSongsToDatabase(files).ConfigureAwait(false);

        return files;
    }

    public async Task<List<string>> GetMusicFiles(string folderPath)
    {
        var files = await _storageService.GetFilesRecursive(folderPath);

        var result = new List<string>();

        foreach (var file in files)
        {
            string extension = Path.GetExtension(file).ToLower();
            if (Array.Exists(_musicFileExtensions, ext => ext == extension))
            {
                result.Add(file);
            }
        }

        result.Sort();

        return result;
    }

    public async Task<IList<BaseModel>> GetFolderContents(FolderModel folderModel, Func<SongModel,Task> openPopupFunc)
    {
        var folders = _storageService.GetInnerFolders(folderModel.Path);

        var modelsList = new List<BaseModel>();

        foreach (var folder in folders)
        {
            var model = new FolderModel()
            {
                Path = folder,
                Parent = folderModel
            };

            modelsList.Add(model);
        }

        var files = _storageService.GetInnerFiles(folderModel.Path);

        foreach (var file in files)
        {
            string extension = Path.GetExtension(file).ToLower();
            if (Array.Exists(_musicFileExtensions, ext => ext == extension))
            {
                var entity = await _songsManager.GetSongFromPath(file);

                var model = entity.Map();

                if (model != null)
                {
                    model.OpenPopupFunc = openPopupFunc;
                    modelsList.Add(model);
                }
            }
        }

        return modelsList;
    }

    public async Task PopulateFolderWithChildren(FolderModel parentModel, Func<SongModel,Task> opneePopupFunc)
    {
        var folders = _storageService.GetInnerFolders(parentModel.Path);

        foreach (var folder in folders)
        {
            var model = new FolderModel
            {
                Path = folder,
                Parent = parentModel
            };

            parentModel.InnerItems.Add(model);
        }

        var files = _storageService.GetInnerFiles(parentModel.Path);

        foreach (var file in files)
        {
            string extension = Path.GetExtension(file).ToLower();

            if (Array.Exists(_musicFileExtensions, ext => ext == extension))
            {
                var entity = await _songsManager.GetSongFromPath(file);

                var model = entity.Map();

                if (model != null)
                {
                    model.OpenPopupFunc = opneePopupFunc;
                    parentModel.InnerItems.Add(model);
                }
            }
        }
    }

    public async Task<List<FolderEntity>?> GetAllFolders()
    {
        var folders = await _databaseManager.GetAllItems<FolderEntity>();

        return folders;
    }

    private async Task SaveSongsToDatabase(List<string> songPaths)
    {
        var songs = new List<SongEntity>();

        foreach (var songPath in songPaths)
        {
            // NOTE: this call creates db model on
            var song = await _songsManager.GetSongFromPath(songPath);

            songs.Add(song);
        }

        await _recommendationManager.TryCreateRecommendationPairs(songs);
    }
}