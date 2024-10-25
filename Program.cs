using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

string folder = "C:\\";

// Get all C# files in the folder
string[] csharpFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);

// Create an empty PlantUML script
string plantuml = "@startuml\n";

// Loop through each C# file
foreach (string file in csharpFiles)
{
  // Read the C# file content
  string code = File.ReadAllText(file);

  // Parse the C# code
  SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
  CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

  // Find all namespaces
  IEnumerable<NamespaceDeclarationSyntax> namespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
  foreach (NamespaceDeclarationSyntax namespaceDecl in namespaces)
  {
    string namespaceName = namespaceDecl.Name.ToString();
    plantuml += $"namespace {namespaceName} {{\n";

    // Find all classes within the namespace
    IEnumerable<ClassDeclarationSyntax> classes = namespaceDecl.Members.OfType<ClassDeclarationSyntax>();
    foreach (ClassDeclarationSyntax classDecl in classes)
    {
      string className = classDecl.Identifier.Text;
      plantuml += $"class {className} {{\n";


      IEnumerable<PropertyDeclarationSyntax> properties = classDecl.Members.OfType<PropertyDeclarationSyntax>();
      foreach (PropertyDeclarationSyntax propertyDecl in properties)
      {
        string propertyName = propertyDecl.Identifier.Text;
        string propertyType = propertyDecl.Type.ToString();
        plantuml += $"  {propertyType} {propertyName}\n";
      }

      // Find all fields within the class
      IEnumerable<FieldDeclarationSyntax> fields = classDecl.Members.OfType<FieldDeclarationSyntax>();
      foreach (FieldDeclarationSyntax fieldDecl in fields)
      {
        string fieldName = fieldDecl.Declaration.Variables.First().Identifier.Text;
        string fieldType = fieldDecl.Declaration.Type.ToString();
        plantuml += $"  {fieldType} {fieldName}\n";
      }

      // Find all methods within the class (updated to include return types and parameters)
      IEnumerable<MethodDeclarationSyntax> methods = classDecl.Members.OfType<MethodDeclarationSyntax>();
      foreach (MethodDeclarationSyntax methodDecl in methods)
      {
        string methodName = methodDecl.Identifier.Text;
        string returnType = methodDecl.ReturnType.ToString();

        // Get method parameters
        string parameters = string.Join(", ", methodDecl.ParameterList.Parameters.Select(p => $"{p.Type} {p.Identifier}"));

        plantuml += $"  {returnType} {methodName}({parameters})\n";
      }

      plantuml += "}\n";
    }

    plantuml += "}\n";
  }
}

// Output the PlantUML script to a file
File.WriteAllText("classdiagram.puml", plantuml);

Console.WriteLine("PlantUML diagram generated: classdiagram.puml");
