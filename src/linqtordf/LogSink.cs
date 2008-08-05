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
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
    public class LogSink : QueryResultSink
    {
        protected Logger Logger  = new Logger(typeof(LogSink));
        public override bool Add(VariableBindings result)
        {
            foreach (Variable variable in result.Variables)
            {
                if (variable.LocalName != null && result[variable] != null)
                {
                    Logger.Debug("?{0} => <{1}>",variable.LocalName, result[variable]);
                }
            }
            return true;
        }
    }
}