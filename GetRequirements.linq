<Query Kind="Statements">
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET Web Pages\v2.0\Assemblies\System.Web.Helpers.dll</Reference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Web.Helpers</Namespace>
</Query>

var html = new WebClient().DownloadString("http://registrar.boisestate.edu/undergraduate/course-catalog/cs/");
var doc = new HtmlDocument();
doc.LoadHtml(html);

var content = doc.DocumentNode.SelectSingleNode("//div[@class='entry-content']");
string pattern = @"(?<prefix>\w+) (?<number>\d{3}\w{0,1}) (?<title>.*?) \((?<credits>.*?)\)\((?<semester>.*?)\)(\((?<info>.*?)\)){0,1}\. (?<description>.*?) (PREREQ|COREQ|PRE/COREQ): (?<requisites>.*)";
var classes = from c in content.Descendants("p")
	let text = c.InnerText.Trim()
	where Regex.IsMatch(text, "^CS")
	let match = Regex.Match(text, pattern)
	select new 
	{
		id = match.Groups["prefix"].Value + " " + match.Groups["number"].Value,
		prefix = match.Groups["prefix"].Value,
		number = match.Groups["number"].Value,
		title = match.Groups["title"].Value,
		credits = match.Groups["credits"].Value,
		semester = match.Groups["semester"].Value,
		info = match.Groups["info"] != null ? match.Groups["info"].Value : null,
		description = match.Groups["description"].Value,
		requisites = Regex.Matches(match.Groups["requisites"].Value, @"(\w+ \d{3}\w{0,1})").Cast<Match>().Select(m => m.Value)
	};

Console.WriteLine(Json.Encode(classes));
	
classes.Dump();