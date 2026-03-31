/// <summary>
/// Resolves ItemData definitions by ID.
/// </summary>
public interface IItemDatabase
{
    ItemData Get(string id);
}