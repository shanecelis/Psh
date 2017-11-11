
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

/// <summary>Abstract instruction base for all instructions.</summary>
public interface Instruction {
  void Execute(Interpreter inI);
}

public interface IPeekable {
  bool peek { get; set; }
}

public class Constant<T> : Instruction {

  internal T _value;

  public Constant(T inValue) {
    _value = inValue;
  }

  public void Execute(Interpreter inI) {
    var stack = inI.GetStack<T>();
    stack.Push(_value);
  }
}

internal class ObjectConstant : ObjectStackInstruction {

  internal object _value;

  public ObjectConstant(ObjectStack inStack, object inValue) : base(inStack) {
    _value = inValue;
  }

  public override void Execute(Interpreter inI) {
    _stack.Push(_value);
  }
}

public abstract class Peekable : IPeekable {
  private bool _peek = false;
  public bool peek {
    get { return _peek; }
    set { _peek = value; }
  }

  // public abstract void Execute(Interpreter inI);
}

public class NullaryAction : Instruction {
  private Action func;

  public NullaryAction(Action func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    func();
  }
}

public class NullaryInstruction<T> : Instruction {
  private Func<T> func;

  public NullaryInstruction(Func<T> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 0) {
      stack.Push(func());
    }
  }
}

public class UnaryAction<T> : Peekable, Instruction {
  private Action<T> func;

  public UnaryAction(Action<T> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 0) {
    func(! peek ? stack.Pop() : stack.Top());
    }
  }
}

public class UnaryInstruction<inT,outT> : Peekable, Instruction {
  private Func<inT,outT> func;

  public UnaryInstruction(Func<inT,outT> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    var istack = inI.GetStack<inT>();
    var ostack = inI.GetStack<outT>();
    if (istack.Size() > 0) {
      ostack.Push(func(! peek ? istack.Pop() : istack.Top()));
    }
  }
}

public class BinaryAction<X,Y> : Peekable, Instruction {
  private Action<X,Y> func;

  public BinaryAction(Action<X,Y> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    var stack1 = inI.GetStack<X>();
    var stack2 = inI.GetStack<Y>();

    // This is no good any more, because X might equal Y in which case
    // stack1.Size() > 1

    // if (stack1.Size() > 0 && stack2.Size() > 0) {
      if (stack2.Count == 0)
        return;
      Y b = !peek ? stack2.Pop() : stack2.Top();
      if (stack1.Count == 0)
        return;
      X a = ! peek ? stack1.Pop() : stack1.Top();
      try {
        func(a, b);
      } catch (ArithmeticException) {
        // Don't care.
      } catch (Exception e) {
        throw new Exception("Instruction failed for arguments " + a + " and " + b, e);
      }
  }
}

public class BinaryInstruction<X,Y,Z> : Peekable, Instruction {
  private Func<X,Y,Z> func;

  public BinaryInstruction(Func<X,Y,Z> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    var istack = inI.GetStack<X>();
    var i2stack = inI.GetStack<Y>();
    var ostack = inI.GetStack<Z>();
    // if (istack.Size() > 1 && i2stack.Size() > 1) {
    if (i2stack.Count == 0)
      return;
    Y b = ! peek ? i2stack.Pop() : i2stack.Top();
    if (istack.Count == 0)
      return;
    X a = ! peek ? istack.Pop() : istack.Top();
    Z c;
    try {
      c = func(a, b);
    } catch (ArithmeticException) {
      c = default(Z);
    } catch (Exception e) {
      throw new Exception("Instruction failed for arguments " + a + " and " + b, e);
    }
    ostack.Push(c);
  }
}

public class TrinaryAction<X,Y,Z> : Peekable, Instruction {
  private Action<X,Y,Z> func;
  public TrinaryAction(Action<X,Y,Z> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    var stack1 = inI.GetStack<X>();
    var stack2 = inI.GetStack<Y>();
    var stack3 = inI.GetStack<Z>();

    // This is no good any more, because X might equal Y in which case
    // stack1.Size() > 1

    // if (stack1.Size() > 0 && stack2.Size() > 0) {
    if (stack3.Count == 0)
      return;
    Z c = ! peek ? stack3.Pop() : stack3.Top();
    if (stack2.Count == 0)
      return;
    Y b = ! peek ? stack2.Pop() : stack2.Top();
    if (stack1.Count == 0)
      return;
    X a = ! peek ? stack1.Pop() : stack1.Top();
    try {
      func(a, b, c);
    } catch (ArithmeticException) {
      // Don't care.
    } catch (Exception e) {
      throw new Exception("Instruction failed for arguments " + a + " and " + b + " and " + c, e);
    }
  }
}

