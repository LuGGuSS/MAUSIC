using System.ComponentModel.DataAnnotations.Schema;
using MAUSIC.Data.Entities.Abstract;

namespace MAUSIC.Data.Entities;

public class RecommendationPairEntity : BaseEntity
{
    [ForeignKey(nameof(SongEntity))]
    public int FirstSongId { get; set; }

    [ForeignKey(nameof(SongEntity))]
    public int SecondSongId { get; set; }

    public float AlgorithmWeight { get; set; }

    public float UserWeight { get; set; }
}