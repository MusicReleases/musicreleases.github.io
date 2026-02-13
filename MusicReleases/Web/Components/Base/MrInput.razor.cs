using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrInput
{
	[Parameter]
	public string? Value { get; set; }

	[Parameter]
	public EventCallback<string?> ValueChanged { get; set; }

	[Parameter]
	public string? Placeholder { get; set; }

	[Parameter]
	public string? Class { get; set; }

	private async Task HandleInput(ChangeEventArgs e)
	{
		Value = e.Value?.ToString() ?? null;

		if (ValueChanged.HasDelegate)
		{
			await ValueChanged.InvokeAsync(Value);
		}
	}
}
