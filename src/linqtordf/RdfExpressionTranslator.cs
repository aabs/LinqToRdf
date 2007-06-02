using System.Diagnostics;
using System.Text;

namespace LinqToRdf.Sparql
{
	/// <summary>
	/// A supertype for all expression translators that target RDF in one form or another.
	/// </summary>
	public class RdfExpressionTranslator<T>
	{
		private ITypeTranslator typeTranslator = null;
		protected StringBuilder stringBuilder = new StringBuilder();

		public StringBuilder StringBuilder
		{
			get { return stringBuilder; }
			set { stringBuilder = value; }
		}

		public ITypeTranslator TypeTranslator
		{
			get { return typeTranslator; }
			set { typeTranslator = value; }
		}

		public string InstancePlaceholderName
		{
			get
			{
				return "?" + Sanitise(typeof(T).Name);
			}
		}

		protected void QueryAppend(string fmt, params object[] args)
		{
			stringBuilder.AppendFormat(fmt, args);
		}

		private string Sanitise(string s)
		{
			return s.Replace("<", "").Replace(">", "").Replace("'", "");
		}

		protected void Log(string msg, params object[] args)
		{
			Trace.WriteLine(string.Format(msg, args));
		}
	}
}