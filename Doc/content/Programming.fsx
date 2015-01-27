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

module HopacReq =
  type Request<'a> =
   | Get
   | Put of 'a

  type Cell<'a> = {
    reqCh: Ch<Request<'a>>
    replyCh: Ch<'a>
  }
   
  let put (c: Cell<'a>) (x: 'a) =
    c.reqCh <-- Put x

  let get (c: Cell<'a>) : Job<'a> = c.reqCh <-+ Get >>. c.replyCh

  let cell (x: 'a) : Job<Cell<'a>> = Job.delay <| fun () ->
    let c = {reqCh = ch (); replyCh = ch ()}
    Job.iterateServer x (fun x ->
     c.reqCh >>= function
      | Get   -> c.replyCh <-+ x >>% x
      | Put x -> Job.result x) >>% c

  let run nCells nJobs nUpdates =
    printf "HopacReq: "
    let timer = Stopwatch.StartNew ()
    let cells = Array.zeroCreate nCells
    let before = GC.GetTotalMemory true
    run <| job {
      do! Job.forUpTo 0 (nCells-1) <| fun i ->
            cell i |>> fun cell -> cells.[i] <- cell
      do printf "%4d b/c " (max 0L (GC.GetTotalMemory true - before) / int64 nCells)
      return!
        seq {1 .. nJobs}
        |> Seq.map (fun _ ->
           let rnd = Random ()
           Job.forUpTo 1 nUpdates <| fun _ ->
             let c = rnd.Next (0, nCells)
             get cells.[c] >>= fun x ->
             put cells.[c] (x+1))
        |> Job.conIgnore
    }
    let d = timer.Elapsed
    for i=0 to nCells-1 do
      cells.[i] <- Unchecked.defaultof<_>
    printf "%8.5f s to %d c * %d p * %d u\n"
     d.TotalSeconds nCells nJobs nUpdates

     
//type Cell<'a>
//val cell: 'a -> Job<Cell<'a>>
//val get: Cell<'a> -> Job<'a>
//val put: Cell<'a> -> 'a -> Job<unit>
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
