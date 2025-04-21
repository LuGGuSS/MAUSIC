using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class MoreOptionModel : BaseModel
{
    public string Title { get; set; }

    public object? ResultItem { get; set; }

    public Action<object?> CloseAction { get; set; }
}