using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;

namespace MAUSIC.Services;

public class SongsService
{
    private readonly DatabaseManager _databaseManager;

    private readonly SemaphoreSlim _semaphore = new(1);

    public SongsService(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task TrySaveSongs(IList<string> files)
    {
        var songsEntities = new List<SongEntity>();

        var dbCheckTasks = new List<Task>();

        foreach (var file in files)
        {
            var existingSong = await _databaseManager.GetItemAsync<SongEntity>((song) => song.Path == file);

            if (existingSong != null)
            {
                continue;
            }

            var task = Task.Run(() =>
            {
                try
                {
                    var song = ReadSongEntityFromPath(file);

                    _semaphore.Wait();
                    songsEntities.Add(song);
                    _semaphore.Release();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });

            dbCheckTasks.Add(task);
        }

        await Task.WhenAll(dbCheckTasks);

        await _databaseManager.SaveItemsAsync(songsEntities);
    }

    public async Task<List<SongModel>?> GetAllSongModels()
    {
        var allSongEntities = await _databaseManager.GetAllItems<SongEntity>();

        if (allSongEntities == null)
        {
            return null;
        }

        var songModels = new List<SongModel>();

        var initSongEntityTasks = new List<Task>();

        foreach (var song in allSongEntities)
        {
            var task = Task.Run(() =>
            {
                var model = song.Map();

                if (model == null)
                {
                    return;
                }

                _semaphore.Wait();
                songModels.Add(model);
                _semaphore.Release();
            });

            initSongEntityTasks.Add(task);
        }

        await Task.WhenAll(initSongEntityTasks);

        return songModels;
    }

    public async Task<SongEntity?> GetSongEntityFromPath(string path)
    {
        var result = await _databaseManager.GetItemAsync<SongEntity>((entity) => entity.Path == path);

        return result;
    }

    public async Task<SongEntity?> GetSongEntityFromId(int id)
    {
        var result = await _databaseManager.GetItemAsync<SongEntity>((entity) => entity.Id == id);

        return result;
    }

    public SongEntity ReadSongEntityFromPath(string path)
    {
        var tagsFile = TagLib.File.Create(path);

        var song = new SongEntity
        {
            Title = tagsFile.Tag.Title,
            Artist = tagsFile.Tag.FirstPerformer,
            Album = tagsFile.Tag.Album,
            Duration = tagsFile.Properties.Duration,
            Path = path
        };

        return song;
    }

}