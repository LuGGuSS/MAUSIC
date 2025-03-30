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
            Album = model.Album,
            Artist = model.Artist,
            Duration = model.Duration,
            Path = model.Path,
            Title = model.Title
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
            Album = entity.Album,
            Artist = entity.Artist,
            Duration = entity.Duration,
            Path = entity.Path,
            Title = entity.Title
        };

        return model;
    }
}