using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyTrack : SpotifyIdNameUrlObject, IComparable<SpotifyTrack>
{
	public int TrackNumber { get; init; }

	public int DiscNumber { get; init; }

	public TimeSpan Duration { get; init; }

	public bool Explicit { get; init; }

	public required string ReleaseId { get; init; }

	public HashSet<SpotifyArtist>? Artists { get; init; }

	public string DurationString => string.Format("{0:D2}:{1:D2}", (int)Duration.TotalMinutes, Duration.Seconds);

	public SpotifyTrack()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyTrack(string id, string name, string urlApp, string urlWeb, string releaseId, int trackNumber, int discNumber, TimeSpan duration, bool explicitLyrics) : base(id, name, urlApp, urlWeb)
	{
		ReleaseId = releaseId;
		TrackNumber = trackNumber;
		DiscNumber = discNumber;
		Duration = duration;
		Explicit = explicitLyrics;

		// TODO artists
	}

	[SetsRequiredMembers]
	public SpotifyTrack(string id, string name, string urlApp, string urlWeb, string releaseId, int trackNumber, int discNumber, TimeSpan duration, bool explicitLyrics, HashSet<SpotifyArtist> artists) : base(id, name, urlApp, urlWeb)
	{
		ReleaseId = releaseId;
		TrackNumber = trackNumber;
		DiscNumber = discNumber;
		Duration = duration;
		Explicit = explicitLyrics;
		Artists = artists;
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
