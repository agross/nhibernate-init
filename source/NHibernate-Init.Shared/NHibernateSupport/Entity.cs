using System;

namespace NHibernate_Init.Shared.NHibernateSupport
{
	public abstract class Entity : IEquatable<Entity>
	{
		readonly int _id;

		protected Entity()
		{
		}

		protected Entity(int id)
		{
			_id = id;
		}

		public virtual int Id
		{
			get { return _id; }
		}

		public virtual bool Equals(Entity other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Id.Equals(default(int)) ? base.Equals(other) : other.Id.Equals(Id);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Entity);
		}

		public override int GetHashCode()
		{
			return Id.Equals(default(int)) ? base.GetHashCode() : Id.GetHashCode();
		}

		public static bool operator ==(Entity left, Entity right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Entity left, Entity right)
		{
			return !Equals(left, right);
		}
	}
}