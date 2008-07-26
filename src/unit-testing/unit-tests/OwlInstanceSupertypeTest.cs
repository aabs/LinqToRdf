﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using LinqToRdf;
namespace UnitTests
{
	[TestFixture]
	public class OwlInstanceSupertypeTest
	{

		internal class MyTestFixture
		{
			private int myTestField;

			public int MyTestField
			{
				get { return myTestField; }
				set { myTestField = value; }
			}
		}

		[Test]
		public void InstanceUriTest()
		{
			OwlInstanceSupertype target = new OwlInstanceSupertype();

			string val = "http://tempuri.net/uri"; // TODO: Assign to an appropriate value for the property

			target.InstanceUri = val;


			Assert.AreEqual(val, target.InstanceUri, "LinqToRdf.OwlInstanceSupertype.InstanceUri was not set correctly.");
		}

	}


}
