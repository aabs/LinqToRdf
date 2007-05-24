using System;
using System.Collections.Generic;
using System.Reflection;

namespace linqtordf.core
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
		[Xsdt(false, "int")]
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
	public static class XsdtTypeConverter
	{
		static XsdtTypeConverter()
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
		public static Dictionary<Type, XsdtPrimitiveDataType> TypeLookup
		{
			get { return typeLookup; }
			set { typeLookup = value; }
		}

		static Dictionary<Type, XsdtPrimitiveDataType> typeLookup = new Dictionary<Type, XsdtPrimitiveDataType>();

		public static XsdtPrimitiveDataType GetDataType(Type t)
		{
			if (TypeLookup.ContainsKey(t))
				return TypeLookup[t];
			else return XsdtPrimitiveDataType.XsdtUnknown;
		}

		public static string Get<T>(T obj)
		{
			string result = "";
			XsdtPrimitiveDataType dt = GetDataType(typeof(T));
			XsdtAttribute attr = GetXsdtAttrFor(dt);
			if (dt == XsdtPrimitiveDataType.XsdtUnknown)
			{
				if (attr.IsQuoted)
				{
					return "\"" + obj + "\"";
				}
				else return obj.ToString();
			}

			if (attr.IsQuoted)
			{
				result = "\"" + GetStringRepresentationFor(obj, dt, attr) + "\"";
			}

			return result + "^^" + attr.Name;
		}
		public static string Get(Type t, object obj)
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

			if (t == typeof(DateTime))
				result = GetXsdtDateRepresentationFor((DateTime)obj, dt, attr);
			else
				result = GetStringRepresentationFor(obj, dt, attr);
			if (attr.IsQuoted)
			{
				result = "\"" + result + "\"";
			}
			return result + "^^xsdt:" + attr.Name;

		}

		private static string GetXsdtDateRepresentationFor(DateTime d, XsdtPrimitiveDataType dt, XsdtAttribute attr)
		{
			// TODO: the time zone offset needs to be returned from somewhere...
			return d.ToString("yyyy-MM-ddTHH:mm:sszzz");
		}

		private static string GetStringRepresentationFor<T>(T obj, XsdtPrimitiveDataType dt, XsdtAttribute attr)
		{
			return obj.ToString();
		}

		private static XsdtAttribute GetXsdtAttrFor(XsdtPrimitiveDataType dt)
		{
			FieldInfo fi = dt.GetType().GetField(dt.ToString());
			XsdtAttribute[] attrs = (XsdtAttribute[])fi.GetCustomAttributes(typeof(XsdtAttribute), false);
			return attrs[0]; //  I know by design that there will be one and only one attribute in this array.
		}
	}
}
