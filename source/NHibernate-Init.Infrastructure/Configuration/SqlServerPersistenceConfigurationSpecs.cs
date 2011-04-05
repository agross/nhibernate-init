using Machine.Specifications;

using NHibernate.Cfg;

namespace NHibernate_Init.Infrastructure.Configuration
{
	[Subject(typeof(SqlServerPersistenceConfiguration))]
	public class When_the_NHibernate_configuration_is_created_for_SQL_Server
	{
		static SqlServerPersistenceConfiguration PersistenceConfiguration;
		static NHibernate.Cfg.Configuration Configuration;

		Establish context = () =>
			{
				PersistenceConfiguration = new SqlServerPersistenceConfiguration("connection string", true);
				Configuration = new NHibernate.Cfg.Configuration();
			};

		Because of = () => PersistenceConfiguration.GetConfiguration().ConfigureProperties(Configuration);

		It should_set_the_connection_string =
			() => Configuration.Properties[Environment.ConnectionString].ShouldEqual("connection string");
		
		It should_enable_batching =
			() => Configuration.Properties[Environment.BatchSize].ShouldEqual(10.ToString());
		
		It should_enable_the_reflection_optimizer =
			() => Environment.UseReflectionOptimizer.ShouldBeTrue();
		
		It should_show_SQL_statements =
			() => Configuration.Properties[Environment.ShowSql].ShouldNotBeEmpty();
	}
}