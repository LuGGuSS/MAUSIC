using MAUSIC.Data.Entities;
using MAUSIC.Models;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Mappers;

public static class PlaylistMapper
{
    public static PlaylistModel? Map(this PlaylistEntity? entity)
    {
        if (entity == null)
        {
            return null;
        }

        var playlistModel = new PlaylistModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Songs = new List<BaseModel>()
        };

        return playlistModel;
    }

    public static PlaylistEntity? Map(this PlaylistModel? playlistModel)
    {
        if (playlistModel == null)
        {
            return null;
        }

        var playlistEntity = new PlaylistEntity
        {
            Id = playlistModel.Id,
            Title = playlistModel.Title
        };

        return playlistEntity;
    }
}