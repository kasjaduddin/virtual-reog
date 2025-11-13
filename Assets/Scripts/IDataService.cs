using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDataService
{
    Task AddAsync<T>(string path, T data);
    Task<T> GetAsync<T>(string path);
    Task<IEnumerable<T>> QueryAsync<T>(string path, QueryOptions options = null);
    Task UpdateAsync<T>(string path, T data);
    Task DeleteAsync(string path);
}