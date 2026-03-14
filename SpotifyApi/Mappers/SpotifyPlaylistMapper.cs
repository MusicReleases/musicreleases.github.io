using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Mappers;

internal static class SpotifyPlaylistMapper
{
	public static SpotifyPlaylist ToObject(this FullPlaylist api)
	{
		var id = api.Id.Require();
		var name = api.Name.Require();
		var uri = api.Uri.Require();
		var externalUrls = api.ExternalUrls.Require();
		var snapshotId = api.SnapshotId.Require();
		var owner = api.Owner.Require();
		var collaborative = api.Collaborative.Require();

		return new(id, name, uri, externalUrls[ApiConventions.ExternalUrlSpotifyKey], snapshotId, owner.Id, collaborative);
	}
}