namespace CleanArchitectureTemplate.Application.Interfaces.Services;

public interface ICacheService
{
    Task<bool> HasKey(string key);
    Task<T> GetSession<T>(string key);
    Task SetSession<T>(string key, T value);
}
