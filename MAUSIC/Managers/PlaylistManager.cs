using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class PlaylistManager
{
    private readonly PlaylistService _playlistService;

    private PlaylistEntity? _allSongsPlaylistEntity;

    public PlaylistManager(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    public async Task<List<PlaylistEntity>> GetAllPlaylists()
    {
        var result = await _playlistService.GetAllPlaylists();

        if (result == null || result.Count == 0)
        {
            result = await CreateBasePlaylists();
        }

        return result;
    }

    public async Task<PlaylistEntity?> GetPlaylistByTitle(string title)
    {
        var result = await _playlistService.GetPlaylistByTitle(title);

        return result;
    }

    public async Task<PlaylistEntity> CreatePlaylist(string title)
    {
        var result = await _playlistService.CreatePlaylist(title);

        return result;
    }

    public async Task<PlaylistSongEntity> AddSong(PlaylistEntity playlistEntity, SongEntity song)
    {
        var playlistSongEntities = await GetPlaylistSongs(playlistEntity.Id);

        var playlistSongEntity = playlistSongEntities.FirstOrDefault((playlistSong) => playlistSong.SongId == song.Id);

        if (playlistSongEntity != null)
        {
            return playlistSongEntity;
        }

        var result = await _playlistService.AddSong(playlistEntity, song);

        return result;
    }

    public async Task<bool> RemoveSong(PlaylistEntity playlistEntity, SongEntity song)
    {
        var playlistSongEntities = await GetPlaylistSongs(playlistEntity.Id);

        var playlistSongEntity = playlistSongEntities.FirstOrDefault((playlistSong) => playlistSong.SongId == song.Id);

        if (playlistSongEntity == null)
        {
            return false;
        }

        var result = await _playlistService.RemoveSong(playlistSongEntity);

        return result;
    }

    public async Task<bool> ToggleFavourite(PlaylistEntity playlistEntity, SongEntity song)
    {
        var result = await _playlistService.ToggleFavourite(playlistEntity, song);

        return result;
    }

    public async Task<PlaylistSongEntity> AddSongToAllSongsPlaylist(SongEntity song)
    {
        if (_allSongsPlaylistEntity == null)
        {
            var playlist = await GetPlaylistByTitle(PlaylistsConstants.AllSongs);

            if (playlist == null)
            {
                var playlists = await CreateBasePlaylists();
                playlist = playlists[0];
            }

            _allSongsPlaylistEntity = playlist;
        }

        var result = await AddSong(_allSongsPlaylistEntity, song);

        return result;
    }

    public async Task<List<PlaylistSongEntity>> GetPlaylistSongs(int playlistId)
    {
        var resultPlaylistSongs = await _playlistService.GetPlaylistSongs(playlistId);

        return resultPlaylistSongs;
    }

    private async Task<List<PlaylistEntity>> CreateBasePlaylists()
    {
        var result = new List<PlaylistEntity>();

        result.Add(await CreatePlaylist(PlaylistsConstants.AllSongs));
        result.Add(await CreatePlaylist(PlaylistsConstants.FavoriteSongs));
        result.Add(await CreatePlaylist(PlaylistsConstants.TestPlaylist));

        return result;
    }
}