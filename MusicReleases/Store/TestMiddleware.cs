using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public class TestMiddleware(IDispatcher dispatcher) : Middleware
{
	private readonly IDispatcher _dispatcher = dispatcher;

	public override void AfterDispatch(object action)
	{
		var actionType = action.GetType();
		if (actionType == typeof(SpotifyArtistsActionGetSuccess))
		{
			Console.WriteLine("aa");
		}
	}
}