using System.Reflection;

namespace Holidays.Postgres.Initialization;

internal class EmbeddedResourcesSqlSource
{
    private readonly IReadOnlyList<(string TableName, string ResourceName)> _resources;

    public EmbeddedResourcesSqlSource()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceNames = assembly.GetManifestResourceNames();
        _resources = ReadResourcesOrdered(resourceNames);
    }

    public IEnumerable<string> GetAvailableTableNames() =>
        _resources.Select(r => r.TableName);

    public async Task<string> ReadSqlForTable(string tableName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resource = _resources.SingleOrDefault(t => t.TableName == tableName);
        if (resource == default)
        {
            throw new ArgumentException(
                $"could not find resource file of table {tableName}", nameof(tableName));
        }

        await using var stream = assembly.GetManifestResourceStream(resource.ResourceName);
        if (stream is null)
        {
            throw new IOException(
                $"something went wrong, cannot create stream for embedded resource {resource.ResourceName}");
        }

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    private static string? ResourceNameToTableName(string resourceName, out int order)
    {
        // from Namespace.AnotherNamespace.directory.12_tablename.sql
        // to 12_tablename

        order = 0;

        var resourceNameParts = resourceName.Split('.');

        if (resourceNameParts.Length < 2 || resourceNameParts[^1] != "sql")
        {
            return default;
        }

        var fileName = resourceNameParts[^2];

        // from 12_tablename
        // to tablename
        // out order = 12

        var parts = fileName.Split('_');

        order = int.Parse(parts[0]);

        return parts.Length < 2
            ? default
            : string.Join('_', parts.Skip(1));
    }

    private static IReadOnlyList<(string FileName, string ResourceName)> ReadResourcesOrdered(
        IReadOnlyCollection<string> resourceNames)
    {
        var resources = new List<(int Order, string TableName, string ResourceName)>(resourceNames.Count);
        foreach (var t in resourceNames)
        {
            var tableName = ResourceNameToTableName(t, out var order);

            if (!string.IsNullOrEmpty(tableName))
            {
                resources.Add((order, tableName, t));
            }
        }

        return resources
            .OrderBy(t => t.Order)
            .Select(t => (t.TableName, t.ResourceName))
            .ToArray();
    }
}
