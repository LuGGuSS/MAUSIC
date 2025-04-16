using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.PageModels;
using MAUSIC.Pages.Abstract;

namespace MAUSIC.Pages;

public partial class FoldersPage
{
    public FoldersPage(FoldersPageModel model)
        : base(model)
    {
        InitializeComponent();

        // NOTE: headers behaviour is broken on desktop, it uses screen width and not window one.
#if WINDOWS || MACCATALYST
ContentsCollectionView.Header = null;
#endif
    }

    private void RequestFilesButtonClicked(object? sender, EventArgs e)
    {
        _ = PageModel.RequestFiles();
    }
}