using FluentNHibernate.Mapping;

namespace NHibernate_Init.Domain.Mapping
{
	class TagInfoMap : ClassMap<TagInfo>
	{
		public TagInfoMap()
		{
			Id(x => x.Id).GeneratedBy.HiLo("3");

			Map(x => x.Tags);
		} 
	}
}