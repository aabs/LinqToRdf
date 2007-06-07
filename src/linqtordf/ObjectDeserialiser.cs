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

//			foreach (PropertyInfo pi in OwlClassSupertype.GetAllPersistentProperties(OriginalType))
			foreach (PropertyInfo pi in instanceType.GetProperties())
			{
				try
				{
					string vn = OwlClassSupertype.GetPropertyUri(OriginalType, pi.Name).Split('#')[1];
					string vVal = result[pi.Name].ToString();
					pi.SetValue(t, Convert.ChangeType(vVal, pi.PropertyType), null);
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