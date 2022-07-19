using System.Diagnostics.CodeAnalysis;
using Holidays.BlazorUI.ViewModels;
using Holidays.Core.InfrastructureInterfaces;

namespace Holidays.BlazorUI.Services;

internal sealed class OffersService
{
    private readonly List<Func<Task>> _onChangedActions = new();
    private SortedSet<OfferViewModel>? _viewModels;

    public bool IsInitialized => _viewModels is not null;

    public IReadOnlyCollection<OfferViewModel> Offers => _viewModels ?? throw new InvalidOperationException(
            $"{nameof(OffersService)} is not initialized. Call {nameof(Initialize)} method before.");

    public async Task Initialize(IOffersRepository repository)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException(
                $"{nameof(OffersService)} has been initialized already.");
        }

        var offers = await repository.GetAll();

        _viewModels = new SortedSet<OfferViewModel>(
            offers.Select(OfferViewModel.FromOffer),
            new OfferViewModelComparer());
    }

    public Task Add(OfferViewModel offer)
    {
        ThrowIfUninitialized(_viewModels);

        _viewModels.Add(offer);

        return InvokeOnChangedActions();
    }

    public Task Invoke(Guid offerId, Action<OfferViewModel> action)
    {
        ThrowIfUninitialized(_viewModels);

        var viewModel = _viewModels.Single(o => o.Id == offerId);

        return Invoke(viewModel, action);
    }

    public Task Invoke(OfferViewModel viewModel, Action<OfferViewModel> action)
    {
        ThrowIfUninitialized(_viewModels);

        _viewModels.Remove(viewModel);
        action(viewModel);
        _viewModels.Add(viewModel);

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

        _viewModels.Remove(viewModel);

        return InvokeOnChangedActions();
    }

    public IDisposable OnChanged(Func<Task> action)
    {
        _onChangedActions.Add(action);

        var subscription = new Subscription(() => _onChangedActions.Remove(action));

        return subscription;
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

    private static void ThrowIfUninitialized([NotNull] SortedSet<OfferViewModel>? viewModels)
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
