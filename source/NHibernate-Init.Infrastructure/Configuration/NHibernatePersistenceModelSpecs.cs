using FluentNHibernate.Cfg;
using FluentNHibernate.Diagnostics;

using Machine.Specifications;
using Machine.Specifications.Utility;

using Rhino.Mocks;

namespace NHibernate_Init.Infrastructure.Configuration
{
	[Subject(typeof(NHibernatePersistenceModel))]
	public class When_the_persistence_model_is_built
	{
		static NHibernatePersistenceModel Model;
		static MappingConfiguration MappingConfiguration;
		static IMappingContributor[] MappingContributors;

		Establish context = () =>
			{
				MappingContributors = new[]
				                      {
				                      	MockRepository.GenerateStub<IMappingContributor>(),
				                      	MockRepository.GenerateStub<IMappingContributor>()
				                      };

				Model = new NHibernatePersistenceModel { MappingContributors = MappingContributors };

				MappingConfiguration = new MappingConfiguration(new NullDiagnosticsLogger());
			};

		Because of = () => Model.AddMappings(MappingConfiguration);

		It should_get_the_mappings_from_all_mapping_contributors =
			() => MappingContributors.Each(x => x.AssertWasCalled(c => c.Apply(MappingConfiguration)));
	}
}