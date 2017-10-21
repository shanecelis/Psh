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

namespace Psh
{
  /// <summary>
  /// Abstract instruction class for instructions which operate on any of the
  /// built-in stacks.
  /// </summary>
  internal abstract class StackInstruction : Instruction
  {

    protected internal Stack _stack;

    internal StackInstruction(Stack inStack)
    {
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
  internal abstract class ObjectStackInstruction : Instruction
  {

    protected internal ObjectStack _stack;

    internal ObjectStackInstruction(ObjectStack inStack)
    {
      _stack = inStack;
    }

    public abstract void Execute(Interpreter inI);
  }

  internal class Quote : Instruction
  {

    internal Quote()
    {
    }

    public void Execute(Interpreter inI)
    {
      ObjectStack cstack = inI.CodeStack();
      ObjectStack estack = inI.ExecStack();
      if (estack.Size() > 0)
      {
        cstack.Push(estack.Pop());
      }
    }
  }

  internal class Pop : StackInstruction
  {

    internal Pop(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      if (_stack.Size() > 0)
      {
        _stack.Popdiscard();
      }
    }
  }

  internal class Flush : StackInstruction
  {

    internal Flush(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      _stack.Clear();
    }
  }

  internal class Dup : StackInstruction
  {

    internal Dup(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      _stack.Dup();
    }
  }

  internal class Rot : StackInstruction
  {

    internal Rot(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      if (_stack.Size() > 2)
      {
        _stack.Rot();
      }
    }
  }

  internal class Shove : StackInstruction
  {

    internal Shove(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        int index = iStack.Pop();
        if (_stack.Size() > 0)
        {
          _stack.Shove(index);
        }
        else
        {
          iStack.Push(index);
        }
      }
    }
  }

  internal class Swap : StackInstruction
  {

    internal Swap(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      if (_stack.Size() > 1)
      {
        _stack.Swap();
      }
    }
  }

  internal class Yank : StackInstruction
  {

    internal Yank(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        int index = iStack.Pop();
        if (_stack.Size() > 0)
        {
          _stack.Yank(index);
        }
        else
        {
          iStack.Push(index);
        }
      }
    }
  }

  internal class YankDup : StackInstruction
  {

    internal YankDup(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        int index = iStack.Pop();
        if (_stack.Size() > 0)
        {
          _stack.Yankdup(index);
        }
        else
        {
          iStack.Push(index);
        }
      }
    }
  }

  internal class Depth : StackInstruction
  {

    internal Depth(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack stack = inI.IntStack();
      stack.Push(_stack.Size());
    }
  }

  internal class IntegerConstant : Instruction
  {

    internal int _value;

    public IntegerConstant(int inValue)
    {
      _value = inValue;
    }

    public void Execute(Interpreter inI)
    {
      inI.IntStack().Push(_value);
    }
  }

  internal class FloatConstant : Instruction
  {

    internal float _value;

    public FloatConstant(float inValue)
    {
      _value = inValue;
    }

    public void Execute(Interpreter inI)
    {
      inI.FloatStack().Push(_value);
    }
  }

  internal class BooleanConstant : Instruction
  {

    internal bool _value;

    public BooleanConstant(bool inValue)
    {
      _value = inValue;
    }

    public void Execute(Interpreter inI)
    {
      inI.BoolStack().Push(_value);
    }
  }

  internal class ObjectConstant : ObjectStackInstruction
  {

    internal object _value;

    public ObjectConstant(ObjectStack inStack, object inValue)
      : base(inStack)
    {
      _value = inValue;
    }

    public override void Execute(Interpreter inI)
    {
      _stack.Push(_value);
    }
  }

  internal abstract class BinaryIntegerInstruction : Instruction
  {
    //
    //
    // Binary integer instructions
    //
    internal abstract int BinaryOperator(int inA, int inB);

    public void Execute(Interpreter inI)
    {
      IntStack stack = inI.IntStack();
      if (stack.Size() > 1)
      {
        int a;
        int b;
        a = stack.Pop();
        b = stack.Pop();
        stack.Push(BinaryOperator(b, a));
      }
    }
  }

