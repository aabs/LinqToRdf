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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
    public class ObjectDeserialiserQuerySink : QueryResultSink
    {
        private readonly IList deserialisedObjects = new ArrayList();
        private readonly bool elideDuplicates;
        private readonly Type instanceType;
        private Type originalType;

        public ObjectDeserialiserQuerySink(
            Type originalType,
            Type instanceType,
            string instanceName,
            bool elideDuplicates,
            MethodCallExpression selectExpression,
            RdfDataContext context)
        {
            SelectExpression = selectExpression;
            this.originalType = originalType;
            this.instanceType = instanceType;
            InstanceName = instanceName;
            this.elideDuplicates = elideDuplicates;
            DataContext = context;
        }

        public IList DeserialisedObjects
        {
            get { return deserialisedObjects; }
        }

        private MethodCallExpression SelectExpression { get; set; }

        public Type OriginalType
        {
            get { return originalType; }
            set { originalType = value; }
        }

        private string InstanceName { get; set; }

        private RdfDataContext DataContext { get; set; }

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
            if (originalType == instanceType)
                //  i.e. identity projection, meaning we can use GetAllPersistentProperties safely
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
                AssignDataContextToOwlInstanceType(t as OwlInstanceSupertype, DataContext);
                AssignInstanceUriToOwlInstanceType(t as OwlInstanceSupertype, InstanceName, result);
                foreach (PropertyInfo pi in props)
                {
                    if (pi.PropertyType.IsGenericType &&
                        pi.PropertyType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
                        continue;
                    try
                    {
                        if (result[pi.Name] != null)
                        {
                            string vVal = result[pi.Name].ToString();
                            var tc = new XsdtTypeConverter();
                            object x = tc.Parse(vVal);
//							vVal = RemoveEnclosingQuotesOnString(vVal, pi);
//							if (IsXsdtEncoded(vVal))
//								vVal = DecodeXsdtString(vVal);
                            if (x is IConvertible)
                                pi.SetValue(t, Convert.ChangeType(x, pi.PropertyType), null);

                            // if it's not convertible, it could be because the type is an MS XSDT type rather than a .NET primitive 
                            if (x.GetType().Namespace == "System.Runtime.Remoting.Metadata.W3cXsd2001")
                            {
                                switch (x.GetType().Name)
                                {
                                    case "SoapDate":
                                        var d = (SoapDate) x;
                                        pi.SetValue(t, Convert.ChangeType(d.Value, pi.PropertyType), null);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    catch (ArgumentException ae)
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
                var args = new List<object>();
                foreach (PropertyInfo pi in props)
                {
                    try
                    {
                        if (result[pi.Name] != null)
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

        private void AssignInstanceUriToOwlInstanceType(OwlInstanceSupertype inst, string instanceName,
                                                        VariableBindings bindings)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                return;
            }
            if (bindings.Variables.Map(v => v.LocalName).Contains(instanceName))
            {
                string x = bindings[instanceName].ToString();
                if (x.StartsWith("<") && x.EndsWith(">"))
                {
                    x = x.Substring(1, x.Length - 2);
                }
                if (Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute))
                {
                    inst.InstanceUri = x;
                }
            }
        }

        /// <summary>
        /// Assigns the data context to the instance in case it needs it to lazily load references later on.
        /// </summary>
        /// <param name="inst">the object that has just been deserialised.</param>
        /// <param name="context">The context through which the query was run that led to the instance being deserialised.</param>
        private void AssignDataContextToOwlInstanceType(OwlInstanceSupertype inst, RdfDataContext context)
        {
            if (inst != null)
            {
                inst.DataContext = DataContext;
            }
        }

        private bool ObjectIsUniqueSoFar(object t)
        {
            // 1. get the instance URI for t
            var oo = t as OwlInstanceSupertype;
            string instanceUri = oo.InstanceUri;

            // 2. check other instance in DeserializedObjects for duplicate instance URIs
            foreach (OwlInstanceSupertype o in deserialisedObjects)
            {
                if (o.InstanceUri == oo.InstanceUri)
                    return false;
            }
            return true; // default placeholder impl
        }

        private string DecodeXsdtString(string val)
        {
            var delims = new[] {"^^"};
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
            if (pi.PropertyType == typeof (string) || pi.PropertyType == typeof (TimeSpan))
            {
                if (val.StartsWith("\"") && val.EndsWith("\"") && val.Length > 1)
                {
                    return val.Substring(1, val.Length - 2);
                }
            }
            return val;
        }

        private bool IsSelectMember(MethodCallExpression e)
        {
            if (e == null)
            {
                return false;
            }
            var ue = e.Arguments[1] as UnaryExpression;
            var le = (LambdaExpression) ue.Operand;
            return le.Body is MemberExpression;
        }

        private object ExtractMemberAccess(VariableBindings vb)
        {
            // create function to create the storage object
            var ue = (SelectExpression).Arguments[1] as UnaryExpression;
            var le = (LambdaExpression) ue.Operand;
            if (le == null)
                throw new ApplicationException("Incompatible expression type found when building ontology projection");
            if (le.Body is MemberExpression)
            {
                #region member expression

                var memex = (MemberExpression) le.Body;
                MemberInfo mi = memex.Member;
                Type memType = null;
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        var fi = mi as FieldInfo;
                        memType = fi.FieldType;
                        break;
                    case MemberTypes.Property:
                        var pi = mi as PropertyInfo;
                        memType = pi.PropertyType;
                        break;
                    default:
                        break;
                }

                string vVal = vb[mi.Name].ToString();
                var tc = new XsdtTypeConverter();
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