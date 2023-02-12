using Fluxor;

namespace JakubKastner.MusicReleases.Store.Releases;

public class LoaderReducers : Reducer<LoaderState, LoaderAction>
{
	public override LoaderState Reduce(LoaderState state, LoaderAction action)
	{
		return state with
		{
			Loading = action.Loading,
		};
	}
}
