#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"Andrew Matthews")]
[assembly: AssemblyProduct(@"LinqToRdf")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"LinqToRdf.Design.DslPackage, PublicKey=00240000048000009400000006020000002400005253413100040000010001009FAE5B063681F5607E8E3E815C1C13075011A4B0AE7A0B01F77FA65EA3F8E6C3A0E6670536BD88409B22C4684BC6BF542E732FC935DAD743A049C99DBAB3F927E0B5062C065A8D3802C554C008B2ED89BC0E26943D24B393D80BBBA9955EC714E27DF80F9A9016B806C0D6231A099E2BBE010C44051548683AFDBA195748D7A9")]