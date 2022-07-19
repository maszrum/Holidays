using Holidays.BlazorUI.Services;
using Holidays.BlazorUI.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Holidays.BlazorUI.Components;

public partial class OfferView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public OfferViewModel Offer { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public EventCallback<OfferViewModel> AcknowledgeRequested { get; set; }

    [Parameter]
    [EditorRequired]
    public EventCallback<OfferViewModel> RemoveRequested { get; set; }

    private void GoToDetails()
    {
        NavigationManager.NavigateTo(Offer.DetailsUrl);
    }

    private async Task Acknowledge()
    {
        await AcknowledgeRequested.InvokeAsync(Offer);
    }

    private async Task Remove()
    {
        await RemoveRequested.InvokeAsync(Offer);
    }
}
