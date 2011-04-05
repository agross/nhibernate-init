using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;

using Machine.Specifications;
using Machine.Specifications.Utility;

using NHibernate;
using NHibernate.Util;

using NHibernate_Init.Infrastructure.Configuration;

using Rhino.Mocks;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public class MappedClass
	{
		protected MappedClass()
		{
		}

		public virtual int Id
		{
			get;
			set;
		}
	}

	public class MappedClassMap : ClassMap<MappedClass>
	{
		public MappedClassMap()
		{
			Id(x => x.Id);
		}
	}

	[Subject(typeof(NHibernateSessionFactory))]
	public class When_a_NHibernate_session_created_for_the_first_time
	{
		static IPersistenceConfiguration Configuration;
		static INHibernatePersistenceModel Model;
		static NHibernateSessionFactory Factory;
		static INHibernateInitializationAware[] Initializers;
		static ISession Session;

		Establish context = () =>
			{
				Configuration = MockRepository.GenerateStub<IPersistenceConfiguration>();
				Configuration
					.Stub(x => x.GetConfiguration())
					.Return(new SQLiteInMemoryDatabaseConfiguration().GetConfiguration());

				Model = MockRepository.GenerateStub<INHibernatePersistenceModel>();
				Model
					.Stub(x => x.AddMappings(null))
					.IgnoreArguments()
					.WhenCalled(x =>
						{
							var config = (MappingConfiguration) x.Arguments.First();
							config.FluentMappings.Add<MappedClassMap>();
						});

				Initializers = new[]
				               {
				               	MockRepository.GenerateStub<INHibernateInitializationAware>(),
				               	MockRepository.GenerateStub<INHibernateInitializationAware>()
				               };

				Factory = new NHibernateSessionFactory(Configuration, Model)
				          {
				          	Initializers = Initializers
				          };
			};

		Because of = () => { Session = Factory.CreateSession(); };

		It should_retrieve_the_persistence_configuration =
			() => Configuration.AssertWasCalled(x => x.GetConfiguration());

		It should_add_mappings_from_the_persistence_model =
			() => Model.AssertWasCalled(x => x.AddMappings(Arg<MappingConfiguration>.Is.NotNull),
			                                       o => o.Repeat.AtLeastOnce());

		It should_invoke_the_initializers_before_initialization =
			() => Initializers.Each(x => x.AssertWasCalled(i => i.BeforeInitialization()));

		It should_invoke_the_initializers_while_configuring =
			() => Initializers.Each(x => x.AssertWasCalled(i => i.Configuring(Arg<NHibernate.Cfg.Configuration>.Is.NotNull),
			                                               // First call: by the NHSF, second call by FNH.                                           
			                                               o => o.Repeat.Twice()));

		It should_invoke_the_initializers_with_the_actual_configuration =
			() => Initializers.Each(x => x.AssertWasCalled(i => i.Configured(Arg<NHibernate.Cfg.Configuration>.Is.NotNull)));

		It should_invoke_the_initializers_with_the_session_factory =
			() => Initializers.Each(x => x.AssertWasCalled(i => i.Initialized(Arg<NHibernate.Cfg.Configuration>.Is.NotNull,
			                                                                  Arg<ISessionFactory>.Is.NotNull)));

		It should_be_able_to_create_a_session =
			() => Session.ShouldNotBeNull();

		It should_create_a_session_that_flushes_on_commit =
			() => Session.FlushMode.ShouldEqual(FlushMode.Commit);
	}

	[Subject(typeof(NHibernateSessionFactory))]
	public class When_a_NHibernate_session_created
	{
		static IPersistenceConfiguration Configuration;
		static INHibernatePersistenceModel Model;
		static ISession Session;
		static NHibernateSessionFactory Factory;
		static INHibernateInitializationAware[] Initializers;

		Establish context = () =>
		    {
		        Configuration = new SQLiteInMemoryDatabaseConfiguration();

                Model = MockRepository.GenerateStub<INHibernatePersistenceModel>();
				Model
					.Stub(x => x.AddMappings(null))
					.IgnoreArguments()
					.WhenCalled(x =>
						{
							MappingConfiguration config = (MappingConfiguration) x.Arguments.First();
							config.FluentMappings.Add<MappedClassMap>();
						});

				Initializers = new[]
				               {
				               	MockRepository.GenerateStub<INHibernateInitializationAware>()
				               };

				Factory = new NHibernateSessionFactory(Configuration, Model)
				          {
				          	Initializers = Initializers
				          };

				Factory.CreateSession();

				Initializers = new[]
				               {
				               	MockRepository.GenerateStub<INHibernateInitializationAware>()
				               };
			};

		Because of = () => { Session = Factory.CreateSession(); };

		It should_be_able_to_create_a_session =
			() => Session.ShouldNotBeNull();

		It should_create_a_session_that_flushes_on_commit =
			() => Session.FlushMode.ShouldEqual(FlushMode.Commit);

		It should_not_reinitialize_the_session_factory =
			() => Initializers.Each(x => x.AssertWasNotCalled(i => i.BeforeInitialization()));
	}
}