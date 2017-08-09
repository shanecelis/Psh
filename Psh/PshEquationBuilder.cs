using System;
using Psh;
using Sharpen;

/// <summary>Used to print equations from Psh programs</summary>
public class PshEquationBuilder
{
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
  public static void Main(string[] args)
  {
    if (args.Length != 1)
    {
      System.Console.Out.Println("Usage: PshEquationBuilder inputfile");
      System.Environment.Exit(0);
    }
    FilePath inFile = new FilePath(args[0]);
    // Read fileString
    string fileString = Params.ReadFileString(inFile);
    // Get programString
    string programString;
    int indexNewline = fileString.IndexOf("\n");
    if (indexNewline == -1)
    {
      programString = fileString;
    }
    else
    {
      programString = Sharpen.Extensions.Trim(Sharpen.Runtime.Substring(fileString, 0, indexNewline));
    }
    //Get rid of parentheses
    programString = programString.Replace('(', ' ');
    programString = Sharpen.Extensions.Trim(programString.Replace(')', ' '));
    string[] instructions = programString.Split("\\s+");
    AList<string> stringStack = new AList<string>();
    stringStack.Add("x");
    foreach (string instruction in instructions)
    {
      // (input.in0 float.+ float.- float.* float./ float.exp float.sin float.cos float.2pi)
      if (instruction.Equals("input.in0"))
      {
        stringStack.Add("x");
      }
      else
      {
        if (instruction.Equals("float.+"))
        {
          if (stringStack.Count > 1)
          {
            string top = stringStack.Remove(stringStack.Count - 1);
            string next = stringStack.Remove(stringStack.Count - 1);
            string result = "(" + top + " + " + next + ")";
            stringStack.Add(result);
          }
        }
        else
        {
          if (instruction.Equals("float.-"))
          {
            if (stringStack.Count > 1)
            {
              string top = stringStack.Remove(stringStack.Count - 1);
              string next = stringStack.Remove(stringStack.Count - 1);
              string result = "(" + top + " - " + next + ")";
              stringStack.Add(result);
            }
          }
          else
          {
            if (instruction.Equals("float.*"))
            {
              if (stringStack.Count > 1)
              {
                string top = stringStack.Remove(stringStack.Count - 1);
                string next = stringStack.Remove(stringStack.Count - 1);
                string result = "(" + top + " * " + next + ")";
                stringStack.Add(result);
              }
            }
            else
            {
              if (instruction.Equals("float./"))
              {
                if (stringStack.Count > 1)
                {
                  string top = stringStack.Remove(stringStack.Count - 1);
                  string next = stringStack.Remove(stringStack.Count - 1);
                  string result = "(" + top + " / " + next + ")";
                  stringStack.Add(result);
                }
              }
              else
              {
                if (instruction.Equals("float.exp"))
                {
                  if (stringStack.Count > 0)
                  {
                    string top = stringStack.Remove(stringStack.Count - 1);
                    string result = "(e^" + top + ")";
                    stringStack.Add(result);
                  }
                }
                else
                {
                  if (instruction.Equals("float.sin"))
                  {
                    if (stringStack.Count > 0)
                    {
                      string top = stringStack.Remove(stringStack.Count - 1);
                      string result = "sin(" + top + ")";
                      stringStack.Add(result);
                    }
                  }
                  else
                  {
                    if (instruction.Equals("float.cos"))
                    {
                      if (stringStack.Count > 0)
                      {
                        string top = stringStack.Remove(stringStack.Count - 1);
                        string result = "cos(" + top + ")";
                        stringStack.Add(result);
                      }
                    }
                    else
                    {
                      if (instruction.Equals("float.2pi"))
                      {
                        stringStack.Add("2 * pi");
                      }
                      else
                      {
                        throw new Exception("Unrecognized Psh instruction " + instruction + " in program.");
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    System.Console.Out.Println(stringStack[stringStack.Count - 1]);
  }
}
