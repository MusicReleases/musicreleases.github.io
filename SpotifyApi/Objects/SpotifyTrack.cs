using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyTrack : SpotifyIdNameObject, IComparable<SpotifyTrack>
{
	public int TrackNumber { get; init; }
	public int DiscNumber { get; init; }
	public TimeSpan Duration { get; init; }

	public bool Explicit { get; init; }

	// TODO album
	//public SpotifyRelease? Album { get; private set; }
	public HashSet<SpotifyArtist> Artists { get; init; }

	public string DurationString => string.Format("{0:D2}:{1:D2}", (int)Duration.TotalMinutes, Duration.Seconds);

	// TODO artists - GetArtists
	[SetsRequiredMembers]
	public SpotifyTrack(SimpleTrack simpleTrack, SpotifyRelease release)
	{
		Id = simpleTrack.Id;
		Name = simpleTrack.Name;
		TrackNumber = simpleTrack.TrackNumber;
		DiscNumber = simpleTrack.DiscNumber;
		Duration = TimeSpan.FromMilliseconds(simpleTrack.DurationMs);
		Explicit = simpleTrack.Explicit;
		// TODO empty??
		//Album = null;
		Artists = [];
		foreach (var simpleArtist in simpleTrack.Artists)
		{
			if (release.ReleaseType != SpotifyEnums.ReleaseType.Appears && release.Artists.Any(x => x.Id == simpleArtist.Id))
			{
				// skip artists from album (no for appears)
				continue;
			}

			var artist = new SpotifyArtist(simpleArtist.Id, simpleArtist.Name);
			Artists.Add(artist);
		}
	}

	[SetsRequiredMembers]
	public SpotifyTrack(FullTrack fullTrack)
	{
		Id = fullTrack.Id;
		Name = fullTrack.Name;
		TrackNumber = fullTrack.TrackNumber;
		DiscNumber = fullTrack.DiscNumber;
		Duration = TimeSpan.FromMilliseconds(fullTrack.DurationMs);
		Explicit = fullTrack.Explicit;
		//Album = new(fullTrack.Album, ReleaseType.Tracks);
		Artists = [];
		foreach (var simpleArtist in fullTrack.Artists)
		{
			var artist = new SpotifyArtist(simpleArtist.Id, simpleArtist.Name);
			Artists.Add(artist);
		}
	}

	[SetsRequiredMembers]
	public SpotifyTrack(FullEpisode fullEpisode)
	{
		Id = fullEpisode.Id;
		Name = fullEpisode.Name;
		TrackNumber = 0; // episode doesnt have track number
		DiscNumber = 0; // episode doesnt have disc number
		Duration = TimeSpan.FromMilliseconds(fullEpisode.DurationMs);
		Explicit = fullEpisode.Explicit;
		//Album = new(fullEpisode.Show);
		Artists =
		[
			new(id: "0", name: "podcast")
		];
	}

	public int CompareTo(SpotifyTrack? other)
	{
		if (other is null)
		{
			return 1;
		}

		var discCompare = DiscNumber.CompareTo(other.DiscNumber);
		if (discCompare != 0)
		{
			return discCompare;
		}

		return TrackNumber.CompareTo(other.TrackNumber);
	}
}
