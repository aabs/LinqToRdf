using System.Expressions;
using System.Text;

namespace LinqToRdf
{
	public interface IExpressionTranslator
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

		void Dispatch(Expression expression);
	}
}