namespace JakubKastner.SpotifyApi.Playlists;

[Flags]
public enum SpotifyPlaylistType
{
	None = 0,
	Owned = 1 << 0,          // 1: user playlists
	Collaborative = 1 << 1,  // 2: colaborative
	Subscribed = 1 << 2,     // 4: only followed
	Editable = Owned | Collaborative,
	All = Owned | Collaborative | Subscribed,
}