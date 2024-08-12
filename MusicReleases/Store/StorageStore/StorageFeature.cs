using Fluxor;

namespace JakubKastner.MusicReleases.Store.StorageStore;

public class StorageFeature : Feature<StorageState>
{
	public override string GetName()
	{
		return "Storage";
	}

	protected override StorageState GetInitialState()
	{
		return new()
		{
			//Loading = false,
		};
	}
}
