using MAUSIC.Data.Entities.Abstract;

namespace MAUSIC.Data.Entities;

public class SongEntity : BaseEntity
{
    public string Title { get; set; }

    public string Artist { get; set; }

    public string Album { get; set; }

    public TimeSpan Duration { get; set; }

    public string Path { get; set; }

    public bool IsFavorite { get; set; }

    public string Genres { get; set; }

    public string Performers { get; set; }

    public uint BPM { get; set; }
}