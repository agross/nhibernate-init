using System;

using NHibernate;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public interface INHibernateSessionFactory : IDisposable
	{
		ISession CreateSession();
	}
}