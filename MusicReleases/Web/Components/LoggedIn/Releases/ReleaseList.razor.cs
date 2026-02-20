using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseList
{
	[Parameter, EditorRequired]
	public required ReleaseListComponent ListType { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private string DivClass => $"scroll release-list {ListType.ToLowerString()} {Class}";
}
