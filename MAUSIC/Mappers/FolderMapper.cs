using MAUSIC.Data.Entities;
using MAUSIC.Models;

namespace MAUSIC.Mappers;

public static class FolderMapper
{
    public static FolderEntity? Map(this FolderModel? model)
    {
        if (model == null)
        {
            return null;
        }

        var entity = new FolderEntity
        {
            Id = model.Id,
            Path = model.Path,
        };

        return entity;
    }

    public static FolderModel? Map(this FolderEntity? entity)
    {
        if (entity == null)
        {
            return null;
        }

        var model = new FolderModel
        {
            Id = entity.Id,
            Path = entity.Path
        };

        return model;
    }
}