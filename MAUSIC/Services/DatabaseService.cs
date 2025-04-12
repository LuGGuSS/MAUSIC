using MAUSIC.Data;
using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities.Abstract;
using SQLite;

namespace MAUSIC.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        _database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
    }

    public async Task TryCreateTableAsync<TEntity>()
    {
        await _database.CreateTableAsync(typeof(TEntity));
    }

    public async Task<TEntity?> GetItemAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : BaseEntity, new()
    {
        try
        {
            // TODO: sqlite throws exception if i try to use FirstOrDefault() with external predicate in it
            // need to find a fix
            var result = await _database.Table<TEntity>().ToListAsync();
            return result.FirstOrDefault(predicate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<TEntity>> GetItemsAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : BaseEntity, new()
    {
        try
        {
            var result = await _database.Table<TEntity>().Where(item => predicate.Invoke(item)).ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<TEntity>();
        }
    }

    public async Task<List<TEntity>> GetAllItemsAsync<TEntity>() where TEntity : BaseEntity, new()
    {
        try
        {
            var result = await _database.Table<TEntity>().ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<TEntity>();
        }
    }

    public async Task<int> SaveItemAsync<TEntity>(TEntity item) where TEntity : BaseEntity, new()
    {
        try
        {
            if (item.Id == 0)
            {
                var result = await _database.InsertAsync(item, typeof(TEntity));

                return result;
            }
            else
            {
                var result = await _database.UpdateAsync(item, typeof(TEntity));

                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public async Task<int> SaveItemsAsync<TEntity>(IEnumerable<TEntity> items) where TEntity : BaseEntity, new()
    {
        try
        {
            var result = 0;

            foreach (var item in items)
            {
                result += await SaveItemAsync(item);
            }
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }
}