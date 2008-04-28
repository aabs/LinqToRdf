/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/fromName/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/fromName/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using SemWeb;
using SemWeb.Query;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Linq;

namespace LinqToRdf
{
	public class ObjectDeserialiserQuerySink : QueryResultSink
	{
		public IList DeserialisedObjects
		{
			get { return deserialisedObjects; }
		}
        MethodCallExpression SelectExpression { get; set; }
		private IList deserialisedObjects = new ArrayList();

        public ObjectDeserialiserQuerySink(Type originalType, Type instanceType, bool elideDuplicates, MethodCallExpression selectExpression)
		{
            SelectExpression = selectExpression;
            this.originalType = originalType;
			this.instanceType = instanceType;
		    this.elideDuplicates = elideDuplicates;
		}

		public Type OriginalType
		{
			get { return originalType; }
			set { originalType = value; }
		}

		private Type originalType;
		private readonly Type instanceType;
	    private readonly bool elideDuplicates;

	    public override bool Add(VariableBindings result)
		{
	        if (IsSelectMember(SelectExpression))
	        {
	            deserialisedObjects.Add(ExtractMemberAccess(result));
	            return true;
	        }

			if (originalType == null) throw new ApplicationException("need ontology type to create");
			object t;

			IEnumerable<MemberInfo> props;
			if(originalType == instanceType) //  i.e. identity projection, meaning we can use GetAllPersistentProperties safely
			{
				props = OwlClassSupertype.GetAllPersistentProperties(OriginalType);
			}
			else
			{
				props = instanceType.GetProperties();
			}
			if (originalType == instanceType)
			{
				t = Activator.CreateInstance(instanceType);
				foreach (PropertyInfo pi in props)
				{
					if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
						continue;
					try
					{
						if(result[pi.Name] != null)
						{
							string vVal = result[pi.Name].ToString();
							XsdtTypeConverter tc = new XsdtTypeConverter();
							var x = tc.Parse(vVal);
//							vVal = RemoveEnclosingQuotesOnString(vVal, pi);
//							if (IsXsdtEncoded(vVal))
//								vVal = DecodeXsdtString(vVal);
							if(x is IConvertible)
								pi.SetValue(t, Convert.ChangeType(x, pi.PropertyType), null);

							// if it's not convertible, it could be because the type is an MS XSDT type rather than a .NET primitive 
							if(x.GetType().Namespace == "System.Runtime.Remoting.Metadata.W3cXsd2001")
							{
								switch(x.GetType().Name)
								{
									case "SoapDate":
										SoapDate d = (SoapDate)x;
										pi.SetValue(t, Convert.ChangeType(d.Value, pi.PropertyType), null);
										break;
									default:
										break;
										
								}
							}
						}
					}
                    catch (System.ArgumentException ae)
                    {
                        // required variable is NotFiniteNumberException present
                        // we cannot fill the ValueType we're after.
                        Console.WriteLine(ae);
                    }
					catch (Exception e)
					{
						Console.WriteLine(e);
						return false;
					}
				}
			}
			else
			{
				List<object> args = new List<object>();
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
							args.Add(Convert.ChangeType(vVal, pi.PropertyType));
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						return false;
					}
				}
				t = Activator.CreateInstance(instanceType, args.ToArray());
			}
// NBB at the moment we have no way to know whether there is a duplicate instance in the results, because there is no return of the instance URI
// once that has been returned then it can be used to compare instance in the results collection to determine whether to save the current result.
//            if(elideDuplicates)
//            {
//                if(ObjectIsUniqueSoFar(t))
//			        DeserialisedObjects.Add(t);
//            }
//            else
		        DeserialisedObjects.Add(t);
			return true;
		}

        private bool ObjectIsUniqueSoFar(object t)
        {
            // 1. get the instance URI for t
            OwlInstanceSupertype oo = t as OwlInstanceSupertype;
            string instanceUri = oo.InstanceUri;

            // 2. check other instance in DeserializedObjects for duplicate instance URIs
            foreach (OwlInstanceSupertype o in deserialisedObjects)
            {
                if(o.InstanceUri == oo.InstanceUri)
                    return false;
            }
            return true; // default placeholder impl
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
			if(pi.PropertyType == typeof(string) || pi.PropertyType == typeof(TimeSpan))
			{
				if(val.StartsWith("\"") && val.EndsWith("\"") && val.Length > 1)
				{
					return val.Substring(1, val.Length - 2);
				}
			}
			return val;
		}

        bool IsSelectMember(MethodCallExpression e)
        {
            if (e == null)
            {
                return false;
            }
            UnaryExpression ue = e.Arguments[1] as UnaryExpression;
            LambdaExpression le = (LambdaExpression)ue.Operand;
            return le.Body is MemberExpression;
        }
        object ExtractMemberAccess(VariableBindings vb)
        {
            // create function to create the storage object
            UnaryExpression ue = ((MethodCallExpression)SelectExpression).Arguments[1] as UnaryExpression;
            LambdaExpression le = (LambdaExpression)ue.Operand;
            if (le == null) throw new ApplicationException("Incompatible expression type found when building ontology projection");
            if (le.Body is MemberExpression)
            {
                #region member expression
                MemberExpression memex = (MemberExpression)le.Body;
                MemberInfo mi = memex.Member;
                Type memType = null;
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fi = mi as FieldInfo;
                        memType = fi.FieldType;
                        break;
                    case MemberTypes.Property:
                        PropertyInfo pi = mi as PropertyInfo;
                        memType = pi.PropertyType;
                        break;
                    default:
                        break;
                }

                string vVal = vb[mi.Name].ToString();
                XsdtTypeConverter tc = new XsdtTypeConverter();
                return tc.Parse(vVal);
                #endregion
            }
            return null;
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