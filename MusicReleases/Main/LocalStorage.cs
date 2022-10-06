using Blazored.LocalStorage;

namespace MusicReleases.Main;

public class LocalStorage
{
    private readonly ILocalStorageService _localStorage;

    public LocalStorage(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task Save(string key, string data)
    {
        await _localStorage.SetItemAsync(key, data);
    }
}
