﻿using Fluxor;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public class LoaderReducer : Reducer<LoaderState, LoaderAction>
{
	public override LoaderState Reduce(LoaderState state, LoaderAction action)
	{
		return state with
		{
			Loading = action.Loading,
		};
	}
}
