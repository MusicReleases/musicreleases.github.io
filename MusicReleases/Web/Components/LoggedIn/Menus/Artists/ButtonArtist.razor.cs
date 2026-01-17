using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class ButtonArtist
{
	[Parameter, EditorRequired]
	public required string ArtistId { get; set; }

	[Parameter, EditorRequired]
	public required string ArtistName { get; set; }
	private string ButtonClass => $"rounded-m transparent{(ArtistFilter ? " active" : string.Empty)}";
	private bool ArtistFilter => SpotifyFilterService.Filter?.Artist == ArtistId;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
	}
	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		GC.SuppressFinalize(this);
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task FilterArtist()
	{
		var artistFilter = ArtistFilter ? null : ArtistId;

		var url = await SpotifyFilterUrlService.GetFilterUrl(artistFilter);
		NavManager.NavigateTo(url);
	}
}