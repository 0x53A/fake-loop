module TargetEx

#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
// there are some issues with FSI and .NET Standard 2.0
// see https://github.com/Microsoft/visualfsharp/issues/5216
#r @"C:\Users\rieluk\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll"
#endif

open Fake.Core.TargetOperators
open System.Collections.Generic
open Fake.Core
open System


type Target =
| Target of string
| TargetGroup of Target list
    member x.TargetNames = seq {
        match x with
        | Target s -> yield s
        | TargetGroup g -> yield! g |> Seq.collect (fun g -> g.TargetNames)
    }

module Target =
    let create name body =
        Fake.Core.Target.create name body
        Target name



// inspired from https://github.com/fsharp/FAKE/issues/1971

let all_Pipelines = Dictionary<string,Target>(StringComparer.OrdinalIgnoreCase)

let soft = HashSet()
let hard = HashSet()

// set this to TRUE to add duplicated references to FAKE.
let degradeAlgo = true

/// The last target is the name of the pipeline
let private SetPipelineRelations (targets:Target list) : unit =
    let targetNames = targets |> Seq.collect (fun t -> t.TargetNames)
    let last = targetNames |> Seq.last
    all_Pipelines.[last] <- TargetGroup targets
    for (a,b) in targetNames |> Seq.pairwise do
        if soft.Add(a,b) || degradeAlgo then
            a ?=> b |> ignore
        if hard.Add(a,last) || degradeAlgo then
            a ==> last |> ignore

let CreatePipeline name (targets: Target list) : unit =    
    let p = Target.create name ignore
    SetPipelineRelations [ yield (TargetGroup targets); yield p ]
