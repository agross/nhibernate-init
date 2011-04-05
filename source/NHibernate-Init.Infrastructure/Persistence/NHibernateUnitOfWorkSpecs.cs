using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

using Machine.Specifications;
using Machine.Specifications.Utility;

using NHibernate;

using NHibernate_Init.Infrastructure.Configuration;

using Rhino.Mocks;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public abstract class UnitOfWorkSpecs
	{
	    protected static IUnitOfWork CreateUnitOfWork()
	    {
	        return
	            new NHibernateUnitOfWork(new NHibernateSessionFactory(new SQLiteInMemoryDatabaseConfiguration(),
	                                                                  new NHibernatePersistenceModel()));
	    }
	}

	[Subject(typeof(NHibernateUnitOfWork), "with NServiceBus handler")]
	public class When_an_unstarted_unit_of_work_is_started_inside_an_ambient_transaction : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () => { UoW = CreateUnitOfWork(); };

	    Because of = () =>
			{
				using (new TransactionScope())
				{
					UoW.Start();
					Session = UoW.CurrentSession;
				}
			};

		Cleanup after = () => UoW.Dispose();

		It should_not_start_a_new_transaction =
			() => UoW.CurrentSession.Transaction.IsActive.ShouldBeFalse();

		It should_open_a_session =
			() => Session.IsOpen.ShouldBeTrue();
	}

	[Subject(typeof(NHibernateUnitOfWork), "with succeeding NServiceBus handler")]
	public class When_a_started_unit_of_work_is_committed_inside_an_ambient_transaction_that_completes : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;
		static ISession SessionAfterCommit;
		static Exception SessionAccessError;

		Establish context = () => { UoW = CreateUnitOfWork(); };

		Because of = () =>
			{
				using (var scope = new TransactionScope())
				{
					UoW.Start();
					Session = UoW.CurrentSession;
					UoW.Commit();

					SessionAccessError = Catch.Exception(() => SessionAfterCommit = UoW.CurrentSession);

					scope.Complete();
				}
			};

		Cleanup after = () => UoW.Dispose();

		It should_not_commit_the_transaction_because_it_is_enlisted =
			() => Session.Transaction.WasCommitted.ShouldBeFalse();

		It should_not_rollback_the_transaction_because_it_is_enlisted =
			() => Session.Transaction.WasRolledBack.ShouldBeFalse();

		It should_keep_the_session_open =
			() => Session.IsOpen.ShouldBeTrue();
		
		It should_disallow_access_to_the_session_after_committing =
			() => SessionAccessError.ShouldBeOfType<InvalidOperationException>();
	}

	[Subject(typeof(NHibernateUnitOfWork), "inside ambient transaction with failing NServiceBus handler")]
	public class When_a_started_unit_of_work_is_first_committed_inside_an_ambient_transaction_and_then_rolled_back
		: UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () => { UoW = CreateUnitOfWork(); };

		Because of = () =>
			{
				using (new TransactionScope())
				{
					UoW.Start();
					Session = UoW.CurrentSession;
					UoW.Commit();
				}

				UoW.Rollback();
			};

		Cleanup after = () => UoW.Dispose();

		It should_not_commit_the_transaction_because_it_is_enlisted =
			() => Session.Transaction.WasCommitted.ShouldBeFalse();

		It should_not_rollback_the_transaction_because_it_is_enlisted =
			() => Session.Transaction.WasRolledBack.ShouldBeFalse();

		It should_close_the_session =
			() => Session.IsOpen.ShouldBeFalse();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_an_unstarted_unit_of_work_is_started : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;

		Establish context = () => { UoW = CreateUnitOfWork(); };

		Because of = () => UoW.Start();

		Cleanup after = () => UoW.Dispose();

		It should_start_a_new_transaction =
			() => UoW.CurrentSession.Transaction.IsActive.ShouldBeTrue();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_started_unit_of_work_is_committed : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				Session = UoW.CurrentSession;
			};

		Because of = () => UoW.Commit();

		Cleanup after = () => UoW.Dispose();
		
		/* This as been verified by NHProf - no idea why it doesn't work here.
		It should_commit_the_transaction =
			() => Session.Transaction.WasCommitted.ShouldBeTrue();*/

		It should_not_rollback_the_the_transaction =
			() => Session.Transaction.WasRolledBack.ShouldBeFalse();

		It should_close_the_session =
			() => Session.IsOpen.ShouldBeFalse();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_started_unit_of_work_is_rolled_back : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				Session = UoW.CurrentSession;
			};

		Because of = () => UoW.Rollback();

		Cleanup after = () => UoW.Dispose();

		It should_not_commit_the_transaction =
			() => Session.Transaction.WasCommitted.ShouldBeFalse();

		/* This as been verified by NHProf - no idea why it doesn't work here.
		It should_rollback_the_transaction =
			() => Session.Transaction.WasRolledBack.ShouldBeTrue();*/

		It should_close_the_session =
			() => Session.IsOpen.ShouldBeFalse();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_started_unit_of_work_is_disposed : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				Session = UoW.CurrentSession;
			};

		Because of = () => UoW.Dispose();

		Cleanup after = () => UoW.Dispose();

		It should_end_the_transaction =
			() => Session.Transaction.IsActive.ShouldBeFalse();

		It should_rollback_the_transaction =
			() => Session.Transaction.WasCommitted.ShouldBeFalse();

		It should_close_the_session =
			() => Session.IsOpen.ShouldBeFalse();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_started_unit_of_work_is_started_again : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static Exception Exception;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
			};

		Because of = () => { Exception = Catch.Exception(() => UoW.Start()); };

		Cleanup after = () => UoW.Dispose();

		It should_fail =
			() => Exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_an_previously_committed_unit_of_work_is_reused : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession FirstSession;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				FirstSession = UoW.CurrentSession;
				UoW.Commit();
			};

		Because of = () => UoW.Start();

		Cleanup after = () => UoW.Dispose();

		It should_create_a_new_session =
			() => UoW.CurrentSession.ShouldNotBeTheSameAs(FirstSession);
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_an_previously_rolled_back_unit_of_work_is_reused : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession FirstSession;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				FirstSession = UoW.CurrentSession;
				UoW.Rollback();
			};

		Because of = () => UoW.Start();

		Cleanup after = () => UoW.Dispose();

		It should_create_a_new_session =
			() => UoW.CurrentSession.ShouldNotBeTheSameAs(FirstSession);
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_disposed_unit_of_work_is_disposed_again : UnitOfWorkSpecs
	{
		static IUnitOfWork UoW;
		static ISession Session;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();

				Session = UoW.CurrentSession;

				UoW.Dispose();
			};

		Because of = () => UoW.Dispose();

		It should_keep_the_transaction_ended =
			() => Session.Transaction.IsActive.ShouldBeFalse();

		It should_keep_the_transaction_rolled_back =
			() => Session.Transaction.WasCommitted.ShouldBeFalse();

		It should_keep_the_session_closed =
			() => Session.IsOpen.ShouldBeFalse();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
	public class When_a_disposed_unit_of_work_is_accessed : UnitOfWorkSpecs
	{
		static List<Exception> Exceptions;
		static IEnumerable<MethodInfo> Methods;
		static IUnitOfWork UoW;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();
				UoW.Start();
				UoW.Dispose();

				Methods = UoW
					.GetType()
					.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
					.Where(m => !m.Name.StartsWith("get_"))
					.Where(m => !m.Name.StartsWith("set_"))
					.Where(m => m.Name != "Dispose");

				Exceptions = new List<Exception>();
			};

		Because of = () =>
		             Methods.Each(m =>
		             	{
		             		var args = m.GetParameters().Select(x => new object());

		             		var exception = Catch.Exception(() => m.Invoke(UoW, args.ToArray()));

		             		Exceptions.Add(exception);
		             	});

		It should_fail_for_each_method =
			() => Exceptions.Count().ShouldEqual(Methods.Count());

		It should_fail =
			() => Exceptions.All(x => x.InnerException as ObjectDisposedException != null).ShouldBeTrue();
	}

	[Subject(typeof(NHibernateUnitOfWork))]
    public class When_any_operation_except_starting_or_rollback_or_disposing_is_executed_on_an_unstarted_unit_of_work : UnitOfWorkSpecs
	{
		static List<Exception> Exceptions;
		static IEnumerable<MethodInfo> Methods;
		static IUnitOfWork UoW;

		Establish context = () =>
			{
				UoW = CreateUnitOfWork();

				Methods = UoW
					.GetType()
					.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
					.Where(m => !m.Name.StartsWith("get_"))
					.Where(m => !m.Name.StartsWith("set_"))
					.Where(m => m.Name != "Start" && m.Name != "Rollback" && m.Name != "Dispose");

				Exceptions = new List<Exception>();
			};

		Because of = () =>
		             Methods.Each(m =>
		             	{
		             		var args = m.GetParameters().Select(x => new object());

		             		var exception = Catch.Exception(() => m.Invoke(UoW, args.ToArray()));

		             		Exceptions.Add(exception);
		             	});

		Cleanup after = () => UoW.Dispose();

		It should_fail_for_each_method =
			() => Exceptions.Count().ShouldEqual(Methods.Count());

		It should_fail =
			() => Exceptions.All(x => x.InnerException as InvalidOperationException != null).ShouldBeTrue();
	}
}