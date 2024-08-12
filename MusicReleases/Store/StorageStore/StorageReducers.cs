using Fluxor;

namespace JakubKastner.MusicReleases.Store.StorageStore;

public class StorageReducers : Reducer<StorageState, StorageActions>
{
	public override StorageState Reduce(StorageState state, StorageActions action)
	{
		return state with
		{

			//Loading = action.Loading,
		};
	}
}
