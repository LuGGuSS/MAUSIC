using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MAUSIC.ViewModels.Abstract;

public abstract partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private async Task Initialize() => await InitializeAsync();

    protected abstract Task InitializeAsync();
}