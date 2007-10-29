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
[assembly: AssemblyTitle(@"LinqToRdfDesigner")]
[assembly: AssemblyDescription(@"A domain specific language for the production of N3 and C# files compatible with LinqToRdf")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"Andrew Matthews")]
[assembly: AssemblyProduct(@"LinqToRdfDesigner")]
[assembly: AssemblyCopyright("Copyright 2007 Andrew Matthews")]
[assembly: AssemblyTrademark("LinqToRdf")]
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

[assembly: AssemblyVersion(@"0.4.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"LinqToRdf.Design.DslPackage, PublicKey=0024000004800000940000000602000000240000525341310004000001000100B51ADC18E8783552B646BC4A086614880209385AF1AE595A2F3770F5A55F23E2DD7ADF0346AE7298062971AB2EAB0336954C91A9D992CC1C3465A6C0A84F40B4F6836A885D42AD6DC117B546719E0EF085EA20DC217CB4408CEA2B474E0EE6F42B577AD6D36AFD2FACADDAA1546F9297862EB64ED657D70F9904932C3B8CE486")]