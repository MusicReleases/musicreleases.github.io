using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyTrack : SpotifyIdObject
{
	public SpotifyRelease? Album { get; private set; }

	public HashSet<SpotifyArtist> Artists { get { return _artists; } }
	private readonly HashSet<SpotifyArtist> _artists;

	public string ArtistsString
	{
		get
		{
			return string.Join(" • ", _artists.Select(x => x.Name));
		}
	}

	// TODO artists - GetArtists
	public SpotifyTrack(SimpleTrack simpleTrack) : base(simpleTrack.Id, simpleTrack.Name)
	{
		// TODO empty??
		Album = null;
		_artists = [];
		//_artists = Controller.GetArtists(simpleTrack.Artists);
	}
	public SpotifyTrack(FullTrack fullTrack) : base(fullTrack.Id, fullTrack.Name)
	{
		Album = new(fullTrack.Album, ReleaseType.Tracks);
		_artists = [];
		//_artists = Controller.GetArtists(fullTrack.Artists);
	}
	public SpotifyTrack(FullEpisode fullEpisode) : base(fullEpisode.Id, fullEpisode.Name)
	{
		Album = new(fullEpisode.Show);
		_artists =
		[
			new(id: "0", name: "podcast")
		];
	}
}
