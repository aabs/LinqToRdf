using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Expressions;
using System.IO;
using System.Query;
using System.Reflection;
using System.Text;
using SemWeb;
using SemWeb.Query;

namespace RdfSerialisation
{
public class RdfN3Query<T> : IRdfQuery<T>
{
	public RdfN3Query(Store store)
	{
		this.store = store;
		this.originalType = typeof (T);
		parser = new ExpressionNodeParser<T>();
	}

	private ExpressionNodeParser<T> parser;
	private Expression expression;
	private TextWriter logger = Console.Out;
	private Type originalType = typeof(T);
	private Dictionary<string, string> properties = new Dictionary<string, string>();
	private Delegate project;

	private string query = "";

	private Store store;

	private List<T> result = null;

	public TextWriter Logger
	{
		get { return logger; }
		set { logger = value; }
	}

	public string Query
	{
		get { return query; }
		set { query = value; }
	}

	public Type OriginalType
	{
		get { return originalType; }
		set { originalType = value; }
	}

	public ExpressionNodeParser<T> Parser
	{
		get { return parser; }
		set { parser = value; }
	}

	public Delegate Project
	{
		get { return project; }
		set { project = value; }
	}

	public Dictionary<string, string> Properties
	{
		get { return properties; }
		set { properties = value; }
	}

	public Expression Expression
	{
		get { return System.Expressions.Expression.Constant(this); }
	}

	public Type ElementType
	{
		get { return OriginalType; }
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		RdfN3Query<TElement> newQuery = new RdfN3Query<TElement>(store);
		newQuery.OriginalType = originalType;
		newQuery.Project = project;
		newQuery.Properties = properties;
		newQuery.Query = Query;
		newQuery.Logger = logger;
		newQuery.Parser = new ExpressionNodeParser<TElement>(new StringBuilder(parser.StringBuilder.ToString()));

		MethodCallExpression call = expression as MethodCallExpression;
		if (call != null)
		{
			switch (call.Method.Name)
			{
				case "Where":
					Log("Processing the where expression");
					newQuery.BuildQuery(call.Parameters[1]);
					break;
				case "Select":
					Log("Processing the select expression");
					newQuery.BuildProjection(call);
					break;
			}
		}
		return newQuery;
	}

	public S Execute<S>(Expression expression)
	{
		this.expression = expression;
		throw new NotImplementedException("Execute not implmented");
	}

	///<summary>
	///Returns an enumerator that iterates through a collection.
	///</summary>
	///
	///<returns>
	///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
	///</returns>
	///<filterpriority>2</filterpriority>
	public IEnumerator GetEnumerator()
	{
		string qry = ConstructQuery();
		PrepareQueryAndConnection();
		PresentQuery(qry);
		return (IEnumerator<T>)result;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		return CreateQuery<T>(expression);
	}

	public object Execute(Expression expression)
	{
		return Execute<T>(expression);
	}

	///<summary>
	///Returns an enumerator that iterates through the collection.
	///</summary>
	///<returns>
	///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
	///</returns>
	///<filterpriority>1</filterpriority>
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		if (result != null)
			return result.GetEnumerator();
		query = ConstructQuery();
		PrepareQueryAndConnection();
		PresentQuery(query);
		return result.GetEnumerator();
	}

	private void PresentQuery(string qry)
	{
		Store ms = store;
		ObjectDeserialiserQuerySink sink = new ObjectDeserialiserQuerySink(originalType, typeof(T));
		Query graphMatchQuery = new GraphMatch(new N3Reader(new StringReader(qry)));
		graphMatchQuery.Run(ms, sink);
		result = new List<T>(); 
		foreach(T t in sink.DeserialisedObjects)
		{
			result.Add(t);
		}
	}

	private void PrepareQueryAndConnection()
	{
		// create a ObjectDeserialiserQuerySink and attach it to the store
		string q = string.Format("@prefix m: <{0}> .\n",OwlInstanceSupertype.GetOntologyBaseUri(originalType));
		query = q + query;
		foreach (PropertyInfo pi in typeof(T).GetProperties())
		{
			query += string.Format("?{0} <{1}> ?{2} .\n", originalType.Name, OwlInstanceSupertype.GetPropertyUri(originalType, pi.Name), pi.Name);
		}
	}

