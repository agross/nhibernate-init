using FluentNHibernate.Cfg;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public interface INHibernatePersistenceModel
	{
		void AddMappings(MappingConfiguration configuration);
	}
}