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

namespace Psh {
public class InputPusher {

  public void PushInput(Interpreter inI, int n) {
    ObjectStack _stack = inI.InputStack();
    if (_stack.Size() > n) {
      object inObject = _stack.DeepPeek(n);
      if (inObject is int) {
        IntStack istack = inI.IntStack();
        istack.Push((int)inObject);
      } else {
        // if (inObject is Number)
        // {
        //   FloatStack fstack = inI.FloatStack();
        //   fstack.Push(((Number)inObject).FloatValue());
        // }
        //else
        if (inObject is float) {
          FloatStack fstack = inI.FloatStack();
          fstack.Push((float) inObject);
        } else {
          if (inObject is bool) {
            BooleanStack bstack = inI.BoolStack();
            bstack.Push((bool)inObject);
          } else {
            Console.Error.WriteLine("Error during input.index - object " + inObject.GetType() + " is not a legal object according to " + this.GetType() + ".");
          }
        }
      }
    }
  }
}
}
