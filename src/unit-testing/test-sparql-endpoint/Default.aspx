<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="test_sparql_endpoint._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<body>
	<form id="form1" runat="server">
	</form>
	<form action="SparqlQuery.aspx" method="post">
		<input type="hidden" name="outputMimeType" value="text/xml" />
		<textarea name="query" rows="10" cols="80">
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX xsdt: <http://www.w3.org/2001/XMLSchema#>
PREFIX fn: <http://www.w3.org/2005/xpath-functions#> 
PREFIX a: <http://aabs.purl.org/ontologies/2007/04/music#>

SELECT ?Title ?FileLocation 
WHERE {
_:t a:year ?Year ;
a:genreName ?GenreName ;
a:title ?Title ;
a:fileLocation ?FileLocation .
FILTER((regex(?Year, "2007"))&&(regex(?GenreName, "Rory Blyth: The Smartest Man in the World") ))
}
        </textarea>
		<p>
			<input type="submit" /></p>
	</form>
</body>
</html>
