using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Holidays.BlazorUI.ViewModels;
using Holidays.Core.InfrastructureInterfaces;

namespace Holidays.BlazorUI.Services;

internal sealed class OffersService
{
    private static readonly OfferViewModelComparer ViewModelComparer = new();

    private readonly List<Func<Task>> _onChangedActions = new();
    private ConcurrentDictionary<Guid, OfferViewModel>? _viewModels;

    public bool IsInitialized => _viewModels is not null;

    public IReadOnlyList<OfferViewModel> Offers => GetOffersOrdered();

    public async Task Initialize(IOffersRepository repository)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException(
                $"{nameof(OffersService)} has been initialized already.");
        }

        var offers = await repository.GetAll();

        _viewModels = new ConcurrentDictionary<Guid, OfferViewModel>(
            offers.Select(offer => KeyValuePair.Create(offer.Id, OfferViewModel.FromOffer(offer))));
    }

    public Task Add(OfferViewModel offer)
    {
        ThrowIfUninitialized(_viewModels);

        var addedOrExistingOffer = _viewModels.GetOrAdd(offer.Id, offer);

        if (addedOrExistingOffer.IsRemoved)
        {
            addedOrExistingOffer.MarkAsNew();
        }

        return InvokeOnChangedActions();
    }

    public Task Invoke(Guid offerId, Action<OfferViewModel> action)
    {
        ThrowIfUninitialized(_viewModels);

        var viewModel = _viewModels[offerId];

        return Invoke(viewModel, action);
    }

    public Task Invoke(OfferViewModel viewModel, Action<OfferViewModel> action)
    {
        ThrowIfUninitialized(_viewModels);

        action(viewModel);

        return InvokeOnChangedActions();
    }

    public Task Remove(OfferViewModel viewModel)
    {
        ThrowIfUninitialized(_viewModels);

        if (!viewModel.IsRemoved)
        {
            throw new InvalidOperationException(
                "Cannot remove offer from collection.");
        }

        var removed = _viewModels.Remove(viewModel.Id, out _);

        if (!removed)
        {
            throw new InvalidOperationException(
                "Cannot remove offer from collection.");
        }

        return InvokeOnChangedActions();
    }

    public IDisposable OnChanged(Func<Task> action)
    {
        _onChangedActions.Add(action);

        var subscription = new Subscription(() => _onChangedActions.Remove(action));

        return subscription;
    }

    private IReadOnlyList<OfferViewModel> GetOffersOrdered()
    {
        if (_viewModels is null)
        {
            throw new InvalidOperationException(
                $"{nameof(OffersService)} is not initialized. Call {nameof(Initialize)} method before.");
        }

        return _viewModels.Values
            .OrderBy(vm => vm, ViewModelComparer)
            .ToArray();
    }

    private Task InvokeOnChangedActions()
    {
        return _onChangedActions.Count switch
        {
            0 => Task.CompletedTask,
            1 => _onChangedActions[0](),
            _ => InvokeAsync()
        };

        async Task InvokeAsync()
        {
            foreach (var action in _onChangedActions)
            {
                await action();
            }
        }
    }

    private static void ThrowIfUninitialized([NotNull] ConcurrentDictionary<Guid, OfferViewModel>? viewModels)
    {
        if (viewModels is null)
        {
            throw new InvalidOperationException(
                $"{nameof(OffersService)} is not initialized. Call {nameof(Initialize)} method before.");
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _onDispose;


        public Subscription(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}
