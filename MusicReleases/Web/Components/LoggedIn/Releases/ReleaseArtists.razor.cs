using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseArtists : IDisposable
{
	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;

	[Inject]
	private ISpotifyArtistState SpotifyArtistState { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required HashSet<SpotifyArtist> Artists { get; set; }

	[Parameter]
	public string? ButtonClass { get; set; }

	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;
		SpotifyArtistState.OnChange += StateChanged;
		SpotifyReleaseFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SettingsService.OnChange -= StateChanged;
		SpotifyArtistState.OnChange -= StateChanged;
		SpotifyReleaseFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private bool IsArtistFollowed(string artistId)
	{
		return SpotifyArtistState.IsFollowed(artistId);
	}

	public string GetButtonClass(string artistId)
	{
		var followed = IsArtistFollowed(artistId);
		return $"artist {ButtonClass} {followed.ToCssClass(null, "not-followed")}";
	}

	private string? GetButtonUrl(SpotifyArtist artist)
	{
		if (IsArtistFollowed(artist.Id))
		{
			return null;
		}
		return SettingsService.GetUrl(artist);
	}

	private string? GetButtonTitle(SpotifyArtist artist)
	{
		if (IsArtistFollowed(artist.Id))
		{
			return SpotifyReleaseFilterService.IsArtistFiltered(artist.Id) ? $"Remove artist '{artist.Name}' filter" : $"Filter artist '{artist.Name}'";
		}
		return SettingsService.GetUrlTitle($"artist '{artist.Name}'");
	}

	private bool GetButtonNewTab(SpotifyArtist artist)
	{
		if (IsArtistFollowed(artist.Id))
		{
			return false;
		}
		return !SettingsService.UserSettings.OpenLinksInApp;
	}

	private void FilterArtist(string artistId)
	{
		if (!IsArtistFollowed(artistId))
		{
			return;
		}

		SpotifyReleaseFilterService.FilterArtist(artistId);
	}
}
