/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/p/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/p/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
	public class ObjectDeserialiserQuerySink : QueryResultSink
	{
		public IList DeserialisedObjects
		{
			get { return deserialisedObjects; }
		}

		private IList deserialisedObjects = new ArrayList();

		public ObjectDeserialiserQuerySink(Type originalType, Type instanceType)
		{
			this.originalType = originalType;
			this.instanceType = instanceType;
		}

		public Type OriginalType
		{
			get { return originalType; }
			set { originalType = value; }
		}

		private Type originalType;
		private readonly Type instanceType;

		public override bool Add(VariableBindings result)
		{
			if (originalType == null) throw new ApplicationException("need a type to create");
			object t = Activator.CreateInstance(instanceType);

			IEnumerable<PropertyInfo> props;
			if(originalType == instanceType) //  i.e. identity projection, meaning we can use GetAllPersistentProperties safely
			{
				props = OwlClassSupertype.GetAllPersistentProperties(OriginalType);
			}
			else
			{
				props = instanceType.GetProperties();
			}

			foreach (PropertyInfo pi in props)
			{
				try
				{
					if(result[pi.Name] != null)
					{
						string vVal = result[pi.Name].ToString();
						vVal = RemoveEnclosingQuotesOnString(vVal, pi);
						if (IsXsdtEncoded(vVal))
							vVal = DecodeXsdtString(vVal);
						pi.SetValue(t, Convert.ChangeType(vVal, pi.PropertyType), null);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					return false;
				}
			}
			DeserialisedObjects.Add(t);
			return true;
		}

		private string DecodeXsdtString(string val)
		{
			string[] delims = new string[] {"^^"};
			string[] sa = val.Split(delims, StringSplitOptions.None);
			string sValue = sa[0];
			string xsdtType = sa[1];
			if (xsdtType.EndsWith("integer>"))
				return sValue.Substring(1, sValue.Length - 2);
			return sValue;
		}

		private bool IsXsdtEncoded(string val)
		{
			return val.Contains("^^");
		}

		private string RemoveEnclosingQuotesOnString(string val, PropertyInfo pi)
		{
			if(pi.PropertyType == typeof(string))
			{
				if(val.StartsWith("\"") && val.EndsWith("\"") && val.Length > 1)
				{
					return val.Substring(1, val.Length - 2);
				}
			}
			return val;
		}
	}

	public class PrintQuerySink : QueryResultSink
	{
		public override bool Add(VariableBindings result)
		{
			foreach (Variable variable in result.Variables)
			{
				if (variable.LocalName != null && result[variable] != null)
				{
					Console.WriteLine(variable.LocalName + " ==> " + result[variable]);
				}
				Console.WriteLine("\n");
			}
			return true;
		}
	}
}