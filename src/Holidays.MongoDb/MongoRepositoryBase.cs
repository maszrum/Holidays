using System.Runtime.CompilerServices;
using MongoDB.Driver;

namespace Holidays.MongoDb;

public abstract class MongoRepositoryBase : IDisposable
{
    private IClientSessionHandle? _session;
    private readonly bool _externalSession;
    
    protected MongoRepositoryBase(ConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    protected MongoRepositoryBase(ConnectionFactory connectionFactory, IClientSessionHandle session)
    {
        ConnectionFactory = connectionFactory;
        _session = session;
        _externalSession = true;
    }
    
    protected ConnectionFactory ConnectionFactory { get; }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected Task<IClientSessionHandle> GetSession()
    {
        return _session is null 
            ? StartAndSet() 
            : Task.FromResult(_session);

        async Task<IClientSessionHandle> StartAndSet()
        {
            _session = await ConnectionFactory.StartSession();
            return _session;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_externalSession && _session is not null)
            {
                _session.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
