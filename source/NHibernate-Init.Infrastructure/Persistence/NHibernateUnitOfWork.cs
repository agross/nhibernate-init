using System;
using System.Transactions;

using NHibernate;

namespace NHibernate_Init.Infrastructure.Persistence
{
    public class NHibernateUnitOfWork : IUnitOfWork
	{
		readonly INHibernateSessionFactory _sessionFactory;
		ISession _currentSession;
		bool _isDisposed;

		public NHibernateUnitOfWork(INHibernateSessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public ISession CurrentSession
		{
			get
			{
				AssertIsNotDisposed();
				AssertIsStarted();

				return _currentSession;
			}
		}

		public void Start()
		{
			AssertIsNotDisposed();
			AssertIsNotStarted();

			_currentSession = _sessionFactory.CreateSession();
			StartNewTransaction();

			IsStarted = true;
		}

		public void Commit()
		{
			AssertIsNotDisposed();
			AssertIsStarted();

			if (InsideAmbientTransaction())
			{
				IsStarted = false;
				return;
			}

			_currentSession.Transaction.Commit();
			_currentSession.Transaction.Dispose();

			_currentSession.Dispose();

			IsStarted = false;
		}

		public void Rollback()
		{
			AssertIsNotDisposed();

			if (_currentSession != null)
			{
				if (_currentSession.Transaction.IsActive)
				{
					_currentSession.Transaction.Rollback();
					_currentSession.Transaction.Dispose();
				}

				if (_currentSession.IsOpen)
				{
					_currentSession.Dispose();
				}
			}

			IsStarted = false;
		}

		public bool IsStarted
		{
			get;
			private set;
		}

		/// <summary>
		///	Cleans up managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			// Prevent the destructor from being called.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///	Central method for cleaning up resources.
		/// </summary>
		protected virtual void Dispose(bool @explicit)
		{
			if (_isDisposed || !IsStarted)
			{
				return;
			}

			// If explicit is true, then this method was called through
			// the public Dispose().
			if (@explicit)
			{
				// Release or cleanup managed resources.
				if (IsStarted)
				{
					Rollback();
				}

				if (_currentSession.Transaction != null)
				{
					_currentSession.Transaction.Dispose();
				}
				if (_currentSession != null)
				{
					_currentSession.Dispose();
				}
			}

			// Always release or cleanup (any) unmanaged resources.
			_isDisposed = true;
		}

		/// <summary>
		///	Finalizes an instance of the <see cref = "NHibernateUnitOfWork" /> class.
		/// </summary>
		/// <remarks>
		///	This destructor will run only if the Dispose method does not get called.
		///	The destructor is called indeterministicly by the Garbage Collector.
		/// </remarks>
		~NHibernateUnitOfWork()
		{
			// Since other managed ob jects are disposed automatically,
			// we should not try to dispose any managed resources.
			// We therefore pass false to Dispose().
			Dispose(false);
		}

		void StartNewTransaction()
		{
			if (InsideAmbientTransaction())
			{
				return;
			}

			_currentSession.BeginTransaction();
		}

		static bool InsideAmbientTransaction()
		{
			return Transaction.Current != null;
		}

		void AssertIsNotDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		void AssertIsStarted()
		{
			if (IsStarted)
			{
				return;
			}

			throw new InvalidOperationException(
				"Must initialize (call Start()) the unit of work before committing or rolling back");
		}

		void AssertIsNotStarted()
		{
			if (!IsStarted)
			{
				return;
			}

			throw new InvalidOperationException("You cannot restart a started unit of work");
		}
	}
}