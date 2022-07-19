using Holidays.BlazorUI.Services;
using Holidays.BlazorUI.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Holidays.BlazorUI.Pages;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class OffersList : IDisposable
{
    private IDisposable? _onChangedSubscription;

    [Inject] private OffersService OffersService { get; set; } = default!;

    private IReadOnlyCollection<OfferViewModel>? Offers => OffersService.IsInitialized
        ? OffersService.Offers
        : default;

    public void Dispose()
    {
        _onChangedSubscription?.Dispose();
    }

    protected override void OnInitialized()
    {
        _onChangedSubscription = OffersService
            .OnChanged(() => InvokeAsync(StateHasChanged));
    }

    private Task Acknowledge(OfferViewModel viewModel)
    {
        return OffersService.Invoke(
            viewModel,
            vm => vm.Acknowledge());
    }

    private Task Remove(OfferViewModel viewModel)
    {
        return OffersService.Remove(viewModel);
    }
}
