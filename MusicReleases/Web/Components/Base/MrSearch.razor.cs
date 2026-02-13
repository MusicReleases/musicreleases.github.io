using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrSearch
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? SearchText { get; set; }

	[Parameter]
	public EventCallback<string?> ValueChanged { get; set; }

	[Parameter]
	public string Placeholder { get; set; } = "Search...";

	[Parameter]
	public string? Class { get; set; }


	private async Task TextValueChanged(string? newValue)
	{
		await UpdateValue(newValue);
	}

	private async Task Clear()
	{
		await UpdateValue(null);
	}

	private async Task UpdateValue(string? newValue)
	{
		SearchText = newValue;

		if (ValueChanged.HasDelegate)
		{
			await ValueChanged.InvokeAsync(SearchText);
		}
	}
}
