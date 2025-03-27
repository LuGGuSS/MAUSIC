using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUSIC.ViewModels;

namespace MAUSIC.Views;

public partial class PlayerView
{

    public PlayerView(PlayerViewModel vm)
        :base(vm)
    {
        InitializeComponent();
    }
}