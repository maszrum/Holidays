using Holidays.Eventing;
using Holidays.InMemoryStore;
using Holidays.WebAPI.Eventing;
using Holidays.WebAPI.Json;
using Holidays.WebAPI.Mapping;
using Holidays.WebAPI.ServiceCollectionExtensions;
using Holidays.WebAPI.Services;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAutoMapper(options => options.AddProfile(new AutoMapperProfile()))
    .AddTransient(typeof(MaybeConverter<,>), typeof(MaybeConverter<,>));

builder.Services
    .AddApplicationConfiguration(builder.Configuration)
    .AddEventBus()
    .AddPostgres()
    .AddInMemoryStore();

builder.Services
    .AddScoped<OffersService>()
    .AddScoped<PriceHistoryService>();

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapGet(
    "/api/offers",
    (OffersService service) => service.GetOffers());

app.MapGet(
    "/api/price-history/{id:guid}",
    async (Guid id, PriceHistoryService service) => (await service.GetPriceHistory(id))
        .Match(
            () => Results.NotFound(),
            Results.Ok));

app.MapHub<EventsHub>("/api/events");

await app.Services.GetRequiredService<IEventBus>().Initialize();
await app.Services.GetRequiredService<InMemoryDatabase>().Initialize();

await app.RunAsync();
