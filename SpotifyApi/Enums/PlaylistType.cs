namespace JakubKastner.SpotifyApi.Enums;

[Flags]
public enum PlaylistType
{
	None = 0,
	Owned = 1 << 0,          // 1: my playlists
	Collaborative = 1 << 1,  // 2: colaborative
	Subscribed = 1 << 2,     // 4: only followed
	Editable = Owned | Collaborative,
	All = Owned | Collaborative | Subscribed,
}