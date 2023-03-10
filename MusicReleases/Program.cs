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
builder.Services.AddScoped<Client>();
builder.Services.AddScoped<User>();

// api controllers
builder.Services.AddScoped<ControllerApiArtist>();
builder.Services.AddScoped<ControllerApiPlaylist>();
builder.Services.AddScoped<ControllerApiRelease>();
builder.Services.AddScoped<ControllerApiTrack>();
builder.Services.AddScoped<ControllerApiUser>();

// controllers
builder.Services.AddScoped<ControllerArtist>();
builder.Services.AddScoped<ControllerPlaylist>();
builder.Services.AddScoped<ControllerRelease>();
builder.Services.AddScoped<ControllerTrack>();
builder.Services.AddScoped<ControllerUser>();

// local storage
builder.Services.AddBlazoredLocalStorage();

// fluxor
builder.Services.AddFluxor(options =>
{
	options.ScanAssemblies(typeof(Program).Assembly);
#if DEBUG
	options.UseReduxDevTools();
#endif
});

await builder.Build().RunAsync();