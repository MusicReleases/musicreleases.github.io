using Blazored.LocalStorage;
using JakubKastner.SpotifyApi;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MusicReleases;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// spotify api
builder.Services.AddScoped<IUser, User>();
// TODO interface
builder.Services.AddScoped<Controller>();
builder.Services.AddScoped<Login>();

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();