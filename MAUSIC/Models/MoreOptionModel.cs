using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class MoreOptionModel : BaseModel
{
    public string Title { get; set; }

    public Action<string> CloseAction { get; set; }
}