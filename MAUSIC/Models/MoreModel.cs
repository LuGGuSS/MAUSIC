using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;

namespace MAUSIC.Models;

public partial class MoreModel
{
    public MoreModel(List<MoreOptionModel> models)
    {
        MoreOptions = models;
    }


    public List<MoreOptionModel> MoreOptions { get; set; }
}