	string ConstructQuery()
	{
		return Parser.StringBuilder.ToString();
	}

private void BuildQuery(Expression q)
{
	StringBuilder sb = new StringBuilder();
	ParseQuery(q, sb);
	Query = Parser.StringBuilder.ToString();
	Trace.WriteLine(Query);
}

private void ParseQuery(Expression expression, StringBuilder sb)
{
	sb.Append("#Query - " + DateTime.Now.ToLongTimeString());
	Parser.Dispatch(expression);
}

	/// <summary>
	/// Helper method for projection clauses (Select).
	/// </summary>
	/// <param name="expression">Lambda expression representing the projection.</param>
private void BuildProjection(Expression expression)
{
	LambdaExpression le = ((MethodCallExpression)expression).Parameters[1] as LambdaExpression;
	if (le == null) throw new ApplicationException("Incompatible expression type found when building a projection");
	project = le.Compile();
	MemberInitExpression mie = le.Body as MemberInitExpression;
	if (mie != null)
		foreach (Binding b in mie.Bindings)
			FindProperties(b);
	else
		foreach (PropertyInfo i in originalType.GetProperties())
			properties.Add(i.Name, null);
}

	/// <summary>
	/// Recursive helper method to finds all required properties for projection.
	/// </summary>
	/// <param name="e">Expression to detect property uses for.</param>
	private void FindProperties(Binding e)
	{
		switch(e.BindingType)
		{
			case BindingType.MemberAssignment:
				properties.Add(e.Member.Name, "");
				break;
			case BindingType.MemberListBinding:
				break;
			case BindingType.MemberMemberBinding:
				properties.Add(e.Member.Name, "");
				break;
		}

		//
		// Record member accesses to properties or fields from the entity.
		//

		#region old implementation

		/*
		if (e.NodeType == ExpressionType.MemberAccess)
		{
			MemberExpression me = e as MemberExpression;
			if (me.Member.DeclaringType == originalType)
			{
				OwlPropertyAttribute[] da = me.Member.GetCustomAttributes(typeof(OwlPropertyAttribute), false) as OwlPropertyAttribute[];
				properties.Add(me.Member.Name, null);
			}
		}
		else
		{
			if (e is BinaryExpression)
			{
				BinaryExpression b = e as BinaryExpression;
				FindProperties(b.Left);
				FindProperties(b.Right);
			}
			else if (e is UnaryExpression)
			{
				UnaryExpression u = e as UnaryExpression;
				FindProperties(u.Operand);
			}
			else if (e is ConditionalExpression)
			{
				ConditionalExpression c = e as ConditionalExpression;
				FindProperties(c.IfFalse);
				FindProperties(c.IfTrue);
				FindProperties(c.Test);
			}
			else if (e is InvocationExpression)
			{
				InvocationExpression i = e as InvocationExpression;
				FindProperties(i.Expression);
				foreach (Expression ex in i.Args)
					FindProperties(ex);
			}
			else if (e is LambdaExpression)
			{
				LambdaExpression l = e as LambdaExpression;
				FindProperties(l.Body);
				foreach (Expression ex in l.Parameters)
					FindProperties(ex);
			}
			else if (e is ListInitExpression)
			{
				ListInitExpression li = e as ListInitExpression;
				FindProperties(li.NewExpression);
				foreach (Expression ex in li.Expressions)
					FindProperties(ex);
			}
			else if (e is MemberInitExpression)
			{
				MemberInitExpression mi = e as MemberInitExpression;
				FindProperties(mi.NewExpression);
				foreach (MemberAssignment b in mi.Bindings)
					FindProperties(b.Expression);
			}
			else if (e is MethodCallExpression)
			{
				MethodCallExpression mc = e as MethodCallExpression;
				FindProperties(mc.Object);
				foreach (Expression ex in mc.Parameters)
					FindProperties(ex);
			}
			else if (e is NewExpression)
			{
				NewExpression n = e as NewExpression;
				foreach (Expression ex in n.Args)
					FindProperties(ex);
			}
			else if (e is NewArrayExpression)
			{
				NewArrayExpression na = e as NewArrayExpression;
				foreach (Expression ex in na.Expressions)
					FindProperties(ex);
			}
			else if (e is TypeBinaryExpression)
			{
				TypeBinaryExpression tb = e as TypeBinaryExpression;
				FindProperties(tb.Expression);
			}
	}
*/

		#endregion
	}

	internal void Log(string msg)
	{
		Debug.WriteLine("+ :" + msg);
		//            if (logger != null)
		//                logger.WriteLine(msg);
	}
}
}

