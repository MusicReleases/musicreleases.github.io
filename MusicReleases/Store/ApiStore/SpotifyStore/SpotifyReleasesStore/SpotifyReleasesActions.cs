using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

// init
public record SpotifyReleasesActionInitialized();

// get releases
public class SpotifyReleasesActionLoad(ReleaseType _releaseType)
{
	public ReleaseType ReleaseType { get; } = _releaseType;
};

public record SpotifyReleasesActionLoadSuccess();
public record SpotifyReleasesActionLoadFailure(string ErrorMessage);

// set artists
public class SpotifyReleasesActionSet(ISet<SpotifyAlbum> _releases)
{
	public ISet<SpotifyAlbum> Releases { get; } = _releases;
};

// local storage -> set
public record SpotifyReleasesActionStorageSet(SpotifyReleasesState ReleasesState); // persists state
public record SpotifyReleasesActionStorageSetSuccess();
public record SpotifyReleasesActionStorageSetFailure(string ErrorMessage);

// local storage -> get
public record SpotifyReleasesActionStorageGet();
public record SpotifyReleasesActionStorageGetSuccess();
public record SpotifyReleasesActionStorageGetFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyReleasesActionStorageClear();
public record SpotifyReleasesActionStorageClearSuccess();
public record SpotifyReleasesActionStorageClearFailure(string ErrorMessage);