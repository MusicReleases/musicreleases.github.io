using Blazored.LocalStorage;
using JakubKastner.MusicReleases;
using JakubKastner.MusicReleases.Web.Objects;
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

// testing
builder.Services.AddScoped<Client>();
builder.Services.AddScoped<User>();
builder.Services.AddScoped<ControllerApiUser>();
builder.Services.AddScoped<ControllerApiPlaylist>();
builder.Services.AddScoped<ControllerUser>();
builder.Services.AddScoped<ControllerPlaylist>();

// TODO interface
builder.Services.AddScoped<Controller>();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<LoaderService>();

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();