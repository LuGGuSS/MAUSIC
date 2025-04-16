using MAUSIC.Data.Entities;
using MAUSIC.Models;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class PlaylistManager
{
    public PlaylistService _playlistService;

    public PlaylistManager(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    public async Task<PlaylistEntity> CreatePlaylist(string title)
    {
        var result = await _playlistService.CreatePlaylist(title);

        return result;
    }

    public async Task<PlaylistSongEntity> AddSong(PlaylistEntity playlistEntity, SongModel song)
    {
        var result = await _playlistService.AddSong(playlistEntity, song);

        return result;
    }

    public async Task<List<PlaylistSongEntity>> GetPlaylistSongs(PlaylistEntity playlistEntity)
    {
        var result = await _playlistService.GetPlaylistSongs(playlistEntity);

        return result;
    }
}