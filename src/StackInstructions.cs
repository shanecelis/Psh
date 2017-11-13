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
/// <summary>
/// Abstract instruction class for instructions which operate on any of the
/// built-in stacks.
/// </summary>
internal abstract class StackInstruction : Instruction {

  protected internal Stack _stack;

  internal StackInstruction(Stack inStack) {
    //
    // All instructions
    //
    _stack = inStack;
  }

  public abstract void Execute(Interpreter inI);
}

/// <summary>
/// Abstract instruction class for instructions which operate on one of the
/// standard ObjectStacks (code & exec).
/// </summary>
public abstract class ObjectStackInstruction : Instruction {

  protected internal ObjectStack _stack;

  internal ObjectStackInstruction(ObjectStack inStack) {
    _stack = inStack;
  }

  public abstract void Execute(Interpreter inI);
}

internal class Quote : Instruction {

  public void Execute(Interpreter inI) {
    ObjectStack cstack = inI.CodeStack();
    ObjectStack estack = inI.ExecStack();
    if (estack.Size() > 0) {
      cstack.Push(estack.Pop());
    }
  }
}

internal class Pop : StackInstruction {

  internal Pop(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    if (_stack.Size() > 0) {
      _stack.Popdiscard();
    }
  }
}

internal class Flush : StackInstruction {

  internal Flush(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    _stack.Clear();
  }
}

internal class Dup : StackInstruction {

  internal Dup(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    _stack.Dup();
  }
}

internal class Rot : StackInstruction {

  internal Rot(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    if (_stack.Size() > 2) {
      _stack.Rot();
    }
  }
}

internal class Shove : StackInstruction {

  internal Shove(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    IntStack iStack = inI.IntStack();
    if (iStack.Size() > 0) {
      int index = iStack.Pop();
      if (_stack.Size() > 0) {
        _stack.Shove(index);
      } else {
        iStack.Push(index);
      }
    }
  }
}

internal class Swap : StackInstruction {

  internal Swap(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    if (_stack.Size() > 1) {
      _stack.Swap();
    }
  }
}

internal class Yank : StackInstruction {

  internal Yank(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    IntStack iStack = inI.IntStack();
    if (iStack.Size() > 0) {
      int index = iStack.Pop();
      if (_stack.Size() > 0) {
        _stack.Yank(index);
      } else {
        iStack.Push(index);
      }
    }
  }
}

internal class YankDup : StackInstruction {

  internal YankDup(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    IntStack iStack = inI.IntStack();
    if (iStack.Size() > 0) {
      int index = iStack.Pop();
      if (_stack.Size() > 0) {
        _stack.YankDup(index);
      } else {
        iStack.Push(index);
      }
    }
  }
}

internal class Depth : StackInstruction {

  internal Depth(Stack inStack) : base(inStack) {}

  public override void Execute(Interpreter inI) {
    IntStack stack = inI.IntStack();
    stack.Push(_stack.Size());
  }
}

}
