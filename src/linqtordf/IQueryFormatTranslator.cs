using System.Expressions;
using System.Reflection;
using System.Text;
using C5;

namespace LinqToRdf
{
	public interface IQueryFormatTranslator
	{
		StringBuilder StringBuilder
		{
			get;
			set;
		}

		string InstancePlaceholderName
		{
			get;
		}

		ITypeTranslator TypeTranslator
		{
			get;
			set;
		}

		HashSet<MemberInfo> Parameters
		{
			get;
			set;
		}

		void Dispatch(Expression expression);
	}
}