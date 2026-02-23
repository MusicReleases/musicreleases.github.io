using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseList
{
	[Parameter, EditorRequired]
	public required ReleaseListComponent ListType { get; set; }

	[Parameter]
	public bool Hidden { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private string DivClass => $"release-list {ListType.ToLowerString()}{Hidden.ToCssClass()}";
}
