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
using System.IO;
using SharpenMinimal;

namespace Psh
{
  /// <summary>A utility class to help read PshInspector input files.</summary>
  public class InspectorInput
  {
    internal Program _program;

    internal int _executionLimit;

    internal Interpreter _interpreter;

    /// <summary>Constructs an InspectorInput from a filename string</summary>
    /// <param name="inFilename">The file to input from.</param>
    /// <exception cref="System.Exception"/>
    public InspectorInput(string inFilename)
    {
      InitInspectorInput(inFilename);
    }

    /// <summary>Initializes an InspectorInput.</summary>
    /// <remarks>
    /// Initializes an InspectorInput. The file should be organized as follows:
    /// 1 Program to execute
    /// 2 Execution limit
    /// 3 Integer, float, or boolean inputs
    /// 4 Available instructions. This is only used for creating random code
    /// with code.rand or exec.rand
    /// </remarks>
    /// <param name="inFile">The file to input from.</param>
    /// <exception cref="System.Exception"/>
    private void InitInspectorInput(string inFile)
    {
      _interpreter = new Interpreter();
      // Read fileString
      string fileString = Params.ReadFileString(inFile);
      // Get programString
      int indexNewline = fileString.IndexOf("\n");
      string programString = Sharpen.Extensions.Trim(Sharpen.Runtime.Substring(fileString, 0, indexNewline));
      fileString = Sharpen.Runtime.Substring(fileString, indexNewline + 1);
      // Get _executionLimit
      indexNewline = fileString.IndexOf("\n");
      if (indexNewline != -1)
      {
        string limitString = Sharpen.Extensions.Trim(Sharpen.Runtime.Substring(fileString, 0, indexNewline));
        _executionLimit = System.Convert.ToInt32(limitString);
        fileString = Sharpen.Runtime.Substring(fileString, indexNewline + 1);
      }
      else
      {
        // If here, no inputs to be pushed were included
        _executionLimit = System.Convert.ToInt32(fileString);
        fileString = string.Empty;
      }
      // Get inputs and push them onto correct stacks. If fileString = ""
      // at this point, then can still do the following with correct result.
      indexNewline = fileString.IndexOf("\n");
      if (indexNewline != -1)
      {
        string inputsString = Sharpen.Extensions.Trim(Sharpen.Runtime.Substring(fileString, 0, indexNewline));
        fileString = Sharpen.Runtime.Substring(fileString, indexNewline + 1);
        // Parse the inputs and load them into the interpreter
        ParseAndLoadInputs(inputsString);
      }
      else
      {
        ParseAndLoadInputs(fileString);
        fileString = string.Empty;
      }
      // Get the available instructions for random code generation
      indexNewline = fileString.IndexOf("\n");
      if (!Sharpen.Extensions.Trim(fileString).Equals(string.Empty))
      {
        _interpreter.SetInstructions(new Program(_interpreter, Sharpen.Extensions.Trim(fileString)));
      }
      // Check for input.inN instructions
      CheckForInputIn(programString);
      // Add random integer and float parameters
      _interpreter._minRandomInt = -10;
      _interpreter._maxRandomInt = 10;
      _interpreter._randomIntResolution = 1;
      _interpreter._minRandomFloat = -10.0f;
      _interpreter._maxRandomFloat = 10.0f;
      _interpreter._randomFloatResolution = 0.01f;
      _interpreter._maxRandomCodeSize = 50;
      // Load the program
      _program = new Program(_interpreter, programString);
      _interpreter.LoadProgram(_program);
    }

    // Initializes program
    /// <summary>Returns the initialized interpreter</summary>
    /// <returns>The initialized interpreter</returns>
    public virtual Interpreter GetInterpreter()
    {
      return _interpreter;
    }

    public virtual Program GetProgram()
    {
      return _program;
    }

    /// <summary>Returns the execution limit</summary>
    /// <returns>The execution limit</returns>
    public virtual int GetExecutionLimit()
    {
      return _executionLimit;
    }

    /// <exception cref="System.Exception"/>
    private void ParseAndLoadInputs(string inputs)
    {
      string[] inputTokens = inputs.Split("\\s+");
      for (int i = 0; i < inputTokens.Length; i++)
      {
        string token = inputTokens[i];
        if (token.Equals(string.Empty))
        {
          continue;
        }
        else
        {
          if (token.Equals("true"))
          {
            _interpreter.BoolStack().Push(true);
            _interpreter.InputStack().Push(true);
          }
          else
          {
            if (token.Equals("false"))
            {
              _interpreter.BoolStack().Push(false);
              _interpreter.InputStack().Push(false);
            }
            else
            {
              if (token.Matches("((-|\\+)?[0-9]+(\\.[0-9]+)?)+"))
              {
                if (token.IndexOf('.') != -1)
                {
                  _interpreter.FloatStack().Push(float.Parse(token));
                  _interpreter.InputStack().Push(float.Parse(token));
                }
                else
                {
                  _interpreter.IntStack().Push(System.Convert.ToInt32(token));
                  _interpreter.InputStack().Push(System.Convert.ToInt32(token));
                }
              }
              else
              {
                throw new Exception("Inputs must be of type int, float, or boolean. \"" + token + "\" is none of these.");
              }
            }
          }
        }
      }
    }

    private void CheckForInputIn(string programString)
    {
      string added = string.Empty;
      string numstr = string.Empty;
      int index = 0;
      int numindex = 0;
      int spaceindex = 0;
      int parenindex = 0;
      int endindex = 0;
      while (true)
      {
        index = programString.IndexOf("input.in");
        if (index == -1)
        {
          break;
        }
        // System.out.println(programString + "    " + index);
        numindex = index + 8;
        if (!char.IsDigit(programString[numindex]))
        {
          programString = Sharpen.Runtime.Substring(programString, numindex);
          continue;
        }
        spaceindex = programString.IndexOf(' ', numindex);
        parenindex = programString.IndexOf(')', numindex);
        if (spaceindex == -1)
        {
          endindex = parenindex;
        }
        else
        {
          if (parenindex == -1)
          {
            endindex = spaceindex;
          }
          else
          {
            endindex = Math.Min(spaceindex, parenindex);
          }
        }
        numstr = Sharpen.Runtime.Substring(programString, numindex, endindex);
        // Check for doubles in added
        if (added.IndexOf(" " + numstr + " ") == -1)
        {
          added = added + " " + numstr + " ";
          _interpreter.AddInstruction("input.in" + numstr, new InputInN(System.Convert.ToInt32(numstr)));
        }
        programString = Sharpen.Runtime.Substring(programString, numindex);
      }
    }
  }
}
