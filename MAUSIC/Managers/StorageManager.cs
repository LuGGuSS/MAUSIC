using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class StorageManager
{
    private readonly string[] _musicFileExtensions = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".wma", ".alac", ".aiff", ".opus"];

    private readonly StorageService _storageService;

    public StorageManager(StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<List<string>?> PickFolder()
    {
        var result = await _storageService.PickFolder();

        if (result == null || !result.IsSuccessful)
        {
            return null;
        }

        var files = await GetMusicFiles(result.Folder.Path);
        return files;
    }

    public async Task<List<string>> GetMusicFiles(string folderPath)
    {
        var files = await _storageService.GetFiles(folderPath);

        var result = new List<string>();

        foreach (var file in files)
        {
            string extension = Path.GetExtension(file).ToLower();
            if (Array.Exists(_musicFileExtensions, ext => ext == extension))
            {
                result.Add(file);
            }
        }

        result.Sort();

        return result;
    }
}