public class TrinaryInsruction<X,Y,Z,W> : Peekable, Instruction {
  private Func<X,Y,Z,W> func;
  public TrinaryInsruction(Func<X,Y,Z,W> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI) {
    var stack1 = inI.GetStack<X>();
    var stack2 = inI.GetStack<Y>();
    var stack3 = inI.GetStack<Z>();
    var stack4 = inI.GetStack<W>();

    // This is no good any more, because X might equal Y in which case
    // stack1.Size() > 1

    // if (stack1.Size() > 0 && stack2.Size() > 0) {
    if (stack3.Count == 0)
      return;
    Z c = ! peek ? stack3.Pop() : stack3.Top();
    if (stack2.Count == 0)
      return;
    Y b = ! peek ? stack2.Pop() : stack2.Top();
    if (stack1.Count == 0)
      return;
    X a = ! peek ? stack1.Pop() : stack1.Top();
    W d;
    try {
      d = func(a, b, c);
    } catch (ArithmeticException) {
      // Don't care.
      d = default(W);
    } catch (Exception e) {
      throw new Exception("Instruction failed for arguments " + a + " and " + b + " and " + c, e);
    }
    stack4.Push(d);
  }
}

internal class IntegerRand : Instruction {
  internal Random Rng;

  internal IntegerRand() {
    Rng = new Random();
  }

  public void Execute(Interpreter inI) {
    int range = (inI._maxRandomInt - inI._minRandomInt) / inI._randomIntResolution;
    int randInt = (Rng.Next(range) * inI._randomIntResolution) + inI._minRandomInt;
    inI.IntStack().Push(randInt);
  }
}


// internal class FloatTan : UnaryFloatInstruction
// {
//   internal override float UnaryOperator(float inValue)
//   {
//     // Test for overflow
//     float result = (float)Math.Tan(inValue);
//     if (float.IsInfinity(result) && result > 0)
//     {
//       return float.MaxValue;
//     }
//     if (float.IsInfinity(result) && result < 0)
//     {
//       return (1.0f - float.MaxValue);
//     }
//     if (float.IsNaN(result))
//     {
//       return 0.0f;
//     }
//     return result;
//   }
// }

internal class FloatRand : Instruction {
  internal Random Rng;

  internal FloatRand() {
    Rng = new Random();
  }

  public void Execute(Interpreter inI) {
    float range = (inI._maxRandomFloat - inI._minRandomFloat) / inI._randomFloatResolution;
    float randFloat = ((float)Rng.NextDouble() * range * inI._randomFloatResolution) + inI._minRandomFloat;
    inI.FloatStack().Push(randFloat);
  }
}

internal class BoolRand : Instruction {
  internal Random Rng;

  internal BoolRand() {
    Rng = new Random();
  }

  public void Execute(Interpreter inI) {
    inI.BoolStack().Push(Rng.Next(2) == 1);
  }
}

internal class InputInN : Instruction {
  protected internal int index;

  internal InputInN(int inIndex) {
    //
    // Instructions for input stack
    //
    index = inIndex;
  }

  public void Execute(Interpreter inI) {
    inI.GetInputPusher().PushInput(inI, index);
  }
}

internal class InputInAll : ObjectStackInstruction {
  internal InputInAll(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    if (_stack.Size() > 0) {
      for (int index = 0; index < _stack.Size(); index++) {
        inI.GetInputPusher().PushInput(inI, index);
      }
    }
  }
}

internal class InputInRev : ObjectStackInstruction {
  internal InputInRev(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    if (_stack.Size() > 0) {
      for (int index = _stack.Size() - 1; index >= 0; index--) {
        inI.GetInputPusher().PushInput(inI, index);
      }
    }
  }
}

internal class InputIndex : ObjectStackInstruction {
  internal InputIndex(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    if (istack.Size() > 0 && _stack.Size() > 0) {
      int index = istack.Pop();
      if (index < 0) {
        index = 0;
      }
      if (index >= _stack.Size()) {
        index = _stack.Size() - 1;
      }
      inI.GetInputPusher().PushInput(inI, index);
    }
  }
}

// (
//   BOOLEAN.OR
//   FALSE
//   TRUE
//   FLOAT.+
//   5.2
//   4.1
//   INTEGER.*
//   3
//  2

//   )


internal class CodeDoRange : ObjectStackInstruction {
  internal CodeDoRange(Interpreter inI)
  : base(inI.CodeStack()) {}

  //
  // Instructions for code and exec stack
  //
  // trh//All code and exec stack iteration fuctions have been fixed to match the
  // specifications of Push 3.0
  // Begin code iteration functions
  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 1) {
      int stop = istack.Pop();
      int start = istack.Pop();
      object code = _stack.Pop();
      if (start == stop) {
        istack.Push(start);
        estack.Push(code);
      } else {
        istack.Push(start);
        start = (start < stop) ? (start + 1) : (start - 1);
        try {
          Program recursiveCallProgram = new Program();
          recursiveCallProgram.Push(start);
          recursiveCallProgram.Push(stop);
          recursiveCallProgram.Push("code.quote");
          recursiveCallProgram.Push(code);
          recursiveCallProgram.Push("code.do*range");
          estack.Push(recursiveCallProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
        estack.Push(code);
      }
    }
  }
}

