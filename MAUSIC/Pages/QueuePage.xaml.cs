using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.PageModels;

namespace MAUSIC.Pages;

public partial class QueuePage
{
    public QueuePage(QueuePageModel vm)
        : base(vm)
    {
        InitializeComponent();
    }
}