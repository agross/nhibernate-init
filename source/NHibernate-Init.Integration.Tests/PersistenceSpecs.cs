using System.IO;

using Machine.Specifications;

using NHibernate;

using NHibernate_Init.Domain;
using NHibernate_Init.Infrastructure.Configuration;
using NHibernate_Init.Infrastructure.Persistence;

namespace NHibernate_Init.Integration.Tests
{
	public class When_loading_a_post : PersistenceSpecs
	{
		Establish context = () =>
		{
			using(var txn = Session.BeginTransaction())
			{
				Saved = new Post { Title = "title", Tags = new TagInfo { Tags = "tags" } };
				Session.Save(Saved);
				txn.Commit();
			}

			Session.Evict(Saved);
		};

		Because of = () => { Loaded = Session.Load<Post>(Saved.Id); };

		It should_be_able_to_load_the_post =
			() => Loaded.ShouldNotBeNull();

		It should_lazy_load_the_one_to_one_property =
			() => NHibernateUtil.IsInitialized(Loaded.Tags).ShouldBeFalse();

		static Post Saved;
		static Post Loaded;
	}

	public abstract class PersistenceSpecs
	{
		static NHibernateSessionFactory Factory;
		protected static ISession Session;

		Establish context = () =>
		{
			Factory = new NHibernateSessionFactory(
				new SQLiteInMemoryDatabaseConfiguration(),
				new NHibernatePersistenceModel
				{
					MappingContributors = new IMappingContributor[]
					                      {
					                      	new FluentMappingsFromAssembly(typeof(Blog)),
					                      	new FluentMappingConventions()
					                      }
				}) { Initializers = new INHibernateInitializationAware[] { new SchemaCreatorAndSessionSource(s => Session = s) } };


			Factory.Configure();
		};

		Cleanup after = () =>
		{
			Session.Dispose();
			Factory.Dispose();
		};
	}
}