using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.PageModels;

namespace MAUSIC.Pages;

public partial class QueuePage
{
    public QueuePage(PlayerPageModel vm)
        : base(vm)
    {
        InitializeComponent();
    }

    private void RequestFilesButtonClicked(object? sender, EventArgs e)
    {
        if (PageModel.CurrentSongPath == null)
        {
            _ = PageModel.RequestFiles();
        }
    }
}