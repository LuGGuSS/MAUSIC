using MAUSIC.Models;

namespace MAUSIC.Data.TemplateSelectors;

public class PlaylistContentsTemplateSelector : DataTemplateSelector
{
    public DataTemplate PlaylistTemplate { get; set; }
    public DataTemplate SongTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is PlaylistModel)
        {
            return PlaylistTemplate;
        }

        return SongTemplate;
    }
}