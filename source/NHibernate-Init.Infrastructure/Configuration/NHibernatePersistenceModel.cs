using FluentNHibernate.Cfg;
using FluentNHibernate.Utils;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public class NHibernatePersistenceModel : INHibernatePersistenceModel
	{
	    public NHibernatePersistenceModel()
	    {
	        MappingContributors = new IMappingContributor[] { };
	    }

		public IMappingContributor[] MappingContributors
		{
			get;
			set;
		}

		public void AddMappings(MappingConfiguration configuration)
		{
			MappingContributors.Each(x => x.Apply(configuration));
		}
	}
}