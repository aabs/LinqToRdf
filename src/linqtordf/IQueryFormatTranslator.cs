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
using System.Expressions;
using System.Reflection;
using System.Text;
using C5;

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

		HashSet<MemberInfo> Parameters
		{
			get;
			set;
		}

		void Dispatch(Expression expression);
	}
}