using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MAUSIC.PageModels.Abstract;

public abstract partial class BasePageModel : ObservableObject
{
    [RelayCommand]
    private async Task Initialize() => await InitializeAsync();

    protected abstract Task InitializeAsync();
}