using JakubKastner.Extensions;
using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonSearch
{
	[Parameter, EditorRequired]
	public MenuButtonsType Type { get; set; }

	[Parameter, EditorRequired]
	public EventCallback<bool> OnSearching { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	//private InputText? _inputTextSearchValue;
	private string? _searchValue;

	private string? _searchText;
	private bool _searching = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_searchText = "Search " + Type.ToString().ToLower();
	}

	private async Task SearchStart()
	{
		if (_searching)
		{
			Search();
			return;
		}
		await SearchInit();

	}
	private async Task SearchInit()
	{
		SearchingChange(true);

		// TODO search focus doesnt work because _inputTextSearchValue is null here
		/*if (_inputTextSearchValue?.Element is not null)
		{
			await _inputTextSearchValue.Element.Value.FocusAsync();
		}*/
	}

	private void Search()
	{
		if (_searchValue.IsNullOrEmpty())
		{
			_searchValue = string.Empty;
			SearchStop();
			return;
		}
		// TODO SearchValue
	}

	private void SearchStop()
	{
		SearchingChange(false);
	}

	private void SearchingChange(bool searching)
	{
		_searching = searching;
		OnSearching.InvokeAsync(!searching);
	}
}