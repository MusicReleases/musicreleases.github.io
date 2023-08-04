using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.MusicReleases;
using JakubKastner.SpotifyApi;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// client and user
builder.Services.AddScoped<SpotifyClient>();
builder.Services.AddScoped<SpotifyUser>();

// api controllers
builder.Services.AddScoped<ControllerApiArtist>();
builder.Services.AddScoped<ControllerApiPlaylist>();
builder.Services.AddScoped<ControllerApiRelease>();
builder.Services.AddScoped<ControllerApiTrack>();
builder.Services.AddScoped<ControllerApiUser>();

// controllers
builder.Services.AddScoped<SpotifyControllerArtist>();
builder.Services.AddScoped<SpotifyControllerPlaylist>();
builder.Services.AddScoped<SpotifyControllerRelease>();
builder.Services.AddScoped<SpotifyControllerTrack>();
builder.Services.AddScoped<SpotifyControllerUser>();

// local storage
builder.Services.AddBlazoredLocalStorage(config =>
{
	config.JsonSerializerOptions.WriteIndented = true;
});

// fluxor
builder.Services.AddFluxor(options =>
{
	options.ScanAssemblies(typeof(Program).Assembly);
#if DEBUG
	options.UseReduxDevTools();
#endif
});

await builder.Build().RunAsync();