using FluentNHibernate.Cfg;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public interface IMappingContributor
	{
		void Apply(MappingConfiguration configuration);
	}
}