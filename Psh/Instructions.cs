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
using Sharpen;

namespace Psh
{
  /// <summary>
  /// Abstract instruction class for instructions which operate on any of the
  /// built-in stacks.
  /// </summary>
  [System.Serializable]
  internal abstract class StackInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    protected internal Stack _stack;

    internal StackInstruction(Stack inStack)
    {
      //
      // All instructions 
      //
      _stack = inStack;
    }
  }

  /// <summary>
  /// Abstract instruction class for instructions which operate on one of the
  /// standard ObjectStacks (code & exec).
  /// </summary>
  [System.Serializable]
  internal abstract class ObjectStackInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    protected internal ObjectStack _stack;

    internal ObjectStackInstruction(ObjectStack inStack)
    {
      _stack = inStack;
    }
  }

  [System.Serializable]
  internal class Quote : Instruction
  {
    private const long serialVersionUID = 1L;

    internal Quote()
    {
    }

    public override void Execute(Interpreter inI)
    {
      ObjectStack cstack = inI.CodeStack();
      ObjectStack estack = inI.ExecStack();
      if (estack.Size() > 0)
      {
        cstack.Push(estack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class Pop : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class Flush : StackInstruction
  {
    private const long serialVersionUID = 1L;

    internal Flush(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      _stack.Clear();
    }
  }

  [System.Serializable]
  internal class Dup : StackInstruction
  {
    private const long serialVersionUID = 1L;

    internal Dup(Stack inStack)
      : base(inStack)
    {
    }

    public override void Execute(Interpreter inI)
    {
      _stack.Dup();
    }
  }

  [System.Serializable]
  internal class Rot : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class Shove : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class Swap : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class Yank : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class YankDup : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class Depth : StackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class IntegerConstant : Instruction
  {
    private const long serialVersionUID = 1L;

    internal int _value;

    public IntegerConstant(int inValue)
    {
      _value = inValue;
    }

    public override void Execute(Interpreter inI)
    {
      inI.IntStack().Push(_value);
    }
  }

  [System.Serializable]
  internal class FloatConstant : Instruction
  {
    private const long serialVersionUID = 1L;

    internal float _value;

    public FloatConstant(float inValue)
    {
      _value = inValue;
    }

    public override void Execute(Interpreter inI)
    {
      inI.FloatStack().Push(_value);
    }
  }

  [System.Serializable]
  internal class BooleanConstant : Instruction
  {
    private const long serialVersionUID = 1L;

    internal bool _value;

    public BooleanConstant(bool inValue)
    {
      _value = inValue;
    }

    public override void Execute(Interpreter inI)
    {
      inI.BoolStack().Push(_value);
    }
  }

  [System.Serializable]
  internal class ObjectConstant : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal abstract class BinaryIntegerInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    //
    // Binary integer instructions
    //
    internal abstract int BinaryOperator(int inA, int inB);

    public override void Execute(Interpreter inI)
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

[System.Serializable]
internal class BinaryInstruction<T> : Instruction
{
  private const long serialVersionUID = 1L;

  private Func<T,T,T> func;
  public BinaryInstruction(Func<T,T,T> func) {
    this.func = func;
  }

  //
  //
  // Binary integer instructions
  //
  // internal abstract T BinaryOperator(T inA, T inB);

  public override void Execute(Interpreter inI)
  {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 1)
    {
      T a;
      T b;
      a = stack.Pop();
      b = stack.Pop();
      stack.Push(func(b, a));
    }
  }

  public static implicit operator BinaryInstruction<T>(Func<T,T,T> f) {
    return new BinaryInstruction<T>(f);
  }
}


  [System.Serializable]
  internal class IntegerAdd : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      // Test for overflow
      if (inA.WillAdditionOverflow(inB))
      // if ((Math.Abs(inA) > int.MaxValue / 10) || (Math.Abs(inB) > int.MaxValue / 10))
      {
        long lA = (long)inA;
        long lB = (long)inB;
        if (lA + lB != inA + inB)
        {
          if (inA > 0)
          {
            return int.MaxValue;
          }
          else
          {
            return int.MinValue;
          }
        }
      }
      return inA + inB;
    }
  }

  [System.Serializable]
  internal class IntegerSub : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      if (inA.WillSubtractionUnderflow(inB))
      // Test for overflow
      // if ((Math.Abs(inA) > int.MaxValue / 10) || (Math.Abs(inB) > int.MaxValue / 10))
      {
        long lA = (long)inA;
        long lB = (long)inB;
        if (lA - lB != inA - inB)
        {
          if (inA > 0)
          {
            return int.MaxValue;
          }
          else
          {
            return int.MinValue;
          }
        }
      }
      return inA - inB;
    }
  }

  [System.Serializable]
  internal class IntegerDiv : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      // return inB != 0 && ! inA.WillDivisionUnderflow(inB) ? (inA / inB) : 0;
      try {
        return inB != 0 ? checked(inA / inB) : 0;
      } catch (OverflowException) {
        return 0;
      }
    }
  }

  [System.Serializable]
  internal class IntegerMul : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      // Test for overflow
      if (inA.WillMultiplicationOverflow(inB))
      // if ((Math.Abs(inA) > Math.Sqrt(int.MaxValue - 1)) || (Math.Abs(inB) > Math.Sqrt(int.MaxValue - 1)))
      {
        long lA = (long)inA;
        long lB = (long)inB;
        if (lA * lB != inA * inB)
        {
          if ((inA > 0 && inB > 0) || (inA < 0 && inB < 0))
          {
            return int.MaxValue;
          }
          else
          {
            return int.MinValue;
          }
        }
      }
      return inA * inB;
    }
  }

  [System.Serializable]
  internal class IntegerMod : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      return inB != 0 && ! inA.WillModuloOverflow(inB) ? (inA % inB) : 0;
    }
  }

  [System.Serializable]
  internal class IntegerPow : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      // Test for overflow
      double result = Math.Pow(inA, inB);
      if (double.IsInfinity(result) && result > 0)
      {
        return int.MaxValue;
      }
      if (double.IsInfinity(result) && result < 0)
      {
        return int.MinValue;
      }
      if (double.IsNaN(result))
      {
        return 0;
      }
      return (int)result;
    }
  }

  [System.Serializable]
  internal class IntegerLog : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      // Test for overflow
      double result = Math.Log(inB) / Math.Log(inA);
      if (double.IsInfinity(result) && result > 0)
      {
        return int.MaxValue;
      }
      if (double.IsInfinity(result) && result < 0)
      {
        return int.MinValue;
      }
      if (double.IsNaN(result))
      {
        return 0;
      }
      return (int)result;
    }
  }

  [System.Serializable]
  internal class IntegerMin : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      return Math.Min(inA, inB);
    }
  }

  [System.Serializable]
  internal class IntegerMax : BinaryIntegerInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int BinaryOperator(int inA, int inB)
    {
      return Math.Max(inA, inB);
    }
  }


