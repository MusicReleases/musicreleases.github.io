using Blazored.LocalStorage;
using JakubKastner.MusicReleases;
using JakubKastner.MusicReleases.Database;
using JakubKastner.SpotifyApi;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// spotify api
builder.Services.AddSpotifyApi();

// spotify api client id from settings
var clientId = builder.Configuration["Spotify:ClientId"];
if (string.IsNullOrEmpty(clientId))
{
	throw new InvalidOperationException("Spotify ClientId is not configured. Please set 'Spotify:ClientId' in appsettings.json or environment variables.");
}
builder.Services.AddSingleton(new SpotifyConfig(clientId));

// music releases
builder.Services.AddMusicReleases();

// local storage
builder.Services.AddBlazoredLocalStorage(config =>
{
	config.JsonSerializerOptions.WriteIndented = true;
});

// indexed db
builder.Services.AddIndexedDbService();
builder.Services.AddIndexedDb(SpotifyReleasesDb.Name, SpotifyReleasesDb.GetAllTables(), SpotifyReleasesDb.Version);

await builder.Build().RunAsync();