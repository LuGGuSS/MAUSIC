using System.ComponentModel.DataAnnotations.Schema;
using MAUSIC.Data.Entities.Abstract;

namespace MAUSIC.Data.Entities;

public class PlaylistSongEntity : BaseEntity
{
    [ForeignKey(nameof(PlaylistEntity))]
    public int PlaylistId { get; set; }

    [ForeignKey(nameof(SongEntity))]
    public int SongId { get; set; }
}