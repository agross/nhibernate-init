using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate_Init.Shared.NHibernateSupport
{
	public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T>
	{
		#region IEquatable<TEvent> Members
		public virtual bool Equals(T other)
		{
			if (other == null)
			{
				return false;
			}

			Type leftType = GetType();
			Type otherType = other.GetType();
			if (leftType != otherType)
			{
				return false;
			}

			IEnumerable<FieldInfo> fields = GetFieldsFromTypeHierarchy();
			foreach (FieldInfo field in fields)
			{
				object value1 = field.GetValue(this);
				object value2 = field.GetValue(other);

				if (value1 == null)
				{
					if (value2 != null)
					{
						return false;
					}
				}
				else if (!value1.Equals(value2))
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			return Equals(obj as T);
		}

		public override int GetHashCode()
		{
			IEnumerable<FieldInfo> fields = GetFieldsFromTypeHierarchy();

			const int StartValue = 17;
			const int Multiplier = 59;

			int hashCode = StartValue;
			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);

				hashCode = (hashCode * Multiplier) + (value != null ? value.GetHashCode() : StartValue);
			}

			return hashCode;
		}

		IEnumerable<FieldInfo> GetFieldsFromTypeHierarchy()
		{
			Type t = GetType();

			List<FieldInfo> fields = new List<FieldInfo>();
			while (t != typeof(object))
			{
				fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

				t = t.BaseType;
			}

			return fields;
		}

		public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
		{
			return !Equals(left, right);
		}
	}
}
