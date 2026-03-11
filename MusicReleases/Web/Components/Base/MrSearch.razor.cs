using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrSearch : IDisposable
{
	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public string ClearButtonClass { get; set; } = "search";

	[Parameter]
	public string? SearchText { get; set; }

	[Parameter]
	public EventCallback<string?> ValueChanged { get; set; }

	[Parameter]
	public string Placeholder { get; set; } = "Search...";


	private string InputClass => $"search {Class}";


	private CancellationTokenSource? _cts;


	public void Dispose()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		GC.SuppressFinalize(this);
	}

	private async Task TextValueChanged(string? newValue)
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = new CancellationTokenSource();

		try
		{
			// wait 400 ms
			await Task.Delay(400, _cts.Token);

			await UpdateValue(newValue);
		}
		catch (TaskCanceledException)
		{
			// user is still writing
		}
	}


	private async Task Clear()
	{
		_cts?.Cancel();
		await UpdateValue(null);
	}

	private async Task UpdateValue(string? newValue)
	{
		//SearchText = newValue;

		if (ValueChanged.HasDelegate)
		{
			await ValueChanged.InvokeAsync(newValue);
		}
	}
}
