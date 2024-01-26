using System.Xml;

string path = @"D:\a\THUAI7\";
Visit(new DirectoryInfo(path));

void Visit(DirectoryInfo root)
{
    foreach (var file in root.EnumerateFiles())
    {
        if (file.Name.EndsWith("csproj"))
        {
            ChangeFile(file.FullName);
        }
    }
    foreach (var dir in root.EnumerateDirectories())
    {
        Visit(dir);
    }
}

void ChangeFile(string path)
{
    var document = new XmlDocument();
    document.Load(path);
    var es = document.GetElementsByTagName("TargetFrameworks");
    if (es.Count == 2)
    {
        var i0 = es[0];
        var i1 = es[1];
        var text = i1.InnerText;
        i0.InnerText = text.Split(';')[1];
        i0.ParentNode.RemoveChild(i1);
    }
    document.Save(path);
}