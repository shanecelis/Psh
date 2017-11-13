/*
 * Copyright 2017 Shane Celis
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

using System.Collections.Generic;
using NUnit.Framework;
using Psh;
namespace Psh.Tests {

[TestFixture]
public class InterpreterTests {
  private Interpreter interpreter;

  [SetUp]
  public void Setup() {
    interpreter = new Interpreter();
  }

  [Test]
  public void TestSetInstructions() {
    interpreter.randProgram.SetInstructions(interpreter);
    Assert.AreEqual(0, interpreter.randProgram._randomGenerators.Count);
    interpreter.randProgram.SetInstructions(interpreter, @".*\+");
    Assert.AreEqual(2, interpreter.randProgram._randomGenerators.Count);
    interpreter.randProgram.SetInstructions(interpreter, @"boolean.*");
    Assert.AreEqual(22, interpreter.randProgram._randomGenerators.Count);
  }

  [Test]
  public void TestRandBool() {
    var boolGen = new Interpreter.BoolAtomGenerator();
    object o = boolGen.Generate();
    Assert.IsNotNull(o);
    Assert.IsTrue(o is bool);
    bool b = (bool) o;
    Assert.IsTrue(b == true || b == false);
    // Assert.Fail("b is " + b);
  }

  [Test]
  public void TestRandFloat() {
    var floatGen = new Interpreter.FloatAtomGenerator();
    object o = floatGen.Generate();
    Assert.IsNotNull(o);
    Assert.IsTrue(o is float);
    float b = (float) o;
    Assert.IsTrue(b > 0f || b < 0f || b == 0f, "What the heck is b? " + b);
    var f = floatGen.GenerateT();
    Assert.AreNotEqual(0f, f);
  }

  [Test]
  public void TestResolution() {
    Assert.AreEqual(0.100000381f, 11.1f % 1f);
    Assert.AreEqual(0f, 11f % 1f);
    Assert.AreEqual(0f, 1f % 1f);
    Assert.AreEqual(2f, 12f % 10f);
    Assert.AreEqual(float.NaN, 1f % 0f);
    Assert.AreEqual(0.1f, 0.1f % 1f);

    Assert.AreEqual(0, 11 % 1);
    Assert.AreEqual(0, 1 % 1);
    Assert.AreEqual(2, 12 % 10);
    // Assert.AreEqual(-1, 1 % 0);
  }

}

}
