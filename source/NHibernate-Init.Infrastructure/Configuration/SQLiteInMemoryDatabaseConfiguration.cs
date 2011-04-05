using FluentNHibernate.Cfg.Db;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public class SQLiteInMemoryDatabaseConfiguration : IPersistenceConfiguration
	{
		public IPersistenceConfigurer GetConfiguration()
		{
			return SQLiteConfiguration
				.Standard
				.InMemory()
				.ShowSql();
		}
	}
}