using AutoMapper;
using Holidays.DataTransferObjects;
using Holidays.InMemoryStore;

namespace Holidays.WebAPI.Services;

internal class OffersService
{
    private readonly InMemoryDatabase _database;
    private readonly IMapper _mapper;

    public OffersService(
        InMemoryDatabase database,
        IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OfferDto>> GetOffers()
    {
        var repository = new OffersInMemoryRepository(_database);
        var offers = await repository.GetAll();

        var dtos = _mapper.Map<OfferDto[]>(offers.Elements);

        return dtos;
    }
}