internal class BinaryInstruction<T> : Instruction
{

  private Func<T,T,T> func;
  public BinaryInstruction(Func<T,T,T> func) {
    this.func = func;
  }

  // Binary integer instructions
  //
  // internal abstract T BinaryOperator(T inA, T inB);

  public void Execute(Interpreter inI)
  {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 1)
    {
      T a, b, c;
      a = stack.Pop();
      b = stack.Pop();
      try {
        c = func(b, a);
        stack.Push(c);
      } catch (ArithmeticException) {
        c = default(T);
      } catch (Exception e) {
        throw new Exception("Instruction failed for arguments " + a + " and " + b, e);
      }
      stack.Push(c);
    }
  }

  // public static implicit operator BinaryInstruction<T>(Func<T,T,T> f) {
  //   return new BinaryInstruction<T>(f);
  // }
}

internal class UnaryInstruction<T> : Instruction
{
  private Func<T,T> func;

  public UnaryInstruction(Func<T,T> func)
  {
    this.func = func;
  }

  public void Execute(Interpreter inI)
  {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 0)
    {
      stack.Push(func(stack.Pop()));
    }
  }
}

internal class UnaryInstruction<inT,outT> : Instruction
{
  private Func<inT,outT> func;

  public UnaryInstruction(Func<inT,outT> func)
  {
    this.func = func;
  }

  public void Execute(Interpreter inI)
  {
    var istack = inI.GetStack<inT>();
    var ostack = inI.GetStack<outT>();
    if (istack.Size() > 0)
    {
      ostack.Push(func(istack.Pop()));
    }
  }
}

  internal abstract class UnaryIntInstruction : Instruction
  {
    //
    //Unary int instructions
    //
    internal abstract int UnaryOperator(int inValue);

    public void Execute(Interpreter inI)
    {
      IntStack stack = inI.IntStack();
      if (stack.Size() > 0)
      {
        stack.Push(UnaryOperator(stack.Pop()));
      }
    }
  }

  internal class IntegerRand : Instruction
  {
    internal Random Rng;

    internal IntegerRand()
    {
      Rng = new Random();
    }

    public void Execute(Interpreter inI)
    {
      int range = (inI._maxRandomInt - inI._minRandomInt) / inI._randomIntResolution;
      int randInt = (Rng.Next(range) * inI._randomIntResolution) + inI._minRandomInt;
      inI.IntStack().Push(randInt);
    }
  }

internal class BinaryInstruction<inT,outT> : Instruction
{
  private Func<inT,inT,outT> func;

  public BinaryInstruction(Func<inT,inT,outT> func) {
    this.func = func;
  }

