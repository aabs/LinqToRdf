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
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinqToRdf
{
	[AttributeUsage(AttributeTargets.Field)]
	public class XsdtAttribute : Attribute
	{
		public XsdtAttribute(bool isQuoted, string name)
		{
			this.isQuoted = isQuoted;
			this.name = name;
		}

		private readonly bool isQuoted;
		private readonly string name;

		public string Name
		{
			get { return name; }
		}

		public bool IsQuoted
		{
			get { return isQuoted; }
		}
	}

	public enum XsdtPrimitiveDataType : int
	{
		[Xsdt(true, "string")]
		XsdtString,
		[Xsdt(false, "boolean")]
		XsdtBoolean,
		[Xsdt(false, "short")]
		XsdtShort,
		[Xsdt(false, "integer")]
		XsdtInt,
		[Xsdt(false, "long")]
		XsdtLong,
		[Xsdt(false, "float")]
		XsdtFloat,
		[Xsdt(false, "double")]
		XsdtDouble,
		[Xsdt(false, "decimal")]
		XsdtDecimal,
		[Xsdt(true, "duration")]
		XsdtDuration,
		[Xsdt(true, "dateTime")]
		XsdtDateTime,
		[Xsdt(true, "time")]
		XsdtTime,
		[Xsdt(true, "date")]
		XsdtDate,
		[Xsdt(true, "gYearMonth")]
		XsdtGYearMonth,
		[Xsdt(true, "gYear")]
		XsdtGYear,
		[Xsdt(true, "gMonthDay")]
		XsdtGMonthDay,
		[Xsdt(true, "gDay")]
		XsdtGDay,
		[Xsdt(true, "gMonth")]
		XsdtGMonth,
		[Xsdt(true, "hexBinary")]
		XsdtHexBinary,
		[Xsdt(true, "base64Binary")]
		XsdtBase64Binary,
		[Xsdt(false, "anyUri")]
		XsdtAnyUri,
		[Xsdt(true, "QName")]
		XsdtQName,
		[Xsdt(true, "Notation")]
		XsdtNotation,
		[Xsdt(false, "")]
		XsdtUnknown
	}
	public interface ITypeTranslator
	{
		XsdtPrimitiveDataType GetDataType(Type t);
		object Get<T>(T obj);
		object Get(Type t, object obj);
	}
	public class XsdtTypeConverter : ITypeTranslator
	{
		public XsdtTypeConverter()
		{
			typeLookup.Add(typeof(string), XsdtPrimitiveDataType.XsdtString);
			typeLookup.Add(typeof(Char), XsdtPrimitiveDataType.XsdtString);
			typeLookup.Add(typeof(Boolean), XsdtPrimitiveDataType.XsdtBoolean);
			typeLookup.Add(typeof(Single), XsdtPrimitiveDataType.XsdtFloat);
			typeLookup.Add(typeof(Double), XsdtPrimitiveDataType.XsdtDouble);
			typeLookup.Add(typeof(Decimal), XsdtPrimitiveDataType.XsdtDecimal);
			typeLookup.Add(typeof(TimeSpan), XsdtPrimitiveDataType.XsdtDuration);
			typeLookup.Add(typeof(Int16), XsdtPrimitiveDataType.XsdtShort);
			typeLookup.Add(typeof(Int32), XsdtPrimitiveDataType.XsdtInt);
			typeLookup.Add(typeof(Int64), XsdtPrimitiveDataType.XsdtLong);
			typeLookup.Add(typeof(Byte[]), XsdtPrimitiveDataType.XsdtHexBinary);
			typeLookup.Add(typeof(Uri), XsdtPrimitiveDataType.XsdtAnyUri);
			typeLookup.Add(typeof(DateTime), XsdtPrimitiveDataType.XsdtDateTime);
		}
		public  Dictionary<Type, XsdtPrimitiveDataType> TypeLookup
		{
			get { return typeLookup; }
			set { typeLookup = value; }
		}

		 Dictionary<Type, XsdtPrimitiveDataType> typeLookup = new Dictionary<Type, XsdtPrimitiveDataType>();

		public  XsdtPrimitiveDataType GetDataType(Type t)
		{
			if (TypeLookup.ContainsKey(t))
				return TypeLookup[t];
			else return XsdtPrimitiveDataType.XsdtUnknown;
		}

		public  object Get<T>(T obj)
		{
			return Get(typeof (T), obj);
		}
		public  object Get(Type t, object obj)
		{
			string result = "";
			XsdtPrimitiveDataType dt = GetDataType(t);
			XsdtAttribute attr = GetXsdtAttrFor(dt);
			if (dt == XsdtPrimitiveDataType.XsdtUnknown)
			{
				if (attr.IsQuoted)
				{
					return "\"" + obj + "\"";
				}
				else return obj.ToString();
			}
            switch (t.FullName)
            {
                case "System.DateTime":
				    result = GetXsdtDateRepresentationFor((DateTime)obj, dt, attr);
                    break;
                case "System.Byte[]":
                    result = Encoding.ASCII.GetString((Byte[])obj);
                    break;
                default:
				    result = GetStringRepresentationFor(obj, dt, attr);
                    break;
            }

            if (attr.IsQuoted)
			{
				result = "\"" + result + "\"";
			}
			string xsdTypeSuffix;
			if (dt != XsdtPrimitiveDataType.XsdtInt && dt != XsdtPrimitiveDataType.XsdtString)
				xsdTypeSuffix = "^^xsdt:" + attr.Name;
			else
				xsdTypeSuffix = "";
			return result + xsdTypeSuffix;
		}

		private  string GetXsdtDateRepresentationFor(DateTime d, XsdtPrimitiveDataType dt, XsdtAttribute attr)
		{
			// TODO: the time zone offset needs to be returned from somewhere...
			return d.ToString("yyyy-MM-ddTHH:mm:sszzz");
		}

		private  string GetStringRepresentationFor<T>(T obj, XsdtPrimitiveDataType dt, XsdtAttribute attr)
		{
			return obj.ToString();
		}

        internal XsdtAttribute GetXsdtAttrFor(Type t)
        {
            return GetXsdtAttrFor(GetDataType(t));
        }

		internal  XsdtAttribute GetXsdtAttrFor(XsdtPrimitiveDataType dt)
		{
			FieldInfo fi = dt.GetType().GetField(dt.ToString());
			XsdtAttribute[] attrs = (XsdtAttribute[])fi.GetCustomAttributes(typeof(XsdtAttribute), false);
			return attrs[0]; //  I know by design that there will be one and only one attribute in this array.
		}
	}
}
