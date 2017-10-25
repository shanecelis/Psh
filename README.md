PshSharp
========

PshSharp is a C# port of Jon Klein's [project Psh](https://github.com/jonklein/Psh), a Java implementation of the Push programming language and PushGP.  Push is a stack-based language designed for evolutionary computation, specifically genetic programming.  PushGP is a genetic programming system that evolves programs in Push.  More information about Push and PushGP can be found [here](http://hampshire.edu/lspector/push.html).

This is version 0.1.0 of PshSharp.

Getting Started
===============

Get PshSharp
------------

Get the code from github.

    $ git clone git://github.com/shanecelis/PshSharp.git
    $ cd PshSharp

Build PshSharp
--------------

Build the library and binaries.

    $ make

Exercise PshGP
--------------

Run PshGP on a sample problem.

    $ mono PshGP.exe gpsamples/intreg1.pushgp

This problem uses integer symbolic regression to solve the equation y = 12x^2 + 5. Other sample problems are available, with descriptions, in `gpsamples/`. For example, `intreg2.pushgp` uses integer symbolic regression to solve the factorial function, and `regression1.pushgp` uses float symbolic regression to solve y = 12x^2 + 5.

PshInspector
------------

PshInspector allows you to examine each step of a Psh program as it executes. To run PshInspector on a sample psh program:

    $ mono PshInspector.exe pushsamples/exampleProgram1.push

This push file runs the psh program `(2994 5 integer.+)` for 100 steps after pushing the inputs `44, 22, true, 17.76`. Other sample Psh programs are available in the `pushsamples` directory.

Documentation
-------------

[Doxygen](http://www.doxygen.org) is used to generate the API documentation.

    $ make doc

Then open `html/index.html`.

Unit Tests
----------

Install [NUnit](http://nunit.org) and edit the Makefile variable `NUNIT_DIR`.

Run tests.

    $ make test

Psh In Detail
=============

Configuration Files
-------------------
PshGP runs are setup using configuration files which have the extension `.pushgp`. These files contain a list of parameters in the form of 

    param-name = value

The following parameters must be defined in the configuration file, given with example values:

    problem-class = Psh.IntSymbolicRegression
    
    max-generations = 200
    population-size = 1000
    execution-limit = 150
    max-points-in-program = 100
    max-random-code-size = 40
    
    tournament-size = 7
    mutation-percent = 30
    crossover-percent = 55
    simplification-percent = 5
    
    reproduction-simplifications = 25
    report-simplifications = 100
    final-simplifications = 1000
    
    test-cases = ((1 1) (2 2) (3 6) (4 24) (5 120) (6 720))
    instruction-set = (registered.exec registered.boolean integer.% integer.* integer.+ integer.- integer./ integer.dup)

The following parameters are optional. If not specified, the default values below will be used for these parameters, except for the parameters `mutation-mode`, `output-file`, and `push-frame-mode`, which significantly change the run when specified. Also, `target-function-string` defaults to not displaying a string, but a representative example is given below.

    trivial-geography-radius = 10
    simplify-flatten-percent = 20
    mutation-mode = fair
    fair-mutation-range = .3
    
    node-selection-mode = unbiased  (others available are leaf-probability and size-tournament)
	node-selection-leaf-probability = 10  (only used if node-selection-mode = leaf-probability)
	node-selection-tournament-size = 2  (only used if node-selection-mode = size-tournament)
    
    min-random-integer = -10
    max-random-integer = 10
    random-integer-resolution = 1
    min-random-float = -10.0
    max-random-float = 10.0
    random-float-resolution = 0.01
    
    target-function-string = "y = x^4 - 2x + 7"
    
    interpreter-class = Psh.Interpreter
    individual-class = Psh.PushGPIndividual
    inputpusher-class = Psh.InputPusher
    
    output-file = out.txt
    push-frame-mode = pushstacks

PshInspector Files
------------------
In order to inspect the execution of a program, PshInspector takes a push program file with the extension `.push`. After every step of the program, the stacks of the interpreter are displayed. The input file contains the following, separated by new lines:

- Program: The Psh program to run
- ExecutionLimit: Maximum execution steps
- Input(optional): Any inputs to be pushed before execution, separated by spaces. The inputs are pushed in the order in which they are given. Note: Only int, float, and boolean inputs are accepted.

Problem Classes
---------------
PshGP uses problem classes, implemented as C# classes, to determine certain aspects of the run, such as how to compute fitness values. The choice of problem class determines how test case data is interpreted, and which stacks are used for test case input and output. In addition, certain inherited methods in both GA.cs and PushGP.cs may be overwritten for further customization.

Psh comes with a few standard problem classes. The following problem classes are currently implemented, and are in the Psh.ProbClass namespace:

- FloatSymbolicRegression.cs: Maps an input floating point value to an output floating point value. Error value is computed as the difference between the desired output value and the top value on the float stack.
- IntSymbolicRegression.cs: Maps an input integer value to an output integer value. Error value is computed as the difference between the desired output value and the top value on the integer stack.
- CartCentering.cs: Maps two input floats (position and velocity) to a boolean value that represents a forward or backward force applied to a cart. The error is the amount of time required to stop the cart at the origin. For more information, see the problem class file.

In order to perform runs for other types of problems, you can implement your own custom problem classes. Please note the following:

- You will likely want to implement the InitFromParamenters method, which can be used to set up test cases. If so, make sure to also call its parent method.
- In PshGP, the term fitness actually refers to error values, which means that lower values are considered more fit and that 0.0 represents no error. The EvaluateTestCase method must be implemented by any problem class, and should compute an individual's fitness, with lower values being better.
- The InitInterpreter method must be implemented by all problem classes though many times this method is simply left empty.
- There are other optional methods that can be overwritten or extended in the GA.cs and PushGP.cs classes. For example, the CartCentering.cs problem class implements the Success method in order to override the conditions that GA uses to identify a successful run.

Change Log
==========

Major Changes from Psh v1.1
---------------------------
- Ported Psh to C# with the help of the [Sharpen tool](https://github.com/mono/sharpen).
- Removed {int,float,bool}stacks in favor of `GenericStack<T>`.
- Added "exec.yield" instruction.
- Removed experimental "frame.{push,pop}" instructions.
- Removed checkpoint functionality.
- Unified stack handling.
- Added case sensitivity option.
- Renamed `Peek()` to `DeepPeek()`.
- Exposed `DefineInstruction()` for use with lambdas.
- Removed classes that are now implemented by lambdas.
- Removed interpreter field from Program class.

See previous change log from Java [Psh project](https://github.com/jonklein/Psh).

Acknowledgments
===============

Jon Klein, Tom Helmuth, Robert Baruch wrote the original Psh implementation in Java, which PshSharp is based on.  Psh was supported partially by National Science Foundation under Grant No. 1017817.

Lee Spector, Chris Perry, Jon Klein, and Maarten Keijzer wrote the [Push 3.0 Programming Language Description](http://faculty.hampshire.edu/lspector/push3-description.html).

License
=======

Apache License v2.0

Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License [here](http://www.apache.org/licenses/LICENSE-2.0).

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.
