using HibernatingRhinos.Profiler.Appender.NHibernate;

using NHibernate;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public class NHibernateProfilerInitializer : INHibernateInitializationAware
	{
		public bool Enabled
		{
			get;
			set;
		}

		public void BeforeInitialization()
		{
		}

		public void Configuring(NHibernate.Cfg.Configuration configuration)
		{
		}

		public void Configured(NHibernate.Cfg.Configuration configuration)
		{
		}

		public void Initialized(NHibernate.Cfg.Configuration configuration, ISessionFactory sessionFactory)
		{
			if (!Enabled)
			{
				return;
			}

			NHibernateProfiler.Initialize();
		}
	}
}