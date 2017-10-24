/*
 * Copyright 2009-2010 Jon Klein and Robert Baruch
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
public class GenericStackTest
{
  [Test] public void testPushPop() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();
    GenericStack<List<string>> stringListStack = new GenericStack<List<string>>();

    List<string> vect = new List<string>();
    vect.Add("a string in a vector 1");
    vect.Add("another string 2");

    stringStack.Push("value 1");
    stringListStack.Push(vect);

    stringStack.Push("value 2");
    stringListStack.Push(null);

    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual(2, stringListStack.Count);

    Assert.AreEqual("value 2", stringStack.Pop());
    Assert.AreEqual(1, stringStack.Count);
    Assert.AreEqual("value 1", stringStack.Pop());
    Assert.AreEqual(0, stringStack.Count);

    Assert.Null(stringListStack.Pop());
    Assert.AreEqual(vect, stringListStack.Pop());

    Assert.Null(stringStack.Pop());
    Assert.AreEqual(0, stringStack.Count);
  }

  [Test] public void testPushAllReverse() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Push("value 1");
    stringStack.Push("value 2");

    GenericStack<string> stringStack2 = new GenericStack<string>();

    stringStack.PushAllReverse(stringStack2);

    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual(2, stringStack2.Count);
    Assert.AreEqual("value 1", stringStack2.Pop());
    Assert.AreEqual("value 2", stringStack2.Pop());
  }

  [Test] public void testHashCode() 
  {
    List<string> stringStack = new List<string>();
    List<string> stringStack2 = new List<string>();

    // System.out.println("stringStack type is " + stringStack.getClass());
    // XXX What!?
    // Assert.True(stringStack == (stringListStack)); // see note in equals
    // Assert.True(stringStack == (stringStack2));

    // In general, collections with the same elements and not equal via hashcode.
    Assert.AreNotEqual(stringStack.GetHashCode(), stringStack2.GetHashCode());
  }

  [Test] public void testEquals() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();
    GenericStack<string> stringStack2 = new GenericStack<string>();
    GenericStack<List<string>> stringListStack = new GenericStack<List<string>>();

    // System.out.println("stringStack type is " + stringStack.getClass());
    // XXX What!?
    // Assert.True(stringStack == (stringListStack)); // see note in equals
    // Assert.True(stringStack == (stringStack2));

    Assert.AreEqual(stringStack.GetHashCode(), stringStack2.GetHashCode());
    // Assert.AreEqual(stringStack.GetHashCode(), stringListStack.GetHashCode()); // see note in equals
    // XXX C# doesn't do type erasure so this doesn't work anymore. Thankfully.
    Assert.AreNotEqual(stringStack.GetHashCode(), stringListStack.GetHashCode());

    stringStack.Push("value 1");
    Assert.False(stringStack == (stringStack2));
    // Assert.False(stringStack == (stringListStack));
    Assert.False(stringStack.GetHashCode() == stringStack2.GetHashCode());

    stringStack2.Push("value 1");
    Assert.True(stringStack.Equals(stringStack2));

    Assert.AreEqual(stringStack.GetHashCode(), stringStack2.GetHashCode());
  }

  [Test] public void testPeek() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Push("value 1");
    stringStack.Push("value 2");

    Assert.AreEqual("value 1", stringStack.Peek(0)); // deepest stack
    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Top());
    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(1));
  }

  [Test] public void testDup() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Dup();
    Assert.AreEqual(0, stringStack.Count);

    stringStack.Push("value 1");
    stringStack.Push("value 2");
    stringStack.Dup();

    Assert.AreEqual(3, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(0));
    Assert.AreEqual("value 2", stringStack.Peek(1));
    Assert.AreEqual("value 1", stringStack.Peek(2));
  }

  [Test] public void testSwap() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Push("value 1");
    stringStack.Swap();
    Assert.AreEqual(1, stringStack.Count);
    Assert.AreEqual("value 1", stringStack.Peek(0));

    stringStack.Push("value 2");
    stringStack.Swap();

    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual("value 1", stringStack.Peek(1));
    Assert.AreEqual("value 2", stringStack.Peek(0));
  }

  [Test] public void testRot() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Push("value 1");
    stringStack.Push("value 2");
    stringStack.Rot();

    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(1));
    Assert.AreEqual("value 1", stringStack.Peek(0));

    stringStack.Push("value 3");
    stringStack.Push("value 4");
    stringStack.Rot();
    Assert.AreEqual(4, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(3));
    Assert.AreEqual("value 4", stringStack.Peek(2));
    Assert.AreEqual("value 3", stringStack.Peek(1));
    Assert.AreEqual("value 1", stringStack.Peek(0));
  }

  [Test] public void testShove() 
  {
    GenericStack<string> stringStack = new GenericStack<string>();

    stringStack.Shove("value 1", 0);
    Assert.AreEqual(1, stringStack.Count);
    Assert.AreEqual("value 1", stringStack.Peek(0));

    stringStack.Shove("value 2", 1);
    Assert.AreEqual(2, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(0));
    Assert.AreEqual("value 1", stringStack.Peek(1));

    stringStack.Shove("value 3", 1);
    Assert.AreEqual(3, stringStack.Count);
    Assert.AreEqual("value 2", stringStack.Peek(0));
    Assert.AreEqual("value 3", stringStack.Peek(1));
    Assert.AreEqual("value 1", stringStack.Peek(2));

  }
}
}
