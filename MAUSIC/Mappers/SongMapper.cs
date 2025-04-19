using MAUSIC.Data.Entities;
using MAUSIC.Models;

namespace MAUSIC.Mappers;

public static class SongMapper
{
    public static SongEntity? Map(this SongModel? model)
    {
        if (model == null)
        {
            return null;
        }

        var entity = new SongEntity
        {
            Id = model.Id,
            Album = model.Album,
            Artist = model.Artist,
            Duration = model.Duration,
            Path = model.Path,
            Title = model.Title,
            IsFavorite = model.IsFavorite,
        };

        return entity;
    }

    public static SongModel? Map(this SongEntity? entity)
    {
        if (entity == null)
        {
            return null;
        }

        var model = new SongModel
        {
            Id = entity.Id,
            Album = entity.Album,
            Artist = entity.Artist,
            Duration = entity.Duration,
            Path = entity.Path,
            Title = entity.Title,
            IsFavorite = entity.IsFavorite,
            CoverImage = TryGetSongCover(entity.Path)
        };

        return model;
    }

    private static ImageSource TryGetSongCover(string songPath)
    {
        var tags = TagLib.File.Create(songPath);

        // TODO: implement downsampling
        var result = tags.Tag.Pictures.Length > 0
            ? ImageSource.FromStream(() => new MemoryStream(tags.Tag.Pictures[0].Data.Data))
            : ImageSource.FromFile("album_100dp.png");

        return result;
    }
}