using Serilog;

namespace Holidays.CollectingApp;

internal static class SerilogExtensions
{
    public static void Error(this ILogger logger, ErrorBase error)
    {
        if (error is ErrorWithException errorWithException)
        {
            logger.Error(errorWithException.GetException(), errorWithException.Message);
        }
        else
        {
            logger.Error(error.Message);
        }
    }

    public static void Error<TException>(this ILogger logger, ErrorBase<TException> error)
        where TException : Exception
    {
        logger.Error(error.Exception, error.Message);
    }
}
