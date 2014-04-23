<Query Kind="Statements">
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <Namespace>System.Net</Namespace>
  <Namespace>HtmlAgilityPack</Namespace>
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
		Prefix = match.Groups["prefix"].Value,
		Number = match.Groups["number"].Value,
		Title = match.Groups["title"].Value,
		Credits = match.Groups["credits"].Value,
		Semester = match.Groups["semester"].Value,
		AdditionalInfo = match.Groups["info"] != null ? match.Groups["info"].Value : null,
		Description = match.Groups["description"].Value,
		Requisites = Regex.Matches(match.Groups["requisites"].Value, @"(\w+ \d{3}\w{0,1})").Cast<Match>().Select(m => m.Value)
	};
	
classes.Dump();