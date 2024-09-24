using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyTrack : SpotifyIdNameObject
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
	[SetsRequiredMembers]
	public SpotifyTrack(SimpleTrack simpleTrack)
	{
		Id = simpleTrack.Id;
		Name = simpleTrack.Name;
		// TODO empty??
		Album = null;
		_artists = [];
		//_artists = Controller.GetArtists(simpleTrack.Artists);
	}
	[SetsRequiredMembers]
	public SpotifyTrack(FullTrack fullTrack)
	{
		Id = fullTrack.Id;
		Name = fullTrack.Name;
		Album = new(fullTrack.Album, ReleaseType.Tracks);
		_artists = [];
		//_artists = Controller.GetArtists(fullTrack.Artists);
	}
	[SetsRequiredMembers]
	public SpotifyTrack(FullEpisode fullEpisode)
	{
		Id = fullEpisode.Id;
		Name = fullEpisode.Name;
		Album = new(fullEpisode.Show);
		_artists =
		[
			new(id: "0", name: "podcast")
		];
	}
}