internal class CodeDoTimes : ObjectStackInstruction {
  internal CodeDoTimes(Interpreter inI)
  : base(inI.CodeStack()) {}

  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 0) {
      if (istack.Top() > 0) {
        object bodyObj = _stack.Pop();
        if (bodyObj is Program) {
          // insert integer.pop in front of program
          ((Program)bodyObj).Shove("integer.pop", ((Program)bodyObj)._size);
        } else {
          // create a new program with integer.pop in front of
          // the popped object
          Program newProgram = new Program();
          newProgram.Push("integer.pop");
          newProgram.Push(bodyObj);
          bodyObj = newProgram;
        }
        int stop = istack.Pop() - 1;
        try {
          Program doRangeMacroProgram = new Program();
          doRangeMacroProgram.Push(0);
          doRangeMacroProgram.Push(stop);
          doRangeMacroProgram.Push("code.quote");
          doRangeMacroProgram.Push(bodyObj);
          doRangeMacroProgram.Push("code.do*range");
          estack.Push(doRangeMacroProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
      }
    }
  }
}

internal class CodeDoCount : ObjectStackInstruction {
  internal CodeDoCount(Interpreter inI)
  : base(inI.CodeStack()) {}

  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 0) {
      if (istack.Top() > 0) {
        int stop = istack.Pop() - 1;
        object bodyObj = _stack.Pop();
        try {
          Program doRangeMacroProgram = new Program();
          doRangeMacroProgram.Push(0);
          doRangeMacroProgram.Push(stop);
          doRangeMacroProgram.Push("code.quote");
          doRangeMacroProgram.Push(bodyObj);
          doRangeMacroProgram.Push("code.do*range");
          estack.Push(doRangeMacroProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
      }
    }
  }
}

internal class CodeFromBoolean : Instruction {
  // End code iteration functions
  //
  // Conversion instructions to code
  //
  public void Execute(Interpreter inI) {
    ObjectStack codeStack = inI.CodeStack();
    BooleanStack bStack = inI.BoolStack();
    if (bStack.Size() > 0) {
      codeStack.Push(bStack.Pop());
    }
  }
}

internal class CodeFromInteger : Instruction {
  public void Execute(Interpreter inI) {
    ObjectStack codeStack = inI.CodeStack();
    IntStack iStack = inI.IntStack();
    if (iStack.Size() > 0) {
      codeStack.Push(iStack.Pop());
    }
  }
}

internal class CodeFromFloat : Instruction {
  public void Execute(Interpreter inI) {
    ObjectStack codeStack = inI.CodeStack();
    FloatStack fStack = inI.FloatStack();
    if (fStack.Size() > 0) {
      codeStack.Push(fStack.Pop());
    }
  }
}

internal class ExecDoRange : ObjectStackInstruction {
  internal ExecDoRange(Interpreter inI)
  : base(inI.ExecStack()) {}

  // Begin exec iteration functions
  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 1) {
      int stop = istack.Pop();
      int start = istack.Pop();
      object code = _stack.Pop();
      if (start == stop) {
        istack.Push(start);
        estack.Push(code);
      } else {
        istack.Push(start);
        start = (start < stop) ? (start + 1) : (start - 1);
        // trh//Made changes to correct errors with code.do*range
        try {
          Program recursiveCallProgram = new Program();
          recursiveCallProgram.Push(start);
          recursiveCallProgram.Push(stop);
          recursiveCallProgram.Push("exec.do*range");
          recursiveCallProgram.Push(code);
          estack.Push(recursiveCallProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
        estack.Push(code);
      }
    }
  }
}

internal class ExecDoTimes : ObjectStackInstruction {
  internal ExecDoTimes(Interpreter inI)
  : base(inI.ExecStack()) {}

  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 0) {
      if (istack.Top() > 0) {
        object bodyObj = _stack.Pop();
        if (bodyObj is Program) {
          // insert integer.pop in front of program
          ((Program)bodyObj).Shove("integer.pop", ((Program)bodyObj)._size);
        } else {
          // create a new program with integer.pop in front of
          // the popped object
          Program newProgram = new Program();
          newProgram.Push("integer.pop");
          newProgram.Push(bodyObj);
          bodyObj = newProgram;
        }
        int stop = istack.Pop() - 1;
        try {
          Program doRangeMacroProgram = new Program();
          doRangeMacroProgram.Push(0);
          doRangeMacroProgram.Push(stop);
          doRangeMacroProgram.Push("exec.do*range");
          doRangeMacroProgram.Push(bodyObj);
          estack.Push(doRangeMacroProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
      }
    }
  }
}

