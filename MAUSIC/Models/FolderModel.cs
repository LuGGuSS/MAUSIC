using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class FolderModel : BaseModel
{
    public int Id { get; set; }

    public string Path { get; set; }
}