using MAUSIC.Data.Entities.Abstract;

namespace MAUSIC.Data.Entities;

public class PlaylistSongEntity : BaseEntity
{
    public int PlaylistId { get; set; }

    public int SongId { get; set; }
}