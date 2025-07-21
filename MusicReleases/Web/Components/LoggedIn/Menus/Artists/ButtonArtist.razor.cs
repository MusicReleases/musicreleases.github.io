using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class ButtonArtist
{
	[Parameter, EditorRequired]
	public required string ArtistId { get; set; }

	[Parameter, EditorRequired]
	public required string ArtistName { get; set; }
	private string? ButtonClass => ArtistFilter ? "active" : string.Empty;
	private bool ArtistFilter => SpotifyFilterState.Value.Filter.Artist == ArtistId;

	private void FilterArtist()
	{
		string? artistFilter = ArtistFilter ? null : ArtistId;

		var url = SpotifyFilterUrlService.GetFilterUrl(artistFilter);
		NavManager.NavigateTo(url);
	}
}