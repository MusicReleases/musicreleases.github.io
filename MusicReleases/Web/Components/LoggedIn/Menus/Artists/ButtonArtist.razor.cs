using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class ButtonArtist
{
	[Parameter, EditorRequired]
	public required string ArtistId { get; set; }

	[Parameter, EditorRequired]
	public required string ArtistName { get; set; }
	private string? ButtonClass => ArtistFilter ? "active" : string.Empty;
	private bool ArtistFilter => SpotifyFilterService.Filter.Artist == ArtistId;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();
	}
	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task FilterArtist()
	{
		string? artistFilter = ArtistFilter ? null : ArtistId;

		var url = await SpotifyFilterUrlService.GetFilterUrl(artistFilter);
		NavManager.NavigateTo(url);
	}
}