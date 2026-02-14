using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar
{

	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }


	[Parameter, EditorRequired]
	public required bool Loading { get; set; }


	private readonly MenuType _type = MenuType.Releases;


	private void AddToPlaylist()
	{

	}

	private void Play()
	{

	}
}