  public void Execute(Interpreter inI)
  {
    var istack = inI.GetStack<inT>();
    var ostack = inI.GetStack<outT>();
    if (istack.Size() > 1)
    {
      inT a;
      inT b;
      a = istack.Pop();
      b = istack.Pop();
      ostack.Push(func(b, a));
    }
  }
}


  internal abstract class BinaryFloatInstruction : Instruction
  {
    //
    // Binary float instructions with float output
    //
    internal abstract float BinaryOperator(float inA, float inB);

    public void Execute(Interpreter inI)
    {
      FloatStack stack = inI.FloatStack();
      if (stack.Size() > 1)
      {
        float a;
        float b;
        a = stack.Pop();
        b = stack.Pop();
        stack.Push(BinaryOperator(b, a));
      }
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

  internal class FloatRand : Instruction
  {
    internal Random Rng;

    internal FloatRand()
    {
      Rng = new Random();
    }

    public void Execute(Interpreter inI)
    {
      float range = (inI._maxRandomFloat - inI._minRandomFloat) / inI._randomFloatResolution;
      float randFloat = ((float)Rng.NextDouble() * range * inI._randomFloatResolution) + inI._minRandomFloat;
      inI.FloatStack().Push(randFloat);
    }
  }

  internal class BoolRand : Instruction
  {
    internal Random Rng;

    internal BoolRand()
    {
      Rng = new Random();
    }

    public void Execute(Interpreter inI)
    {
      inI.BoolStack().Push(Rng.Next(2) == 1);
    }
  }

  internal class InputInN : Instruction
  {
    protected internal int index;

    internal InputInN(int inIndex)
    {
      //
      // Instructions for input stack
      //
      index = inIndex;
    }

    public void Execute(Interpreter inI)
    {
      inI.GetInputPusher().PushInput(inI, index);
    }
  }

  internal class InputInAll : ObjectStackInstruction
  {
    internal InputInAll(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      if (_stack.Size() > 0)
      {
        for (int index = 0; index < _stack.Size(); index++)
        {
          inI.GetInputPusher().PushInput(inI, index);
        }
      }
    }
  }

  internal class InputInRev : ObjectStackInstruction
  {
    internal InputInRev(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      if (_stack.Size() > 0)
      {
        for (int index = _stack.Size() - 1; index >= 0; index--)
        {
          inI.GetInputPusher().PushInput(inI, index);
        }
      }
    }
  }

  internal class InputIndex : ObjectStackInstruction
  {
    internal InputIndex(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      if (istack.Size() > 0 && _stack.Size() > 0)
      {
        int index = istack.Pop();
        if (index < 0)
        {
          index = 0;
        }
        if (index >= _stack.Size())
        {
          index = _stack.Size() - 1;
        }
        inI.GetInputPusher().PushInput(inI, index);
      }
    }
  }

  internal class CodeDoRange : ObjectStackInstruction
  {
    internal CodeDoRange(Interpreter inI)
      : base(inI.CodeStack())
    {
    }

    //
    // Instructions for code and exec stack
    //
    // trh//All code and exec stack iteration fuctions have been fixed to match the
    // specifications of Push 3.0
    // Begin code iteration functions
    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 1)
      {
        int stop = istack.Pop();
        int start = istack.Pop();
        object code = _stack.Pop();
        if (start == stop)
        {
          istack.Push(start);
          estack.Push(code);
        }
        else
        {
          istack.Push(start);
          start = (start < stop) ? (start + 1) : (start - 1);
          try
          {
            Program recursiveCallProgram = new Program(inI);
            recursiveCallProgram.Push(start);
            recursiveCallProgram.Push(stop);
            recursiveCallProgram.Push("code.quote");
            recursiveCallProgram.Push(code);
            recursiveCallProgram.Push("code.do*range");
            estack.Push(recursiveCallProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
          estack.Push(code);
        }
      }
    }
  }

  internal class CodeDoTimes : ObjectStackInstruction
  {
    internal CodeDoTimes(Interpreter inI)
      : base(inI.CodeStack())
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 0)
      {
        if (istack.Top() > 0)
        {
          object bodyObj = _stack.Pop();
          if (bodyObj is Program)
          {
            // insert integer.pop in front of program
            ((Program)bodyObj).Shove("integer.pop", ((Program)bodyObj)._size);
          }
          else
          {
            // create a new program with integer.pop in front of
            // the popped object
            Program newProgram = new Program(inI);
            newProgram.Push("integer.pop");
            newProgram.Push(bodyObj);
            bodyObj = newProgram;
          }
          int stop = istack.Pop() - 1;
          try
          {
            Program doRangeMacroProgram = new Program(inI);
            doRangeMacroProgram.Push(0);
            doRangeMacroProgram.Push(stop);
            doRangeMacroProgram.Push("code.quote");
            doRangeMacroProgram.Push(bodyObj);
            doRangeMacroProgram.Push("code.do*range");
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
        }
      }
    }
  }

  internal class CodeDoCount : ObjectStackInstruction
  {
    internal CodeDoCount(Interpreter inI)
      : base(inI.CodeStack())
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 0)
      {
        if (istack.Top() > 0)
        {
          int stop = istack.Pop() - 1;
          object bodyObj = _stack.Pop();
          try
          {
            Program doRangeMacroProgram = new Program(inI);
            doRangeMacroProgram.Push(0);
            doRangeMacroProgram.Push(stop);
            doRangeMacroProgram.Push("code.quote");
            doRangeMacroProgram.Push(bodyObj);
            doRangeMacroProgram.Push("code.do*range");
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
        }
      }
    }
  }

  internal class CodeFromBoolean : Instruction
  {
    // End code iteration functions
    //
    // Conversion instructions to code
    //
    public void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      BooleanStack bStack = inI.BoolStack();
      if (bStack.Size() > 0)
      {
        codeStack.Push(bStack.Pop());
      }
    }
  }

  internal class CodeFromInteger : Instruction
  {
    public void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        codeStack.Push(iStack.Pop());
      }
    }
  }

  internal class CodeFromFloat : Instruction
  {
    public void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      FloatStack fStack = inI.FloatStack();
      if (fStack.Size() > 0)
      {
        codeStack.Push(fStack.Pop());
      }
    }
  }

  internal class ExecDoRange : ObjectStackInstruction
  {
    internal ExecDoRange(Interpreter inI)
      : base(inI.ExecStack())
    {
    }

    // Begin exec iteration functions
    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 1)
      {
        int stop = istack.Pop();
        int start = istack.Pop();
        object code = _stack.Pop();
        if (start == stop)
        {
          istack.Push(start);
          estack.Push(code);
        }
        else
        {
          istack.Push(start);
          start = (start < stop) ? (start + 1) : (start - 1);
          // trh//Made changes to correct errors with code.do*range
          try
          {
            Program recursiveCallProgram = new Program(inI);
            recursiveCallProgram.Push(start);
            recursiveCallProgram.Push(stop);
            recursiveCallProgram.Push("exec.do*range");
            recursiveCallProgram.Push(code);
            estack.Push(recursiveCallProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
          estack.Push(code);
        }
      }
    }
  }

  internal class ExecDoTimes : ObjectStackInstruction
  {
    internal ExecDoTimes(Interpreter inI)
      : base(inI.ExecStack())
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 0)
      {
        if (istack.Top() > 0)
        {
          object bodyObj = _stack.Pop();
          if (bodyObj is Program)
          {
            // insert integer.pop in front of program
            ((Program)bodyObj).Shove("integer.pop", ((Program)bodyObj)._size);
          }
          else
          {
            // create a new program with integer.pop in front of
            // the popped object
            Program newProgram = new Program(inI);
            newProgram.Push("integer.pop");
            newProgram.Push(bodyObj);
            bodyObj = newProgram;
          }
          int stop = istack.Pop() - 1;
          try
          {
            Program doRangeMacroProgram = new Program(inI);
            doRangeMacroProgram.Push(0);
            doRangeMacroProgram.Push(stop);
            doRangeMacroProgram.Push("exec.do*range");
            doRangeMacroProgram.Push(bodyObj);
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
        }
      }
    }
  }

  internal class ExecDoCount : ObjectStackInstruction
  {
    internal ExecDoCount(Interpreter inI)
      : base(inI.ExecStack())
    {
    }

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 0 && istack.Size() > 0)
      {
        if (istack.Top() > 0)
        {
          int stop = istack.Pop() - 1;
          object bodyObj = _stack.Pop();
          try
          {
            Program doRangeMacroProgram = new Program(inI);
            doRangeMacroProgram.Push(0);
            doRangeMacroProgram.Push(stop);
            doRangeMacroProgram.Push("exec.do*range");
            doRangeMacroProgram.Push(bodyObj);
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            Console.Error.WriteLine("Error while initializing a program.");
          }
        }
      }
    }
  }

  internal class ExecK : ObjectStackInstruction
  {
    internal ExecK(ObjectStack inStack)
      : base(inStack)
    {
    }

    // End exec iteration functions.
    public override void Execute(Interpreter inI)
    {
      // Removes the second item on the stack
      if (_stack.Size() > 1)
      {
        _stack.Swap();
        _stack.Popdiscard();
      }
    }
  }

