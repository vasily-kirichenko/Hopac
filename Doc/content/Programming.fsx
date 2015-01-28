(*** hide ***)
#I "../../bin"
#r "Hopac.Core.dll"
#r "Hopac.dll"
open Hopac
open Hopac.Infixes
open Hopac.Job.Infixes
open Hopac.Alt.Infixes
(**
### Example: Updatable Storage Cells

In the book
[Concurrent Programming in ML](http://www.cambridge.org/us/academic/subjects/computer-science/distributed-networked-and-mobile-computing/concurrent-programming-ml),
[John Reppy](http://people.cs.uchicago.edu/~jhr/) presents as the first
programming example an implementation of updatable storage cells using
Concurrent ML channels and threads.  While this example is not exactly something
that one would do in practice, because F# already provides ref cells, it does a
fairly nice job of illustrating some core aspects of Concurrent ML.  So, let's
reproduce the same example with Hopac.

Here is the signature for our updatable storage cells:
*)

sprintf "string"
sprintf "string %s" 1
System.Console.WriteLine("string")
let a, b = sprintf "dddd", 1

module Native = 
    open System.Threading
    open System.Diagnostics
    
    let run n = 
        printf "Native: "
        let timer = Stopwatch.StartNew()
        let selfCh = new AutoResetEvent(false)
        
        let rec proc n (selfCh: AutoResetEvent) (toCh: AutoResetEvent) = 
            if n = 0 then toCh.Set() |> ignore
            else 
                let childCh = new AutoResetEvent(false)
                let child = Thread(ThreadStart(fun () -> proc (n - 1) childCh toCh), 512)
                child.Start()
                childCh.Set() |> ignore
            selfCh.WaitOne() |> ignore
            selfCh.Dispose()
        proc n selfCh selfCh
        let d = timer.Elapsed
        printf "%9.0f ops/s - %fs\n" (float n / d.TotalSeconds) d.TotalSeconds
        let mutable mutableVar = Some 1
        match mutableVar with
        | Some _ -> ()
        | None -> ()

     
(**
The `cell` function creates a
job[*](http://hopac.github.io/Hopac/Hopac.html#def:type%20Hopac.Job)
that creates a new storage cell.  The `get` function creates a job that returns
the contents of the cell and the `put` function creates a job that updates the
contents of the cell.

The basic idea behind the implementation is that the cell is a concurrent
*server* that responds to `Get` and `Put` request.  We represent the requests
using the `Request` discriminated union type:
*)
type Request<'a> =
 | Get
 | Put of 'a
