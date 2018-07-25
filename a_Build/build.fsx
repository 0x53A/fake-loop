#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.NuGet
nuget Fake.DotNet.Paket
nuget Fake.Tools.Git
nuget Fake.DotNet.Testing.NUnit
//nuget Fake.DotNet.Testing.XUnit
nuget Fake.DotNet.Testing.Expecto
nuget Fake.DotNet.Testing.MSTest
nuget Fake.Core.Target "
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
// there are some issues with FSI and .NET Standard 2.0
// see https://github.com/Microsoft/visualfsharp/issues/5216
#r @"C:\Users\rieluk\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll"
#endif

open System
open System.IO
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.DotNet.Testing
open Fake.Core
open System.Threading
open System.Diagnostics
open System.Runtime.InteropServices
open Fake.Core
open TargetEx

#load "./_targetEx.fsx"


let (@@) a b = Path.combine a b


let tryFindArgValue a =
    let args = System.Environment.GetCommandLineArgs()
    match Array.tryFindIndexBack (fun x -> x = a) args  with
    | None -> None
    | Some i -> if args.Length > i then Some ( args.[i+1] ) else None

// ------------------------------------------------------------------------------------
// Targets
// ------------------------------------------------------------------------------------

let t_targets =
    TargetGroup [
        for i in 0 .. 9 ->
            Target.create (sprintf "Target_%i" i) ignore
    ]
    


// ------------------------------------------------------------------------------------
// Dependencies
// ------------------------------------------------------------------------------------





// -------------------------------------------------
// pipelines

[ t_targets ] |> CreatePipeline "Run.1"
[ t_targets ] |> CreatePipeline "Run.2"
[ t_targets ] |> CreatePipeline "Run.3"
[ t_targets ] |> CreatePipeline "Run.4"



// start build
match tryFindArgValue "--multi-target" with
| Some mts ->
    // synthesize a new pipeline based on the arguments, then run it.
    let targetNames = mts.Split(';', ',') |> Seq.filter (not << String.isNullOrWhiteSpace)
    let targets =
        targetNames
        |> Seq.map (fun t ->
            match all_Pipelines.TryGetValue t with
            | (true, g) -> g
            | (false, _) -> Target t)
        |> Seq.toList

    let _Run = targets |> CreatePipeline "Run"
    Target.runOrDefaultWithArguments "Run"
| None ->
    Target.runOrDefaultWithArguments "Default"
