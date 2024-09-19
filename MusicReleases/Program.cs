using Blazored.LocalStorage;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using IndexedDB.Blazor;
using JakubKastner.MusicReleases;
using JakubKastner.SpotifyApi;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// spotify api
builder.Services.AddSpotifyApi();

// music releases
builder.Services.AddMusicReleases();

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

builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

//builder.Services.AddMudServices();

await builder.Build().RunAsync();