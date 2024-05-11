using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : SpotifyIdObject
{
	public string ReleaseDate { get; private set; }
	public int TotalTracks { get; private set; }

	public string UrlApp { get; private set; }
	public string UrlWeb { get; private set; }
	public string UrlImage { get; private set; }

	public List<Image> Images { get; private set; }

	public HashSet<SpotifyArtist> Artists { get; private set; }

	public SortedSet<SpotifyTrack>? Tracks { get; private set; }

	public ReleaseType ReleaseType { get; private set; }

	// TODO artists - GetArtists
	// TODO images (0), default

	public SpotifyRelease(SimpleAlbum simpleAlbum, ReleaseType releaseType) : base(simpleAlbum.Id, simpleAlbum.Name)
	{
		ReleaseDate = simpleAlbum.ReleaseDate;
		TotalTracks = simpleAlbum.TotalTracks;
		Images = simpleAlbum.Images;
		if (simpleAlbum.Images.Count > 0)
		{
			UrlImage = simpleAlbum.Images.First().Url;
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = simpleAlbum.Uri;
		UrlWeb = simpleAlbum.Href;
		Artists = simpleAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
	}

	public SpotifyRelease(FullAlbum fullAlbum, ReleaseType releaseType) : base(fullAlbum.Id, fullAlbum.Name)
	{
		ReleaseDate = fullAlbum.ReleaseDate;
		TotalTracks = fullAlbum.TotalTracks;
		Images = fullAlbum.Images;
		if (fullAlbum.Images.Count > 0)
		{
			UrlImage = fullAlbum.Images.First().Url;
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = fullAlbum.Uri;
		UrlWeb = fullAlbum.Href;
		Artists = fullAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
	}

	public SpotifyRelease(SimpleShow simpleShow) : base(simpleShow.Id, simpleShow.Name)
	{
		ReleaseDate = "0";
		TotalTracks = 1;
		Images = simpleShow.Images;
		if (simpleShow.Images.Count > 0)
		{
			UrlImage = simpleShow.Images.First().Url;
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = simpleShow.Uri;
		UrlWeb = simpleShow.Href;
		Artists =
		[
			new(id: "0", name: simpleShow.Publisher)
		];
		ReleaseType = ReleaseType.Podcasts;
	}
}
