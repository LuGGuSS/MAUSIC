using MAUSIC.Data.Entities.Abstract;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class DatabaseManager
{
    private readonly DatabaseService _databaseService;

    public DatabaseManager(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<TEntity?> GetItemAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.GetItemAsync(predicate);

        return result;
    }

    public async Task<List<TEntity>> GetItemsAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.GetItemsAsync(predicate);

        return result;
    }

    public async Task<List<TEntity>?> GetAllItems<TEntity>() where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.GetAllItemsAsync<TEntity>();

        return result;
    }

    public async Task<bool> SaveItemAsync<TEntity>(TEntity item) where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.SaveItemAsync(item);

        return result == 1;
    }

    public async Task<bool> DeleteItemAsync<TEntity>(TEntity item) where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.DeleteItemAsync(item);

        return result;
    }

    public async Task<int> SaveItemsAsync<TEntity>(IEnumerable<TEntity> items) where TEntity : BaseEntity, new()
    {
        await _databaseService.TryCreateTableAsync<TEntity>();

        var result = await _databaseService.SaveItemsAsync(items);

        return result;
    }
}