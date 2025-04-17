using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.PageModels;

namespace MAUSIC.Pages;

public partial class QueuePage
{
    public QueuePage(QueuePageModel pageModel)
        : base(pageModel)
    {
        InitializeComponent();

        // NOTE: headers behaviour is broken on desktop, it uses screen width and not window one.
#if WINDOWS || MACCATALYST
ContentsCollectionView.Header = null;
#endif
    }
}