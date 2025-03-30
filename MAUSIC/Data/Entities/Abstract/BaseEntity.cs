using SQLite;

namespace MAUSIC.Data.Entities.Abstract;

public class BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
}