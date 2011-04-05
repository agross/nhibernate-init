using NHibernate;
using NHibernate.Event;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public class NHibernateEventListenerInitializer : INHibernateInitializationAware
	{
		public IPostInsertEventListener[] PostInsert
		{
			get;
			set;
		}

		public IPostUpdateEventListener[] PostUpdate
		{
			get;
			set;
		}

		public void BeforeInitialization()
		{
		}

		public void Configuring(NHibernate.Cfg.Configuration configuration)
		{
			configuration.EventListeners.PostInsertEventListeners = PostInsert;
			configuration.EventListeners.PostUpdateEventListeners = PostUpdate;
		}

		public void Configured(NHibernate.Cfg.Configuration configuration)
		{
		}

		public void Initialized(NHibernate.Cfg.Configuration configuration, ISessionFactory sessionFactory)
		{
		}
	}
}