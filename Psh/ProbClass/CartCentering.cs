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

namespace Psh.ProbClass {
/// <summary>A sample problem class for testing the cart centering problem.</summary>
/// <remarks>
/// A sample problem class for testing the cart centering problem. This solves
/// the cart centering problem as described in John Koza's Genetic Programming,
/// chapter 7.1. In this problem, a cart is placed on a 1-dimensional,
/// frictionless track. At every time, the cart has a position and velocity on
/// the track. The problem is to stop the cart at the origin (within reasonable
/// approximations) by applying a fixed-magnitude force to accelerate the cart
/// in the forward or backward direction.
/// Note: Cart centering does not yet support test case generators.
/// </remarks>
public class CartCentering : PushGP {

  /// <exception cref="System.Exception"/>
  protected internal override void InitFromParameters() {
    base.InitFromParameters();
    string cases = GetParam("test-cases");
    Program caselist = new Program(_interpreter, cases);
    for (int i = 0; i < caselist.Size(); i++) {
      Program singleCase = (Program)caselist.Peek(i);
      if (singleCase.Size() < 2) {
        throw new Exception("Not enough elements for fitness case \"" + singleCase + "\"");
      }
      float x = System.Convert.ToSingle(singleCase.Peek(0).ToString());
      float v = System.Convert.ToSingle(singleCase.Peek(1).ToString());
      Print(";; Fitness case #" + i + " position: " + x + " velocity: " + v + "\n");
      ObjectPair xv = new ObjectPair(x, v);
      _testCases.Add(new GATestCase(xv, null));
    }
  }

  protected internal override void InitInterpreter(Interpreter inInterpreter) {
  }

  public override float EvaluateTestCase(GAIndividual inIndividual, object inInput, object inOutput) {
    int timeSteps = 1000;
    float timeDiscritized = 0.01f;
    float maxTime = timeSteps * timeDiscritized;
    float captureRadius = 0.01f;
    ObjectPair xv = (ObjectPair)inInput;
    float position = (float)xv._first;
    float velocity = (float)xv._second;
    for (int step = 1; step <= timeSteps; step++) {
      _interpreter.ClearStacks();
      FloatStack fStack = _interpreter.FloatStack();
      BooleanStack bStack = _interpreter.BoolStack();
      ObjectStack iStack = _interpreter.InputStack();
      // Position will be on the top of the stack, and velocity will be
      // second.
      fStack.Push(position);
      fStack.Push(velocity);
      // Must be included in order to use the input stack. Uses same order
      // as inputs on Float stack.
      iStack.Push(position);
      iStack.Push(velocity);
      _interpreter.Execute(((PushGPIndividual)inIndividual)._program, _executionLimit);
      // If there is no boolean on the stack, the program has failed to
      // return a reasonable output. So, return a penalty fitness of
      // twice the maximum time.
      if (bStack.Size() == 0) {
        return 2 * maxTime;
      }
      // If there is a boolean, use it to compute the next position and
      // velocity. Then, check for termination conditions.
      // NOTE: If result == True, we will apply the force in the positive
      // direction, and if result == False, we will apply the force in
      // the negative direction.
      bool positiveForce = bStack.Top();
      float acceleration;
      if (positiveForce) {
        acceleration = 0.5f;
      } else {
        acceleration = -0.5f;
      }
      velocity = velocity + (timeDiscritized * acceleration);
      position = position + (timeDiscritized * velocity);
      // Check for termination conditions
      if (position <= captureRadius && position >= -captureRadius && velocity <= captureRadius && velocity >= -captureRadius) {
        //Cart is centered, so return time it took.
        return step * timeDiscritized;
      }
    }
    // If here, the cart failed to come to rest in the allotted time. So,
    // return the failed error of maxTime.
    return maxTime;
  }

  protected internal override bool Success() {
    return _generationCount >= _maxGenerations;
  }
}
}