[System.Serializable]
internal class UnaryInstruction<T> : Instruction
{
  private const long serialVersionUID = 1L;
  private Func<T,T> func;

  public UnaryInstruction(Func<T,T> func)
  {
    this.func = func;
  }

  public override void Execute(Interpreter inI)
  {
    GenericStack<T> stack = inI.GetStack<T>();
    if (stack.Size() > 0)
    {
      stack.Push(func(stack.Pop()));
    }
  }
}

[System.Serializable]
internal class UnaryInstruction<inT,outT> : Instruction
{
  private const long serialVersionUID = 1L;
  private Func<inT,outT> func;

  public UnaryInstruction(Func<inT,outT> func)
  {
    this.func = func;
  }

  public override void Execute(Interpreter inI)
  {
    var istack = inI.GetStack<inT>();
    var ostack = inI.GetStack<outT>();
    if (istack.Size() > 0)
    {
      ostack.Push(func(istack.Pop()));
    }
  }
}

  [System.Serializable]
  internal abstract class UnaryIntInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    //Unary int instructions
    //
    internal abstract int UnaryOperator(int inValue);

    public override void Execute(Interpreter inI)
    {
      IntStack stack = inI.IntStack();
      if (stack.Size() > 0)
      {
        stack.Push(UnaryOperator(stack.Pop()));
      }
    }
  }

  [System.Serializable]
  internal class IntegerAbs : UnaryIntInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int UnaryOperator(int inValue)
    {
      if(inValue == int.MinValue) return int.MaxValue;
      return Math.Abs(inValue);
    }
  }

  [System.Serializable]
  internal class IntegerNeg : UnaryIntInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int UnaryOperator(int inValue)
    {
      // Test for overflow
      if (inValue == int.MinValue)
      {
        return int.MaxValue;
      }
      return -inValue;
    }
  }

  [System.Serializable]
  internal class IntegerLn : UnaryIntInstruction
  {
    private const long serialVersionUID = 1L;

    internal override int UnaryOperator(int inA)
    {
      // Test for overflow
      double result = Math.Log(inA);
      if (double.IsInfinity(result) && result > 0)
      {
        return int.MaxValue;
      }
      if (double.IsInfinity(result) && result < 0)
      {
        return int.MinValue;
      }
      if (double.IsNaN(result))
      {
        return 0;
      }
      return (int)result;
    }
  }

  [System.Serializable]
  internal class IntegerRand : Instruction
  {
    private const long serialVersionUID = 1L;

    internal Random Rng;

    internal IntegerRand()
    {
      Rng = new Random();
    }

    public override void Execute(Interpreter inI)
    {
      int range = (inI._maxRandomInt - inI._minRandomInt) / inI._randomIntResolution;
      int randInt = (Rng.Next(range) * inI._randomIntResolution) + inI._minRandomInt;
      inI.IntStack().Push(randInt);
    }
  }

  [System.Serializable]
  internal class IntegerFromFloat : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Conversion instructions to integer
    //
    public override void Execute(Interpreter inI)
    {
      IntStack iStack = inI.IntStack();
      FloatStack fStack = inI.FloatStack();
      if (fStack.Size() > 0)
      {
        iStack.Push((int)fStack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class IntegerFromBoolean : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      BooleanStack bStack = inI.BoolStack();
      IntStack iStack = inI.IntStack();
      if (bStack.Size() > 0)
      {
        if (bStack.Pop())
        {
          iStack.Push(1);
        }
        else
        {
          iStack.Push(0);
        }
      }
    }
  }


[System.Serializable]
internal class BinaryInstruction<inT,outT> : Instruction
{
  private const long serialVersionUID = 1L;
  private Func<inT,inT,outT> func;

  public BinaryInstruction(Func<inT,inT,outT> func) {
    this.func = func;
  }

  public override void Execute(Interpreter inI)
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

[System.Serializable]
internal class BinaryBoolInstruction<T> : Instruction
{
  private const long serialVersionUID = 1L;
  private Func<T,T,bool> func;

  public BinaryBoolInstruction(Func<T,T,bool> func) {
    this.func = func;
  }
  
  public override void Execute(Interpreter inI)
  {
    var istack = inI.GetStack<T>();
    BooleanStack bstack = inI.BoolStack();
    if (istack.Size() > 1)
    {
      T a;
      T b;
      a = istack.Pop();
      b = istack.Pop();
      bstack.Push(func(b, a));
    }
  }
}

  [System.Serializable]
  internal abstract class BinaryIntegerBoolInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Integer instructions with boolean output
    //
    internal abstract bool BinaryOperator(int inA, int inB);

    public override void Execute(Interpreter inI)
    {
      IntStack istack = inI.IntStack();
      BooleanStack bstack = inI.BoolStack();
      if (istack.Size() > 1)
      {
        int a;
        int b;
        a = istack.Pop();
        b = istack.Pop();
        bstack.Push(BinaryOperator(b, a));
      }
    }
  }

  [System.Serializable]
  internal class IntegerGreaterThan : BinaryIntegerBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(int inA, int inB)
    {
      return inA > inB;
    }
  }

  [System.Serializable]
  internal class IntegerLessThan : BinaryIntegerBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(int inA, int inB)
    {
      return inA < inB;
    }
  }

  [System.Serializable]
  internal class IntegerEquals : BinaryIntegerBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(int inA, int inB)
    {
      return inA == inB;
    }
  }

  [System.Serializable]
  internal abstract class BinaryFloatInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Binary float instructions with float output
    //
    internal abstract float BinaryOperator(float inA, float inB);

    public override void Execute(Interpreter inI)
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

  [System.Serializable]
  internal class FloatAdd : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = inA + inB;
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatSub : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = inA - inB;
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      return inA - inB;
    }
  }

  [System.Serializable]
  internal class FloatMul : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = inA * inB;
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (float.IsNaN(result))
      {
        return 0.0f;
      }
      return inA * inB;
    }
  }

  [System.Serializable]
  internal class FloatDiv : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = inA / inB;
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (float.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatMod : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      return inB != 0.0f ? (inA % inB) : 0.0f;
    }
  }

  [System.Serializable]
  internal class FloatPow : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = (float)Math.Pow(inA, inB);
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (float.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatLog : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      // Test for overflow
      float result = (float)(Math.Log(inB) / Math.Log(inA));
      if (double.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (double.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (double.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatMin : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      return Math.Min(inA, inB);
    }
  }

  [System.Serializable]
  internal class FloatMax : BinaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float BinaryOperator(float inA, float inB)
    {
      return Math.Max(inA, inB);
    }
  }

  [System.Serializable]
  internal abstract class UnaryFloatInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Unary float instructions
    //
    internal abstract float UnaryOperator(float inValue);

    public override void Execute(Interpreter inI)
    {
      FloatStack stack = inI.FloatStack();
      if (stack.Size() > 0)
      {
        stack.Push(UnaryOperator(stack.Pop()));
      }
    }
  }

  [System.Serializable]
  internal class FloatSin : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      return (float)Math.Sin(inValue);
    }
  }

  [System.Serializable]
  internal class FloatCos : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      return (float)Math.Cos(inValue);
    }
  }

  [System.Serializable]
  internal class FloatTan : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      // Test for overflow
      float result = (float)Math.Tan(inValue);
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (float.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatExp : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      // Test for overflow
      float result = (float)Math.Exp(inValue);
      if (float.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (float.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (float.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatAbs : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      return (float)Math.Abs(inValue);
    }
  }

  [System.Serializable]
  internal class FloatNeg : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inValue)
    {
      return -inValue;
    }
  }

  [System.Serializable]
  internal class FloatLn : UnaryFloatInstruction
  {
    private const long serialVersionUID = 1L;

    internal override float UnaryOperator(float inA)
    {
      // Test for overflow
      float result = (float)Math.Log(inA);
      if (double.IsInfinity(result) && result > 0)
      {
        return float.MaxValue;
      }
      if (double.IsInfinity(result) && result < 0)
      {
        return (1.0f - float.MaxValue);
      }
      if (double.IsNaN(result))
      {
        return 0.0f;
      }
      return result;
    }
  }

  [System.Serializable]
  internal class FloatRand : Instruction
  {
    private const long serialVersionUID = 1L;

    internal Random Rng;

    internal FloatRand()
    {
      Rng = new Random();
    }

    public override void Execute(Interpreter inI)
    {
      float range = (inI._maxRandomFloat - inI._minRandomFloat) / inI._randomFloatResolution;
      float randFloat = ((float)Rng.NextDouble() * range * inI._randomFloatResolution) + inI._minRandomFloat;
      inI.FloatStack().Push(randFloat);
    }
  }

  [System.Serializable]
  internal class FloatFromInteger : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Conversion instructions to float
    //
    public override void Execute(Interpreter inI)
    {
      IntStack iStack = inI.IntStack();
      FloatStack fStack = inI.FloatStack();
      if (iStack.Size() > 0)
      {
        fStack.Push(iStack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class FloatFromBoolean : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      BooleanStack bStack = inI.BoolStack();
      FloatStack fStack = inI.FloatStack();
      if (bStack.Size() > 0)
      {
        if (bStack.Pop())
        {
          fStack.Push(1);
        }
        else
        {
          fStack.Push(0);
        }
      }
    }
  }

  [System.Serializable]
  internal abstract class BinaryFloatBoolInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Binary float instructions with boolean output
    //
    internal abstract bool BinaryOperator(float inA, float inB);

    public override void Execute(Interpreter inI)
    {
      FloatStack fstack = inI.FloatStack();
      BooleanStack bstack = inI.BoolStack();
      if (fstack.Size() > 1)
      {
        float a;
        float b;
        b = fstack.Pop();
        a = fstack.Pop();
        bstack.Push(BinaryOperator(a, b));
      }
    }
  }

  [System.Serializable]
  internal class FloatGreaterThan : BinaryFloatBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(float inA, float inB)
    {
      return inA > inB;
    }
  }

  [System.Serializable]
  internal class FloatLessThan : BinaryFloatBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(float inA, float inB)
    {
      return inA < inB;
    }
  }

  [System.Serializable]
  internal class FloatEquals : BinaryFloatBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(float inA, float inB)
    {
      return inA == inB;
    }
  }

  [System.Serializable]
  internal abstract class BinaryBoolInstruction : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    //Binary bool instructions with bool output
    //
    internal abstract bool BinaryOperator(bool inA, bool inB);

    public override void Execute(Interpreter inI)
    {
      BooleanStack stack = inI.BoolStack();
      if (stack.Size() > 1)
      {
        bool a;
        bool b;
        a = stack.Pop();
        b = stack.Pop();
        stack.Push(BinaryOperator(b, a));
      }
    }
  }

  [System.Serializable]
  internal class BoolEquals : BinaryBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(bool inA, bool inB)
    {
      return inA == inB;
    }
  }

  [System.Serializable]
  internal class BoolAnd : BinaryBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(bool inA, bool inB)
    {
      return inA & inB;
    }
  }

  [System.Serializable]
  internal class BoolOr : BinaryBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(bool inA, bool inB)
    {
      return inA | inB;
    }
  }

  [System.Serializable]
  internal class BoolXor : BinaryBoolInstruction
  {
    private const long serialVersionUID = 1L;

    internal override bool BinaryOperator(bool inA, bool inB)
    {
      return inA ^ inB;
    }
  }

  [System.Serializable]
  internal class BoolNot : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      if (inI.BoolStack().Size() > 0)
      {
        inI.BoolStack().Push(!inI.BoolStack().Pop());
      }
    }
  }

  [System.Serializable]
  internal class BoolRand : Instruction
  {
    private const long serialVersionUID = 1L;

    internal Random Rng;

    internal BoolRand()
    {
      Rng = new Random();
    }

    public override void Execute(Interpreter inI)
    {
      inI.BoolStack().Push(Rng.Next(2) == 1);
    }
  }

  [System.Serializable]
  internal class BooleanFromInteger : Instruction
  {
    private const long serialVersionUID = 1L;

    //
    // Conversion instructions to boolean
    //
    public override void Execute(Interpreter inI)
    {
      BooleanStack bStack = inI.BoolStack();
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        bStack.Push(iStack.Pop() != 0);
      }
    }
  }

  [System.Serializable]
  internal class BooleanFromFloat : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      BooleanStack bStack = inI.BoolStack();
      FloatStack fStack = inI.FloatStack();
      if (fStack.Size() > 0)
      {
        bStack.Push(fStack.Pop() != 0.0);
      }
    }
  }

  [System.Serializable]
  internal class InputInN : Instruction
  {
    private const long serialVersionUID = 1L;

    protected internal int index;

    internal InputInN(int inIndex)
    {
      //
      // Instructions for input stack
      //
      index = inIndex;
    }

    public override void Execute(Interpreter inI)
    {
      inI.GetInputPusher().PushInput(inI, index);
    }
  }

  [System.Serializable]
  internal class InputInAll : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class InputInRev : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class InputIndex : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class CodeDoRange : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            recursiveCallProgram.Push(Sharpen.Extensions.ValueOf(start));
            recursiveCallProgram.Push(Sharpen.Extensions.ValueOf(stop));
            recursiveCallProgram.Push("code.quote");
            recursiveCallProgram.Push(code);
            recursiveCallProgram.Push("code.do*range");
            estack.Push(recursiveCallProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
          estack.Push(code);
        }
      }
    }
  }

  [System.Serializable]
  internal class CodeDoTimes : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(0));
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(stop));
            doRangeMacroProgram.Push("code.quote");
            doRangeMacroProgram.Push(bodyObj);
            doRangeMacroProgram.Push("code.do*range");
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
        }
      }
    }
  }

  [System.Serializable]
  internal class CodeDoCount : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(0));
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(stop));
            doRangeMacroProgram.Push("code.quote");
            doRangeMacroProgram.Push(bodyObj);
            doRangeMacroProgram.Push("code.do*range");
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
        }
      }
    }
  }

  [System.Serializable]
  internal class CodeFromBoolean : Instruction
  {
    private const long serialVersionUID = 1L;

    // End code iteration functions
    //
    // Conversion instructions to code
    //
    public override void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      BooleanStack bStack = inI.BoolStack();
      if (bStack.Size() > 0)
      {
        codeStack.Push(bStack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class CodeFromInteger : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      IntStack iStack = inI.IntStack();
      if (iStack.Size() > 0)
      {
        codeStack.Push(iStack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class CodeFromFloat : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
      ObjectStack codeStack = inI.CodeStack();
      FloatStack fStack = inI.FloatStack();
      if (fStack.Size() > 0)
      {
        codeStack.Push(fStack.Pop());
      }
    }
  }

  [System.Serializable]
  internal class ExecDoRange : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            recursiveCallProgram.Push(Sharpen.Extensions.ValueOf(start));
            recursiveCallProgram.Push(Sharpen.Extensions.ValueOf(stop));
            recursiveCallProgram.Push("exec.do*range");
            recursiveCallProgram.Push(code);
            estack.Push(recursiveCallProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
          estack.Push(code);
        }
      }
    }
  }

  [System.Serializable]
  internal class ExecDoTimes : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(0));
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(stop));
            doRangeMacroProgram.Push("exec.do*range");
            doRangeMacroProgram.Push(bodyObj);
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
        }
      }
    }
  }

  [System.Serializable]
  internal class ExecDoCount : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(0));
            doRangeMacroProgram.Push(Sharpen.Extensions.ValueOf(stop));
            doRangeMacroProgram.Push("exec.do*range");
            doRangeMacroProgram.Push(bodyObj);
            estack.Push(doRangeMacroProgram);
          }
          catch (Exception)
          {
            System.Console.Error.Println("Error while initializing a program.");
          }
        }
      }
    }
  }

  [System.Serializable]
  internal class ExecK : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class ExecS : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class ExecY : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class ExecNoop : Instruction
  {
    private const long serialVersionUID = 1L;

    public override void Execute(Interpreter inI)
    {
    }
    // Does Nothing
  }

  [System.Serializable]
  internal class RandomPushCode : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class ObjectEquals : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class IF : ObjectStackInstruction
  {
    private const long serialVersionUID = 1L;

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

  [System.Serializable]
  internal class PopFrame : Instruction
  {
    private const long serialVersionUID = 1L;

    internal PopFrame()
    {
    }

    //
    // Instructions for the activation stack
    //
    public override void Execute(Interpreter inI)
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

  [System.Serializable]
  internal class PushFrame : Instruction
  {
    private const long serialVersionUID = 1L;

    internal PushFrame()
    {
    }

    public override void Execute(Interpreter inI)
    {
      inI.PushFrame();
    }
  }
}

// https://stackoverflow.com/questions/22612418/check-for-arithmetic-overflow-and-get-overflow-count
public static class OverflowExtensions
{
  public static bool WillAdditionOverflow(this byte b, int val)
  {
    return byte.MaxValue - b < val;
  }

  public static bool WillSubtractionUnderflow(this byte b, int val)
  {
    return b - byte.MinValue < val;
  }

  public static bool WillAdditionOverflow(this int b, int val)
  {
    return int.MaxValue - b < val;
  }

  public static bool WillSubtractionUnderflow(this int b, int val)
  {
    return b - int.MinValue < val;
  }

  public static bool WillMultiplicationOverflow(this int b, int val)
  {
    if (b == 0)
      return false;
    return int.MaxValue / b < val;
  }

  public static bool WillDivisionUnderflow(this int b, int val)
  {
    return b * int.MinValue < val;
  }

  public static bool WillModuloOverflow(this int b, int val)
  {
    try {
      var c = checked(b % val);
      return false;
    } catch (OverflowException) {
      return true;
    }
    // return b * int.MinValue < val;
  }
}
