using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace CarCatalog.CharacterizationTests.Fakes
{
    /// <summary>
    /// Registered through App.config. Resolving the provider manifest token normally opens a
    /// connection to read the SQL Server version; a fixed token keeps model building, and therefore
    /// change tracking, entirely offline.
    /// </summary>
    public class TestDbConfiguration : DbConfiguration
    {
        public TestDbConfiguration()
        {
            SetManifestTokenResolver(new SqlServer2012ManifestTokenResolver());
        }

        private sealed class SqlServer2012ManifestTokenResolver : IManifestTokenResolver
        {
            public string ResolveManifestToken(DbConnection connection)
            {
                return "2012";
            }
        }
    }
}
