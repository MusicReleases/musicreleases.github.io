using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MusicReleases;
using Blazored.LocalStorage;
using MusicReleases.Api.Spotify.Objects;
using MusicReleases.Api.Spotify;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// spotify api
builder.Services.AddScoped<IUser, User>();
// TODO interface
builder.Services.AddScoped<Controller>();

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();