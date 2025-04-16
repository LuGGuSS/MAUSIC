using System.Collections.ObjectModel;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class FolderModel : BaseModel
{
    public int Id { get; set; }

    public string Path { get; set; }

    public string Title => Path.Split("/").Last();

    public ObservableCollection<BaseModel> InnerItems { get; set; } = new();

    public FolderModel? Parent { get; set; }
}