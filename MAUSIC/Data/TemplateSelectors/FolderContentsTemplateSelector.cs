using MAUSIC.Models;

namespace MAUSIC.Data.TemplateSelectors;

public class FolderContentsTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate SongTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is FolderModel)
        {
            return FolderTemplate;
        }

        return SongTemplate;
    }
}