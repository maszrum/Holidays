using Holidays.BlazorUI.Services;
using Holidays.BlazorUI.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Holidays.BlazorUI.Components;

public partial class OfferView
{
    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public OfferViewModel Offer { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public EventCallback<OfferViewModel> AcknowledgeRequested { get; set; }

    [Parameter]
    [EditorRequired]
    public EventCallback<OfferViewModel> RemoveRequested { get; set; }

    private async Task GoToDetails()
    {
        await Js.InvokeVoidAsync("open", Offer.DetailsUrl, "_blank");
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
