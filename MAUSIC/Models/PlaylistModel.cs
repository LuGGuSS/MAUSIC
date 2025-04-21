using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class PlaylistModel : BaseModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public List<BaseModel> Songs { get; set; }
}