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
type Cell<'a>
val cell: 'a -> Job<Cell<'a>>
val get: Cell<'a> -> Job<'a>
val put: Cell<'a> -> 'a -> Job<unit>
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
