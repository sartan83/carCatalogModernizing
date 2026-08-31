namespace CarCatalog.ServiceContracts;

public static class ServiceAddresses
{
    /// <summary>
    /// The path the legacy IIS-hosted service answered on, kept so clients only need a new host name.
    /// </summary>
    public const string CatalogService = "/CatalogService.svc";
}
