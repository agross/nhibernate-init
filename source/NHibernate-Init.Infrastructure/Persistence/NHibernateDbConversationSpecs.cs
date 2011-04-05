using Castle.Windsor;

using Machine.Specifications;
using Machine.Specifications.Utility;

using NHibernate;
using NHibernate.Engine;

using NHibernate_Init.Shared;
using NHibernate_Init.Shared.NHibernateSupport;

using Rhino.Mocks;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public abstract class DatabaseConversationSpecs
	{
		protected static NHibernateDbConversation DbConversation;
		protected static DomainEntity Entity;
		protected static ISession Session;
		static IUnitOfWork UoW;

		Establish context = () =>
			{
                UoW = MockRepository.GenerateStub<IUnitOfWork>();
				Session = MockRepository.GenerateStub<ISession>();

				UoW.Stub(x => x.CurrentSession).Return(Session);

				DbConversation = new NHibernateDbConversation(UoW);

				Entity = new DomainEntity();
			};
	}

	[Subject(typeof(NHibernateDbConversation))]
	public class When_an_entity_should_be_inserted_when_committing_the_unit_of_work : DatabaseConversationSpecs
	{
		Because of = () => DbConversation.Insert(Entity);

		It should_call_save_on_the_session =
			() => Session.AssertWasCalled(x => x.SaveOrUpdate(Entity));
	}

	[Subject(typeof(NHibernateDbConversation))]
	public class When_a_query_is_issued : DatabaseConversationSpecs
	{
		static IDomainEntityQueryWithStrategy Query;
		static ISessionImplementor SessionImplementor;

		Establish context = () =>
			{
				Query = MockRepository.GenerateStub<IDomainEntityQueryWithStrategy>();
			};

		Because of = () => DbConversation.Query(Query);

		It should_execute_the_query_against_the_session =
			() => Query.AssertWasCalled(x => x.Execute(Session));
	}
	
	[Subject(typeof(NHibernateDbConversation))]
	public class When_a_query_with_available_strategies_is_issued : DatabaseConversationSpecs
	{
		static IDomainEntityQueryWithStrategy Query;
		static ISessionImplementor SessionImplementor;
		static IQueryStrategy<IStrategyMarkerInterface>[] Strategies;

		Establish context = () =>
			{
				Query = MockRepository.GenerateStub<IDomainEntityQueryWithStrategy>();

				Strategies = new []
				          {
				          	MockRepository.GenerateStub<IQueryStrategy<IStrategyMarkerInterface>>(),
				          	MockRepository.GenerateStub<IQueryStrategy<IStrategyMarkerInterface>>()
				          };

				DbConversation.Container = MockRepository.GenerateStub<IWindsorContainer>();
				DbConversation.Container
					.Stub(x => x.ResolveAll(typeof(IQueryStrategy<IStrategyMarkerInterface>)))
					.Return(Strategies);

				DbConversation.Reflection = new Reflection();
			};

		Because of = () => DbConversation.Query(Query);

		It should_apply_all_query_filters_applicable_to_the_query_s_implemented_interfaces =
			() => Strategies.Each(x => x.AssertWasCalled(y => y.Apply(Query)));

		It should_execute_the_query_against_the_session =
			() => Query.AssertWasCalled(x => x.Execute(Session));

		It should_release_all_strategies =
			() => Strategies.Each(s => DbConversation.Container.AssertWasCalled(x => x.Release(s)));
	}

	public interface IStrategyMarkerInterface
	{
	}
	
	public interface IDomainEntityQueryWithStrategy : IDomainQuery<DomainEntity>, IStrategyMarkerInterface
	{
	}

	public class DomainEntity : Entity
	{
	}
}