using Holidays.BlazorUI;
using Holidays.BlazorUI.ServiceCollectionExtensions;
using Holidays.BlazorUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddApplicationConfiguration(builder.Configuration)
    .AddEventBus()
    .AddPostgres()
    .AddInMemoryStore();

builder.Services.AddSingleton<OffersService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.Services.InitializeServices();

await app.RunAsync();
