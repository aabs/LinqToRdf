<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="test_sparql_endpoint._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Demo SPARQL Query Form</title>
</head>
<body>
	<form id="form1" runat="server">
	</form>
	<form action="http://localhost/linqtordf/SparqlQuery.aspx" method="post">
		<input type="hidden" name="outputMimeType" value="text/xml" />
		<textarea name="query" rows="30" cols="80">
PREFIX owl:  <http://www.w3.org/2002/07/owl#>

SELECT ?u
WHERE
	{
	?u a owl:Class .
	}
    
        </textarea>
		<p>
			<input type="submit" /></p>
	</form>
</body>
</html>
