﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace JakubKastner.MusicReleases.Web.Components.InfiniteScrolling;

public partial class InfiniteScrolling<T>
{
	private List<T> _items = [];
	private ElementReference _lastItemIndicator;
	private DotNetObjectReference<InfiniteScrolling<T>>? _currentComponentReference;
	private IJSObjectReference? _module;
	private IJSObjectReference? _instance;
	private bool _enumerationCompleted;
	private CancellationTokenSource? _loadItemsCts;
	private InfiniteScrollingItemsProviderRequestDelegate<T>? _itemsProvider;

	private bool IsLoading => _loadItemsCts != null;

	[Parameter]
	public InfiniteScrollingItemsProviderRequestDelegate<T>? ItemsProvider { get; set; }

	[Parameter]
	public RenderFragment<T>? ItemTemplate { get; set; }

	[Parameter]
	public RenderFragment? LoadingTemplate { get; set; }

	/// <summary>
	/// Gets or sets the tag name of the HTML element that will be used as the virtualization spacer.
	/// One such element will be rendered before the visible items, and one more after them, using
	/// an explicit "height" style to control the scroll range.
	///
	/// The default value is "div". If you are placing the <see cref="InfiniteScrolling{T}"/> instance inside
	/// an element that requires a specific child tag name, consider setting that here. For example when
	/// rendering inside a "tbody", consider setting <see cref="LastItemIndicatorElement"/> to the value "tr".
	/// </summary>
	[Parameter]
	public string LastItemIndicatorElement { get; set; } = "div";

	[JSInvokable]
	public async Task LoadMoreItems()
	{
		if (_loadItemsCts != null)
			return;

		if (ItemsProvider == null)
			return;

		var items = _items;
		var cts = new CancellationTokenSource();
		_loadItemsCts = cts;
		try
		{
			StateHasChanged(); // Allow the UI to display the loading indicator
			try
			{
				var newItems = await ItemsProvider(new InfiniteScrollingItemsProviderRequest(items.Count, cts.Token));
				if (!cts.IsCancellationRequested)
				{
					var length = items.Count;
					items.AddRange(newItems);

					if (items.Count == length)
					{
						_enumerationCompleted = true;
					}
					else
					{
						System.Diagnostics.Debug.Assert(_instance != null);
						await _instance.InvokeVoidAsync("onNewItems");
					}
				}
			}
			catch (OperationCanceledException oce) when (oce.CancellationToken == cts.Token)
			{
				// No-op; we canceled the operation, so it's fine to suppress this exception.
			}
		}
		finally
		{
			_loadItemsCts = null;
			cts.Dispose();
		}
		// Display the new items and hide the loading indicator
		StateHasChanged();
	}

	public async Task RefreshDataAsync()
	{
		// Will be disposed by the LoadMoreData method
		_loadItemsCts?.Cancel();
		_loadItemsCts = null;

		_items = [];
		_enumerationCompleted = false;
		await LoadMoreItems();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		// Clear items when the provider changed
		if (ItemsProvider != _itemsProvider)
		{
			_items = [];
			_enumerationCompleted = false;
		}

		_itemsProvider = ItemsProvider;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		// Initialize the IntersectionObserver
		if (firstRender)
		{
			_module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/infinite-scrolling.js");
			_currentComponentReference = DotNetObjectReference.Create(this);
			_instance = await _module.InvokeAsync<IJSObjectReference>("initialize", _lastItemIndicator, _currentComponentReference);
		}
	}

	public async ValueTask DisposeAsync()
	{
		// Cancel the current load items operation
		if (_loadItemsCts != null)
		{
			_loadItemsCts.Dispose();
			_loadItemsCts = null;
		}

		// Stop the IntersectionObserver
		if (_instance != null)
		{
			await _instance.InvokeVoidAsync("dispose");
			await _instance.DisposeAsync();
			_instance = null;
		}

		if (_module != null)
		{
			await _module.DisposeAsync();
		}

		_currentComponentReference?.Dispose();
	}

	private void RenderLastElement(RenderTreeBuilder builder)
	{
		// This is the last element, so let's use a huge value
		builder.OpenElement(1000, LastItemIndicatorElement);
		var style = _enumerationCompleted ? "height:0;width:0" : "height:1px;width:1px;flex-shrink:0";
		builder.AddAttribute(1001, "style", style);
		builder.AddElementReferenceCapture(1002, element => _lastItemIndicator = element);
		builder.CloseElement();
	}
}
