/*
 * Copyright 2009-2010 Jon Klein
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using Psh;

/// <summary>
/// PshInspector pshFile
/// PshInspector can be used to inspect the execution of a Psh program.
/// </summary>
/// <remarks>
/// PshInspector pshFile
/// PshInspector can be used to inspect the execution of a Psh program.
/// After every step of the program, the stacks of the interpreter
/// are displayed. The Psh program is given to PshInspector through
/// the file pshFile. pshFile is an input file containing the
/// following, separated by new lines:
/// - Program: The psh program to run
/// - ExecutionLimit: Maximum execution steps
/// - Input(optional): Any inputs to be pushed before execution,
/// separated by spaces. Note: Only int, float, and
/// boolean inputs are accepted.
/// </remarks>
public class PshInspector {
  /// <exception cref="System.Exception"/>
  public static void Main(string[] args) {
    if (args.Length != 1) {
      Console.Out.WriteLine("Usage: PshInspector inputfile");
      System.Environment.Exit(0);
    }
    // _input will allow easy initialization of the interpreter.
    InspectorInput _input = new InspectorInput(args[0]);
    Interpreter _interpreter = _input.GetInterpreter();
    int _executionLimit = _input.GetExecutionLimit();
    int executed = 0;
    int stepsTaken = 1;
    string stepPrint = string.Empty;
    // Print registered instructions
    Console.Out.WriteLine("Registered Instructions: " + _interpreter.GetRegisteredInstructionsString() + "\n");
    // Run the Psh Inspector
    Console.Out.WriteLine("====== State after " + executed + " steps ======");
    _interpreter.PrintStacks();
    while (executed < _executionLimit && stepsTaken == 1) {
      executed += 1;
      // Create output string
      if (executed == 1) {
        stepPrint = "====== State after " + executed + " step ";
      } else {
        stepPrint = "====== State after " + executed + " steps ";
      }
      stepPrint += "(last step: ";
      object execTop = _interpreter.ExecStack().Top();
      if (execTop is Program) {
        stepPrint += "(...)";
      } if (execTop != null) {
        stepPrint += execTop.ToString();
      } else {
        stepPrint += "null";
      }
      stepPrint += ") ======";
      // Execute 1 instruction
      stepsTaken = _interpreter.Step(1);
      if (stepsTaken == 1) {
        Console.Out.WriteLine(stepPrint);
        _interpreter.PrintStacks();
      }
    }
  }
}
