using FluentNHibernate.Cfg.Db;

using NHibernate.ByteCode.Castle;

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
				.ProxyFactoryFactory(typeof(ProxyFactoryFactory).AssemblyQualifiedName)
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