using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Mappers;

internal static class SpotifyReleaseMapper
{
	public static SpotifyRelease ToObject(this SimpleAlbum api, HashSet<SpotifyArtist> featuredArtists)
	{
		var releaseType = MapReleaseTypeFromApi(api.AlbumType);
		var releaseDate = api.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		var urlImage = GetImageUrl(api.Images);
		var artists = api.Artists.Select(simpleArtist => simpleArtist.ToObject()).ToHashSet();

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], releaseType, releaseDate, urlImage, api.TotalTracks, true, artists, featuredArtists);
	}

	public static SpotifyRelease ToObject(this FullAlbum api, HashSet<SpotifyArtist> featuredArtists)
	{
		var releaseType = MapReleaseTypeFromApi(api.AlbumType);
		var releaseDate = api.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		var urlImage = GetImageUrl(api.Images);
		var artists = api.Artists.Select(simpleArtist => simpleArtist.ToObject()).ToHashSet();

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], releaseType, releaseDate, urlImage, api.TotalTracks, true, artists, featuredArtists);
	}

	public static SpotifyRelease ToObject(this SimpleEpisode api, SpotifyArtist podcast)
	{
		var releaseType = ReleaseType.Podcast;
		var releaseDate = api.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		var urlImage = GetImageUrl(api.Images);
		var artists = new HashSet<SpotifyArtist>(1)
		{
			podcast,
		};

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], releaseType, releaseDate, urlImage, 1, true, artists, []);
	}
	public static SpotifyRelease ToObject(this FullEpisode api)
	{
		var releaseType = ReleaseType.Podcast;
		var releaseDate = api.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		var urlImage = GetImageUrl(api.Images);
		var artists = new HashSet<SpotifyArtist>(1)
		{
			api.Show.ToObject(),
		};

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], releaseType, releaseDate, urlImage, 1, true, artists, []);
	}

	private static ReleaseType MapReleaseTypeFromApi(string releaseTypeApi)
	{
		if (Namings.ReleaseTypeMap.TryGetValue(releaseTypeApi, out var result))
		{
			return result;
		}

		throw new ArgumentOutOfRangeException(nameof(releaseTypeApi), $"Not expected release type value: {releaseTypeApi}");
	}

	private static string GetImageUrl(List<Image> images)
	{
		if (images.Count < Namings.SmallImageIndex)
		{
			throw new ArgumentNullException(nameof(images));
		}

		if (images.Count >= Namings.MediumImageIndex)
		{
			return images[Namings.MediumImageIndex].Url;
		}

		return images[Namings.SmallImageIndex].Url;
	}
}
