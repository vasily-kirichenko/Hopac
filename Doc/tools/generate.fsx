// --------------------------------------------------------------------------------------
// Builds the documentation from `.fsx` and `.md` files in the 'docs/content' directory
// (the generated documentation is stored in the 'docs/output' directory)
// --------------------------------------------------------------------------------------

// Binaries that have XML documentation (in a corresponding generated XML file)
let referenceBinaries = [ "Hopac.dll" ]
// Web site location for the generated documentation
let website = "/Hopac"

// Specify more information about your project
let info =
  [ "project-name", "Hopac"
    "project-author", "Housemarque Inc."
    "project-summary", "A library for Higher-Order, Parallel, Asynchronous and Concurrent programming in F#."
    "project-github", "https://github.com/Hopac/Hopac"
    "project-nuget", "https://www.nuget.org/packages/Hopac" ]

// --------------------------------------------------------------------------------------
// For typical project, no changes are needed below
// --------------------------------------------------------------------------------------

#I "../../packages/FSharp.Formatting.2.6.3/lib/net40"
//#I "../../packages/RazorEngine.3.5.1/lib/net40/"
#r @"..\..\packages\FSharp.Compiler.Service.0.0.81\lib\net45\FSharp.Compiler.Service.dll"
#r "../../packages/Microsoft.AspNet.Razor.3.0.0/lib/net45/System.Web.Razor.dll"
#r "../../packages/FAKE/tools/FakeLib.dll"
#r "RazorEngine.dll"
#r "FSharp.Literate.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.MetadataFormat.dll"
//#r "System.Runtime"
//#r "System.Threading.Tasks"
//#r "System.Threading"
//#r @"..\..\bin\Hopac.Core.dll"
open Fake
open System.IO
open Fake.FileHelper
open FSharp.Literate
open FSharp.MetadataFormat

// When called from 'build.fsx', use the public project URL as <root>
// otherwise, use the current 'output' directory.
#if RELEASE
let root = website
#else
let root = "file://" + (__SOURCE_DIRECTORY__ @@ "../output") 
#endif

// Paths with template/source/output locations
let bin        = __SOURCE_DIRECTORY__ @@ "../../bin"
let content    = __SOURCE_DIRECTORY__ @@ "../content"
let output     = __SOURCE_DIRECTORY__ @@ "../output"
let files      = __SOURCE_DIRECTORY__ @@ "../files"
let templates  = __SOURCE_DIRECTORY__ @@ "templates"
let formatting = __SOURCE_DIRECTORY__ @@ "../../packages/FSharp.Formatting.2.6.3/"
let docTemplate = formatting @@ "templates/docpage.cshtml"

// Where to look for *.csproj templates (in this order)
let layoutRoots =
  [ templates; formatting @@ "templates"
    formatting @@ "templates/reference" ]

// Copy static files and CSS + JS from F# Formatting
let copyFiles () =  
  CopyRecursive files output true |> Log "Copying file: "
  ensureDirectory (output @@ "content")
  CopyRecursive (formatting @@ "styles") (output @@ "content") true 
    |> Log "Copying styles and scripts: "

// Build API reference from XML comments
let buildReference () =
  CleanDir (output @@ "reference")
  for lib in referenceBinaries do
    MetadataFormat.Generate
      ( bin @@ lib, 
        output @@ "reference", 
        layoutRoots, 
        parameters = ("root", root)::info,
        //assemblyReferences = ["Hopac.Core.dll"; "System.Runtime.dll"; "System.Threading.dll"; "System.Threading.Tasks.dll"])
        libDirs = [ bin; __SOURCE_DIRECTORY__ ] )

// Build documentation from `fsx` and `md` files in `docs/content`
let buildDocumentation () =
  let subdirs = Directory.EnumerateDirectories(content, "*", SearchOption.AllDirectories)
  for dir in Seq.append [content] subdirs do
    let sub = if dir.Length > content.Length then dir.Substring(content.Length + 1) else "."
    Literate.ProcessDirectory
      ( dir, docTemplate, output @@ sub, replacements = ("root", root)::info,
        layoutRoots = layoutRoots )

// Generate
copyFiles()
buildDocumentation()
//buildReference() 
 