internal class ExecYield : Instruction
{
  // End exec iteration functions.
  public void Execute(Interpreter inI)
  {
    inI.stop = true;
  }
}

  internal class ExecS : ObjectStackInstruction
  {
    internal int _maxPointsInProgram;

    internal ExecS(ObjectStack inStack, int inMaxPointsInProgram)
      : base(inStack)
    {
      _maxPointsInProgram = inMaxPointsInProgram;
    }

    public override void Execute(Interpreter inI)
    {
      // Removes the second item on the stack
      if (_stack.Size() > 2)
      {
        object a = _stack.Pop();
        object b = _stack.Pop();
        object c = _stack.Pop();
        Program listBC = new Program(inI);
        listBC.Push(b);
        listBC.Push(c);
        if (listBC.Programsize() > _maxPointsInProgram)
        {
          // If the new list is too large, turn into a noop by re-pushing
          // the popped instructions
          _stack.Push(c);
          _stack.Push(b);
          _stack.Push(a);
        }
        else
        {
          // If not too big, continue as planned
          _stack.Push(listBC);
          _stack.Push(c);
          _stack.Push(a);
        }
      }
    }
  }

  internal class ExecY : ObjectStackInstruction
  {
    internal ExecY(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      // Removes the second item on the stack
      if (_stack.Size() > 0)
      {
        object a = _stack.Pop();
        Program listExecYA = new Program(inI);
        listExecYA.Push("exec.y");
        listExecYA.Push(a);
        _stack.Push(listExecYA);
        _stack.Push(a);
      }
    }
  }

  internal class ExecNoop : Instruction
  {
    public void Execute(Interpreter inI)
    {
      // Does Nothing
    }
  }

  internal class RandomPushCode : ObjectStackInstruction
  {
    internal Random Rng;

    internal RandomPushCode(ObjectStack inStack)
      : base(inStack)
    {
      Rng = new Random();
    }

    public override void Execute(Interpreter inI)
    {
      int randCodeMaxPoints = 0;
      if (inI.IntStack().Size() > 0)
      {
        randCodeMaxPoints = inI.IntStack().Pop();
        randCodeMaxPoints = Math.Min(Math.Abs(randCodeMaxPoints), inI._maxRandomCodeSize);
        int randomCodeSize;
        if (randCodeMaxPoints > 0)
        {
          randomCodeSize = Rng.Next(randCodeMaxPoints) + 2;
        }
        else
        {
          randomCodeSize = 2;
        }
        Program p = inI.RandomCode(randomCodeSize);
        _stack.Push(p);
      }
    }
  }

  internal class ObjectEquals : ObjectStackInstruction
  {
    internal ObjectEquals(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      BooleanStack bstack = inI.BoolStack();
      if (_stack.Size() > 1)
      {
        object o1 = _stack.Pop();
        object o2 = _stack.Pop();
        bstack.Push(o1.Equals(o2));
      }
    }
  }

  internal class IF : ObjectStackInstruction
  {
    internal IF(ObjectStack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      BooleanStack bstack = inI.BoolStack();
      ObjectStack estack = inI.ExecStack();
      if (_stack.Size() > 1 && bstack.Size() > 0)
      {
        bool istrue = bstack.Pop();
        object iftrue = _stack.Pop();
        object iffalse = _stack.Pop();
        if (istrue)
        {
          estack.Push(iftrue);
        }
        else
        {
          estack.Push(iffalse);
        }
      }
    }
  }

  internal class PopFrame : Instruction
  {
    internal PopFrame()
    {
    }

    //
    // Instructions for the activation stack
    //
    public void Execute(Interpreter inI)
    {
      // floatStack fstack = inI.floatStack();
      // float total = fstack.accumulate();
      inI.PopFrame();
    }
    // do the activation, and push the result on to the end of the previous
    // frame
    // fstack = inI.floatStack();
    // fstack.push( 1.0f / ( 1.0f + (float)Math.exp( -10.0f * ( total - .5 )
    // ) ) );
  }

  internal class PushFrame : Instruction
  {
    internal PushFrame()
    {
    }

    public void Execute(Interpreter inI)
    {
      inI.PushFrame();
    }
  }
}
