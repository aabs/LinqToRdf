using System;
using LinqToRdf;
using System.Data.Linq;


[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Item", true)]
public partial class Item : OwlInstanceSupertype
{
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Title", true)]
public partial class Title : OwlInstanceSupertype
{
		[OwlProperty("name", true)]
		public string Name{get;set;}
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Book", true)]
public partial class Book : Item
{
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Member", true)]
public partial class Member : OwlInstanceSupertype
{
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Library", true)]
public partial class Library : OwlInstanceSupertype
{
	EntitySet<Title> catalog { get; set; }
	EntitySet<Item> stock { get; set; }
	EntitySet<Member> membership { get; set; }
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Loan", true)]
public partial class Loan : OwlInstanceSupertype
{
		[OwlProperty("commenced", true)]
		public string Commenced{get;set;}
}

[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
[OwlClass("Reservation", true)]
public partial class Reservation : OwlInstanceSupertype
{
		[OwlProperty("made", true)]
		public string Made{get;set;}
}
