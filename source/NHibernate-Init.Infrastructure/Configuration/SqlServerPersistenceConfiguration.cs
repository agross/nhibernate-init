using FluentNHibernate.Cfg.Db;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public class SqlServerPersistenceConfiguration : IPersistenceConfiguration
	{
		readonly string _connectionString;
		readonly bool _showSql;

		public SqlServerPersistenceConfiguration(string connectionString, bool showSql)
		{
			_connectionString = connectionString;
			_showSql = showSql;
		}

		public IPersistenceConfigurer GetConfiguration()
		{
			var configuration = MsSqlConfiguration
				.MsSql2005
				.ConnectionString(c => c.Is(_connectionString))
				.AdoNetBatchSize(10)
				.UseReflectionOptimizer()
				.UseOuterJoin();

			if (_showSql)
			{
				configuration.ShowSql();
			}

			return configuration;
		}
	}
}