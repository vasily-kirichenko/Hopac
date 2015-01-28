(*** hide ***)
#I "../../bin"

(**
Hopac: Higher-Order, Parallel, Asynchronous and Concurrent
==========================================================

Hopac is a library for F# with the aim of making it easier to write correct,
modular and efficient parallel, asynchronous and concurrent programs.  The
design of Hopac draws inspiration from languages such as
[Concurrent ML](http://cml.cs.uchicago.edu/) and
[Cilk](http://en.wikipedia.org/wiki/Cilk).  Similar to Concurrent ML, Hopac
provides message passing primitives and supports the construction of first-class
synchronous abstractions.  Parallel jobs (lightweight threads) in Hopac are
created using techniques similar to the F# Async framework.  Similar to Cilk,
Hopac runs parallel jobs using a work distributing scheduler in a non-preemptive
fashion.

* [Programming in Hopac](Programming.html)
* [Actors and Hopac](Actors.html)

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The library can be <a href="https://nuget.org/packages/Hopac">installed from NuGet</a>:
      <pre>PM> Install-Package Hopac</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

This example demonstrates the use of Hopac:

*)
// reference Hopac.dll
#r "Hopac.dll"
open Hopac

(**

Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read [library design notes][readme] to understand how it works.

Copyright (c) 2013-2014 Housemarque Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

For more information see the [License file][license] in the GitHub repository. 


  [content]: https://github.com/Hopac/Hopac/tree/master/docs/content
  [gh]: https://github.com/Hopac/Hopac
  [issues]: https://github.com/Hopac/Hopac/issues
  [readme]: https://github.com/Hopac/Hopac/blob/master/README.md
  [license]: https://github.com/Hopac/Hopac/blob/master/LICENSE.md
*)
