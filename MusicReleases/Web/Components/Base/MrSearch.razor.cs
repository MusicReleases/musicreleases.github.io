using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrSearch
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string SearchText { get; set; } = string.Empty;

	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }

	[Parameter]
	public string Placeholder { get; set; } = "Search...";

	[Parameter]
	public string? Class { get; set; }


	private async Task HandleInput(ChangeEventArgs e)
	{
		SearchText = e.Value?.ToString() ?? string.Empty;
		await ValueChanged.InvokeAsync(SearchText);
	}

	private async Task Clear()
	{
		SearchText = string.Empty;
		await ValueChanged.InvokeAsync(SearchText);
	}
}
