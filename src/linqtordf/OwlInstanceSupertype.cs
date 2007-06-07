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
using System.Diagnostics;
using System.Reflection;
using SemWeb;

namespace LinqToRdf
{
    public class OwlInstanceSupertype : OwlClassSupertype
    {
        public string InstanceUri
        {
            get { return instanceUri; }
            set { instanceUri = value; }
        }

        private string instanceUri;
    }
}