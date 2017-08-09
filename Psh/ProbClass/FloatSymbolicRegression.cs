using System;
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
using Psh.TestCase;
using Sharpen;

namespace Psh.ProbClass
{
  /// <summary>This problem class implements symbolic regression for floating point numbers.</summary>
  /// <remarks>
  /// This problem class implements symbolic regression for floating point numbers.
  /// See also IntSymbolicRegression for integer symbolic regression.
  /// </remarks>
  [System.Serializable]
  public class FloatSymbolicRegression : PushGP
  {
    private const long serialVersionUID = 1L;

    private float _noResultPenalty = 10000;

    /// <exception cref="System.Exception"/>
    protected internal override void InitFromParameters()
    {
      base.InitFromParameters();
      string cases = GetParam("test-cases", true);
      string casesClass = GetParam("test-case-class", true);
      if (cases == null && casesClass == null)
      {
        throw new Exception("No acceptable test-case parameter.");
      }
      if (casesClass != null)
      {
        // Get test cases from the TestCasesClass.
        Type iclass = Sharpen.Runtime.GetType(casesClass);
        object iObject = System.Activator.CreateInstance(iclass);
        if (!(iObject is TestCaseGenerator))
        {
          throw (new Exception("test-case-class must inherit from class TestCaseGenerator"));
        }
        TestCaseGenerator testCaseGenerator = (TestCaseGenerator)iObject;
        int numTestCases = testCaseGenerator.TestCaseCount();
        for (int i = 0; i < numTestCases; i++)
        {
          ObjectPair testCase = testCaseGenerator.TestCase(i);
          float @in = (float)testCase._first;
          float @out = (float)testCase._second;
          Print(";; Fitness case #" + i + " input: " + @in + " output: " + @out + "\n");
          _testCases.Add(new GATestCase(@in, @out));
        }
      }
      else
      {
        // Get test cases from test-cases.
        Program caselist = new Program(_interpreter, cases);
        for (int i = 0; i < caselist.Size(); i++)
        {
          Program p = (Program)caselist.Peek(i);
          if (p.Size() < 2)
          {
            throw new Exception("Not enough elements for fitness case \"" + p + "\"");
          }
          float @in = System.Convert.ToSingle(p.Peek(0).ToString());
          float @out = System.Convert.ToSingle(p.Peek(1).ToString());
          Print(";; Fitness case #" + i + " input: " + @in + " output: " + @out + "\n");
          _testCases.Add(new GATestCase(@in, @out));
        }
      }
    }

    protected internal override void InitInterpreter(Interpreter inInterpreter)
    {
    }

    public override float EvaluateTestCase(GAIndividual inIndividual, object inInput, object inOutput)
    {
      _interpreter.ClearStacks();
      float currentInput = (float)inInput;
      FloatStack stack = _interpreter.FloatStack();
      stack.Push(currentInput);
      // Must be included in order to use the input stack.
      _interpreter.InputStack().Push(currentInput);
      _interpreter.Execute(((PushGPIndividual)inIndividual)._program, _executionLimit);
      float result = stack.Top();
      // Penalize individual if there is no result on the stack.
      if (stack.Size() == 0)
      {
        return _noResultPenalty;
      }
      return result - ((float)inOutput);
    }

    public virtual float GetIndividualTestCaseResult(GAIndividual inIndividual, GATestCase inTestCase)
    {
      _interpreter.ClearStacks();
      float currentInput = (float)inTestCase._input;
      FloatStack stack = _interpreter.FloatStack();
      stack.Push(currentInput);
      // Must be included in order to use the input stack.
      _interpreter.InputStack().Push(currentInput);
      _interpreter.Execute(((PushGPIndividual)inIndividual)._program, _executionLimit);
      float result = stack.Top();
      // If no result, return 0
      if (stack.Size() == 0)
      {
        return 0;
      }
      return result;
    }

    protected internal override bool Success()
    {
      return _bestMeanFitness <= 0.1;
    }
  }
}
