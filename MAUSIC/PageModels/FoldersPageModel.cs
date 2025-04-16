using CommunityToolkit.Mvvm.ComponentModel;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;

namespace MAUSIC.PageModels;

public partial class FoldersPageModel : BasePageModel
{
    private readonly StorageManager _storageManager;

    [ObservableProperty]
    private IList<FolderModel> _folders = new List<FolderModel>();

    public FoldersPageModel(StorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    protected override async Task InitializeAsync()
    {
        var folderEntities = await _storageManager.GetAllFolders();

        if (folderEntities == null || folderEntities.Count == 0)
        {
            return;
        }

        foreach (var folderModel in folderEntities.Select(entity => entity.Map()).ToList())
        {
            if (folderModel == null)
            {
                continue;
            }

            Folders.Add(folderModel);
        };
    }

    public async Task SelectFolder()
    {
        
    }
}