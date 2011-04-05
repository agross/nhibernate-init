using FluentNHibernate.Cfg.Db;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public interface IPersistenceConfiguration
	{
		IPersistenceConfigurer GetConfiguration();
	}
}