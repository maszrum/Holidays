using Holidays.Configuration;
using Serilog;

namespace Holidays.CollectingApp;

internal class ApplicationBootstrapper
{
    private const string ConfigurationFile = "appsettings.json";

    private readonly CancellationTokenSource _applicationCts = new();
    private readonly ILogger _logger;

    private ApplicationBootstrapper(
        ApplicationConfiguration configuration,
        ILogger logger)
    {
        Configuration = configuration;
        _logger = logger;
    }

    public ApplicationConfiguration Configuration { get; }

    public CancellationToken ApplicationCancellationToken => _applicationCts.Token;

    public ILogger GetLogger() => _logger;

    public ILogger GetLogger<TContext>() => _logger.ForContext<TContext>();

    private static ApplicationConfiguration CreateConfiguration()
    {
        var configuration = new ApplicationConfiguration(
            ConfigurationFile,
            overrideWithEnvironmentVariables: true);

        return configuration;
    }

    private static ILogger CreateLogger(ApplicationConfiguration configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration.ConfigurationRoot)
            .WriteTo.Console()
            .CreateLogger();

        return logger;
    }

    private void SetupApplicationEvents()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception exception)
            {
                GetLogger<ApplicationBootstrapper>().Fatal(
                    exception,
                    "Unhandled exception occured");
            }
            else
            {
                GetLogger<ApplicationBootstrapper>().Fatal(
                    "Unhandled unknown exception occured");
            }
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            GetLogger<ApplicationBootstrapper>().Information(
            "Closing application due to process exit event");

            _applicationCts.Cancel();
        };

        Console.CancelKeyPress += (_, e) =>
        {
            GetLogger<ApplicationBootstrapper>().Information(
                "Closing application due to cancel key pressed");

            _applicationCts.Cancel();
            e.Cancel = true;
        };
    }

    public static async Task Run(Func<ApplicationBootstrapper, Task> action)
    {
        var configuration = CreateConfiguration();
        var logger = CreateLogger(configuration);

        var bootstrapper = new ApplicationBootstrapper(configuration, logger);

        bootstrapper.SetupApplicationEvents();

        try
        {
            await action(bootstrapper);
        }
        catch (OperationCanceledException)
        {
            if (!bootstrapper.ApplicationCancellationToken.IsCancellationRequested)
            {
                throw;
            }
        }
    }
}