internal class ExecDoCount : ObjectStackInstruction {
  internal ExecDoCount(Interpreter inI)
  : base(inI.ExecStack()) {}

  public override void Execute(Interpreter inI) {
    IntStack istack = inI.IntStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 0 && istack.Size() > 0) {
      if (istack.Top() > 0) {
        int stop = istack.Pop() - 1;
        object bodyObj = _stack.Pop();
        try {
          Program doRangeMacroProgram = new Program();
          doRangeMacroProgram.Push(0);
          doRangeMacroProgram.Push(stop);
          doRangeMacroProgram.Push("exec.do*range");
          doRangeMacroProgram.Push(bodyObj);
          estack.Push(doRangeMacroProgram);
        } catch (Exception) {
          Console.Error.WriteLine("Error while initializing a program.");
        }
      }
    }
  }
}

internal class ExecK : ObjectStackInstruction {
  internal ExecK(ObjectStack inStack)
  : base(inStack) {}

  // End exec iteration functions.
  public override void Execute(Interpreter inI) {
    // Removes the second item on the stack
    if (_stack.Size() > 1) {
      _stack.Swap();
      _stack.Popdiscard();
    }
  }
}

internal class ExecYield : Instruction {
  // End exec iteration functions.
  public void Execute(Interpreter inI) {
    inI.Yield();
  }
}

internal class ExecS : ObjectStackInstruction {
  internal int _maxPointsInProgram;

  internal ExecS(ObjectStack inStack, int inMaxPointsInProgram)
  : base(inStack) {
    _maxPointsInProgram = inMaxPointsInProgram;
  }

  public override void Execute(Interpreter inI) {
    // Removes the second item on the stack
    if (_stack.Size() > 2) {
      object a = _stack.Pop();
      object b = _stack.Pop();
      object c = _stack.Pop();
      Program listBC = new Program();
      listBC.Push(b);
      listBC.Push(c);
      if (listBC.ProgramSize() > _maxPointsInProgram) {
        // If the new list is too large, turn into a noop by re-pushing
        // the popped instructions
        _stack.Push(c);
        _stack.Push(b);
        _stack.Push(a);
      } else {
        // If not too big, continue as planned
        _stack.Push(listBC);
        _stack.Push(c);
        _stack.Push(a);
      }
    }
  }
}

internal class ExecY : ObjectStackInstruction {
  internal ExecY(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    // Removes the second item on the stack
    if (_stack.Size() > 0) {
      object a = _stack.Pop();
      Program listExecYA = new Program();
      listExecYA.Push("exec.y");
      listExecYA.Push(a);
      _stack.Push(listExecYA);
      _stack.Push(a);
    }
  }
}

internal class ExecNoop : Instruction {
  public void Execute(Interpreter inI) {}
}

internal class RandomPushCode : ObjectStackInstruction {
  internal Random Rng;

  internal RandomPushCode(ObjectStack inStack)
  : base(inStack) {
    Rng = new Random();
  }

  public override void Execute(Interpreter inI) {
    int randCodeMaxPoints = 0;
    if (inI.IntStack().Size() > 0) {
      randCodeMaxPoints = inI.IntStack().Pop();
      randCodeMaxPoints = Math.Min(Math.Abs(randCodeMaxPoints), inI._maxRandomCodeSize);
      int randomCodeSize;
      if (randCodeMaxPoints > 0) {
        randomCodeSize = Rng.Next(randCodeMaxPoints) + 2;
      } else {
        randomCodeSize = 2;
      }
      Program p = inI.RandomCode(randomCodeSize);
      _stack.Push(p);
    }
  }
}

internal class ObjectEquals : ObjectStackInstruction {
  internal ObjectEquals(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    BooleanStack bstack = inI.BoolStack();
    if (_stack.Size() > 1) {
      object o1 = _stack.Pop();
      object o2 = _stack.Pop();
      bstack.Push(o1.Equals(o2));
    }
  }
}

internal class IF : ObjectStackInstruction {
  internal IF(ObjectStack inStack)
  : base(inStack) {}

  public override void Execute(Interpreter inI) {
    BooleanStack bstack = inI.BoolStack();
    ObjectStack estack = inI.ExecStack();
    if (_stack.Size() > 1 && bstack.Size() > 0) {
      bool istrue = bstack.Pop();
      object iftrue = _stack.Pop();
      object iffalse = _stack.Pop();
      if (istrue) {
        estack.Push(iftrue);
      } else {
        estack.Push(iffalse);
      }
    }
  }
}

}
