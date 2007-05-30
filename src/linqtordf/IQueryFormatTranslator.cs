using System.Expressions;
using System.Text;

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

		void Dispatch(Expression expression);
	}
}