using System;

using NHibernate;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public interface IUnitOfWork : IDisposable
	{
		void Start();
		void Commit();
		void Rollback();

		bool IsStarted
		{
			get;
		}

	    ISession CurrentSession
	    {
	        get;
	    }
	}
}