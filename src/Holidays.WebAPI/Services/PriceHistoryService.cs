using AutoMapper;
using Holidays.Core.Monads;
using Holidays.DataTransferObjects;
using Holidays.Postgres;

namespace Holidays.WebAPI.Services;

internal class PriceHistoryService
{
    private readonly PostgresConnectionFactory _postgresConnectionFactory;
    private readonly IMapper _mapper;

    public PriceHistoryService(
        PostgresConnectionFactory postgresConnectionFactory,
        IMapper mapper)
    {
        _postgresConnectionFactory = postgresConnectionFactory;
        _mapper = mapper;
    }

    public async Task<Maybe<PriceHistoryDto>> GetPriceHistory(Guid id)
    {
        await using var postgresConnection = await _postgresConnectionFactory.CreateConnection();
        await using var priceHistoryRepository = new PriceHistoryPostgresRepository(postgresConnection);

        var priceHistory = await priceHistoryRepository.Get(id);

        var dto = _mapper.Map<Maybe<PriceHistoryDto>>(priceHistory);

        return dto;
    }
}
