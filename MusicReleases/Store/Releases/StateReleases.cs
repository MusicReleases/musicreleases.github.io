namespace JakubKastner.MusicReleases.Store.Releases;

public class StateReleases
{
	public bool Loaded { get; }

	public StateReleases(bool loaded)
	{
		Loaded = loaded;
	}
}
