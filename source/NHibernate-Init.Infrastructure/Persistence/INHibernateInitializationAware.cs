using NHibernate;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public interface INHibernateInitializationAware
	{
		void BeforeInitialization();
		void Configuring(NHibernate.Cfg.Configuration configuration);
		void Configured(NHibernate.Cfg.Configuration configuration);
		void Initialized(NHibernate.Cfg.Configuration configuration, ISessionFactory sessionFactory);
	}
}