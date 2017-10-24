using System;
using Psh;

/// <summary>PshGP executes a genetic programming run using the given parameter file.</summary>
/// <remarks>
/// PshGP executes a genetic programming run using the given parameter file. More
/// information about parameter files can be found in the README.
/// </remarks>
public class PshGP {
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
  /// <exception cref="System.Exception"/>
  public static void Main(string[] args) {
    if (args.Length != 1 && args.Length != 3) {
      Console.Out.WriteLine("Usage: PshGP paramfile [testprogram testcasenumber]");
      System.Environment.Exit(0);
    }
    Console.Out.WriteLine("Hi");
    GA ga = null;
    // if (args[0].EndsWith(".gz"))
    // {
    //   ga = GA.GAWithCheckpoint(args[0]);
    // }
    // else
    {
      ga = GA.GAWithParameters(Params.ReadFromFile(args[0]));
    }
    if (args.Length == 3) {
      // Execute a test program
      Program p = new Program(args[1]);
      ((PushGP)ga).RunTestProgram(p, System.Convert.ToInt32(args[2]));
    } else {
      // Execute a GP run.
      ga.Run();
    }
  }
}
