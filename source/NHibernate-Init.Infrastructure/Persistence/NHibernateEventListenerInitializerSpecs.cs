using Machine.Specifications;

using NHibernate.Event;

using Rhino.Mocks;

namespace NHibernate_Init.Infrastructure.Persistence
{
	[Subject(typeof(NHibernateEventListenerInitializer))]
	public class When_event_listeners_are_set_up
	{
		static NHibernateEventListenerInitializer Initializer;
		static NHibernate.Cfg.Configuration Configuration;

		Establish context =
			() =>
				{
					Initializer = new NHibernateEventListenerInitializer
					              {
					              	PostInsert = new[]
					              	             {
					              	             	MockRepository.GenerateStub<IPostInsertEventListener>()
					              	             },
					              	PostUpdate = new[]
					              	             {
					              	             	MockRepository.GenerateStub<IPostUpdateEventListener>()
					              	             }
					              };

					Configuration = new NHibernate.Cfg.Configuration();
				};

		Because of = () => Initializer.Configuring(Configuration);

		It should_add_the_post_insert_listeners_to_the_configuration =
			() => Configuration.EventListeners.PostInsertEventListeners.ShouldNotBeEmpty();

		It should_add_the_post_update_listeners_to_the_configuration =
			() => Configuration.EventListeners.PostUpdateEventListeners.ShouldNotBeEmpty();
	}
}