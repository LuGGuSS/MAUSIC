using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;

namespace MAUSIC.Services;

public class StorageService
{
    private readonly IFolderPicker _folderPicker;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    public StorageService(IFolderPicker folderPicker)
    {
        _folderPicker = folderPicker;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public async Task<FolderPickerResult?> PickFolder()
    {
        try
        {
            var result = await _folderPicker.PickAsync(_cancellationTokenSource.Token);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<string>> GetFilesRecursive(string folderPath)
    {
        try
        {
            var files = Directory.GetFiles(folderPath).ToList();

            var folders = Directory.GetDirectories(folderPath).ToList();

            if (folders.Count > 0)
            {
                foreach (var folder in folders)
                {
                    files.AddRange(await GetFilesRecursive(folder));
                }
            }

            files.Sort();

            return files;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<string> GetInnerFolders(string folderPath)
    {
        var folders = Directory.GetDirectories(folderPath).ToList();

        folders.Sort();

        return folders;
    }

    public List<string> GetInnerFiles(string folderPath)
    {
        var files = Directory.GetFiles(folderPath).ToList();

        files.Sort();

        return files;
    }
}