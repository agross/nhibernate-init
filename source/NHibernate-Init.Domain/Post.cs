using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
    public class Post : Entity
    {
        public string Title
        {
            get;
            set;
        }
    }
}