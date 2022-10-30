namespace MusicReleases.Web.Objects;
public class LoaderService
{
    private bool _active = false;

    public string ActiveClass { get => _active ? " active" : string.Empty; }

    public event Action OnChange;

    public void Start()
    {
        _active = true;
        OnChange?.Invoke();
    }
    public void Stop()
    {
        _active = false;
        OnChange?.Invoke();
    }
}