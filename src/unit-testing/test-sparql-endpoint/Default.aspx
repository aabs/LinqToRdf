<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="test_sparql_endpoint._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Demo SPARQL Query Form</title>
</head>
<body>
	<form id="form1" runat="server">
	</form>
	<form action="/linqtordf/SparqlQuery.aspx" method="post">
		<input type="hidden" name="outputMimeType" value="text/xml" />
		<textarea name="query" rows="30" cols="80">
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX xsdt: <http://www.w3.org/2001/XMLSchema#>
PREFIX fn: <http://www.w3.org/2005/xpath-functions#> 
PREFIX a: <http://aabs.purl.org/ontologies/2007/04/music#>

SELECT * 
WHERE {
_:t  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> a:Track;
a:year ?Year ;
a:genreName ?GenreName .
FILTER(
((xsdt:integer(?Year))>(1998))&&(regex(?GenreName, "History 5 | Fall 2006 | UC Berkeley") )
)
}
        </textarea>
		<p>
			<input type="submit" /></p>
	</form>
</body>
</html>
