using System;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NHibernate_Init.Infrastructure.Persistence;

namespace NHibernate_Init.Integration.Tests
{
	internal class SchemaCreatorAndSessionSource : INHibernateInitializationAware
	{
		readonly Action<ISession> _sessionSetter;

		public SchemaCreatorAndSessionSource(Action<ISession> sessionSetter)
		{
			_sessionSetter = sessionSetter;
		}

		public void BeforeInitialization()
		{
		}

		public void Configuring(Configuration configuration)
		{
		}

		public void Configured(Configuration configuration)
		{
		}

		public void Initialized(Configuration configuration, ISessionFactory sessionFactory)
		{
			var session = sessionFactory.OpenSession();

			var export = new SchemaExport(configuration);
			export.Execute(false, true, false, session.Connection, null);

			_sessionSetter(session);
		}
	}
}