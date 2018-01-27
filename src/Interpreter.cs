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
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Psh {
/// <summary>The Push language interpreter.</summary>
public class Interpreter {

  public Dictionary<string, Instruction> _instructions  = new Dictionary<string, Instruction>();

  public Dictionary<string, AtomGenerator> _generators  = new Dictionary<string, AtomGenerator>();

  protected internal Psh.IntStack         _intStack     = new Psh.IntStack();
  protected internal Psh.FloatStack       _floatStack   = new Psh.FloatStack();
  protected internal BooleanStack         _boolStack    = new BooleanStack();
  protected internal ObjectStack          _codeStack    = new ObjectStack();
  protected internal GenericStack<string> _nameStack    = new GenericStack<string>();
  protected internal ObjectStack          _execStack    = new ObjectStack();
  protected internal ObjectStack          _inputStack   = new ObjectStack();

  // This kind of looks like it wants to be a map, but I think the order is more important.
  protected internal List<Tuple<string, Stack>> _stacks = new List<Tuple<string, Stack>>();
  // protected internal GenericStack<OneOf<ProgramTree, ProgramAtom>> _execStack2 = new Generic<OneOf<ProgramTree, ProgramAtom>>();

  protected internal int _totalStepsTaken;

  protected internal long _evaluationExecutions = 0;

  // These parameters seem weird to have in the interpreter.

  // XXX This value needs to be set at the beginning somehow.
  // Or setting it is just more complicated.
  public bool caseSensitive = false;

  public FloatAtomGenerator randFloat;
  public IntAtomGenerator randInt;
  public BoolAtomGenerator randBool;
  // This is available so a parameters can be set on it by InspectorInput.cs:92.
  internal RandomPushCode randCode;
  public RandomProgram randProgram = new RandomProgram();
  internal ExecS execS;

  // XXX What is the input stack and pusher really for?
  protected internal InputPusher _inputPusher = new InputPusher();

  // XXX yield flag?
  private bool stop = false;
  private bool quoting = false;

  public Interpreter() {
    // All generators
    // Create the stacks.
    // This arraylist will hold all custom stacks that can be created by the
    // problem classes
    /* Since the _inputStack will not change after initialization, it will not
      * need a frame stack.
    */
    AddStack("exec",    _execStack);
    AddStack("code",    _codeStack);
    AddStack("integer", _intStack);
    AddStack("float",   _floatStack);
    AddStack("boolean", _boolStack);
    AddStack("name",    _nameStack);
    AddStack("input",   _inputStack);

    // Auto converted Java to C# using Sharpen but it could still use some more love.

    // How it used to be:
    // ------------------
    // DefineInstruction("integer.+", new IntegerAdd());

    // // The IntegerAdd class is defined like so:
    // internal class IntegerAdd : BinaryIntegerInstruction {
    //   internal override int BinaryOperator(int inA, int inB) {
    //     // Test for overflow
    //     if ((Math.Abs(inA) > int.MaxValue / 10) || (Math.Abs(inB) > int.MaxValue / 10)) {
    //       long lA = (long)inA;
    //       long lB = (long)inB;
    //       if (lA + lB != inA + inB) {
    //         if (inA > 0) {
    //           return int.MaxValue;
    //         } else {
    //           return int.MinValue;
    //         }
    //       }
    //     }
    //     return inA + inB;
    //   }
    // }
    // And there are a lot of these classes:
    // DefineInstruction("integer.-", new IntegerSub());
    // DefineInstruction("integer./", new IntegerDiv());
    // DefineInstruction("integer.%", new IntegerMod());
    // DefineInstruction("integer.*", new IntegerMul());
    // Eek.

    // How it is now:
    // --------------
    DefineInstruction("integer.+", (int a, int b) => unchecked(a + b));
    // Let's use some lambdas and unchecked arithmetic.

    // Sympathy for the Original Author, Jon Klein
    // -------------------------------------------
    //
    // Java has lambdas now, but it didn't when this code was written. Also I
    // don't believe there's a nice way just turning off Java's over- and
    // under-flow checking.

    _generators["float.erc"] = randFloat = new Interpreter.FloatAtomGenerator();
    _generators["integer.erc"] = randInt = new Interpreter.IntAtomGenerator();
    _generators["boolean.erc"] = randBool = new Interpreter.BoolAtomGenerator();

    DefineInstruction("integer.-", (int a, int b) => unchecked(a - b));
    // DefineInstruction("integer./", (int a, int b) => { return (b != 0 ? a / b : 0); } );
    DefineInstruction("integer./", (int a, int b) => unchecked(b != 0 ? (a / b) : 0));
    // DefineInstruction("integer.%", (int a, int b) => { unchecked { var c = (b != 0 ? (a % b) : 0); return c; } } );
    DefineInstruction("integer.%", (int a, int b) => unchecked(b != 0 ? (a % b) : 0));
    DefineInstruction("integer.*", (int a, int b) => unchecked(a * b));
    DefineInstruction("integer.pow", (int a, int b) => unchecked((int) Math.Pow(a, b)));
    DefineInstruction("integer.log", (int a, int b) => unchecked((int) Math.Log(a, b)));
    DefineInstruction("integer.=", (int a, int b) => (a == b));
    DefineInstruction("integer.>", (int a, int b) => (a > b));
    DefineInstruction("integer.<", (int a, int b) => (a < b));
    DefineInstruction("integer.min", (int a, int b) => (a < b ? a : b));
    DefineInstruction("integer.max", (int a, int b) => (a > b ? a : b));
    DefineInstruction("integer.abs", (int a, int b) => unchecked(a < 0 ? -a : a));
    DefineInstruction("integer.neg", (int a) => unchecked(-a));
    DefineInstruction("integer.log", (int a) => unchecked((int) Math.Log(a)));
    DefineInstruction("integer.define", (int i, string name) => { DefineConstant(name, i); });
    DefineInstruction("integer.ln", (int a) => unchecked((int) Math.Log(a)));
    DefineInstruction("integer.fromfloat", (float a) => (int) a);
    DefineInstruction("integer.fromboolean", (bool a) => a ? 1 : 0);
    DefineInstruction("integer.rand", () => randInt.GenerateT());

    DefineInstruction("float.+", (float a, float b) => unchecked(a + b));
    DefineInstruction("float.-", (float a, float b) => unchecked(a - b));
    DefineInstruction("float./", (float a, float b) => unchecked(b != 0f ? a / b : 0f));
    DefineInstruction("float.%", (float a, float b) => unchecked(b != 0f ? a % b : 0f));
    DefineInstruction("float.*", (float a, float b) => unchecked(a * b));
    DefineInstruction("float.pow", (float a, float b) => unchecked((float) Math.Pow(a, b)));
    DefineInstruction("float.log", (float a, float b) => unchecked((float) Math.Log(a, b)));
    DefineInstruction("float.=", (float a, float b) => unchecked(a == b));
    DefineInstruction("float.>", (float a, float b) => unchecked(a > b));
    DefineInstruction("float.<", (float a, float b) => unchecked(a < b));
    DefineInstruction("float.min", (float a, float b) => unchecked(a < b ? a : b));
    DefineInstruction("float.max", (float a, float b) => unchecked(a > b ? a : b));
    DefineInstruction("float.sin", (float a) => unchecked((float) Math.Sin(a)));
    DefineInstruction("float.cos", (float a) => unchecked((float) Math.Cos(a)));
    DefineInstruction("float.tan", (float a) => unchecked((float) Math.Tan(a)));
    DefineInstruction("float.exp", (float a) => unchecked((float) Math.Exp(a)));
    DefineInstruction("float.abs", (float a) => unchecked(a < 0 ? -a : a));
    DefineInstruction("float.neg", (float a) => unchecked(-a));
    DefineInstruction("float.ln", (float a) => unchecked((float) Math.Log(a)));
    DefineInstruction("float.frominteger", (int a) => (float) a);
    DefineInstruction("float.fromboolean", (bool a) => a ? 1f : 0f);
    DefineInstruction("float.rand", () => randFloat.GenerateT());
    DefineInstruction("float.define", (float i, string name) => { DefineConstant(name, i); });

    DefineInstruction("boolean.=", (bool a, bool b) => a == b);
    DefineInstruction("boolean.not", (bool a) => ! a);
    DefineInstruction("boolean.and", (bool a, bool b) => a & b);
    DefineInstruction("boolean.or", (bool a, bool b) => a | b);
    DefineInstruction("boolean.xor", (bool a, bool b) => a ^ b);
    DefineInstruction("boolean.frominteger", (int a) => a != 0);
    DefineInstruction("boolean.fromfloat", (float a) => a != 0f);
    DefineInstruction("boolean.rand", () => randBool.GenerateT());
    DefineInstruction("boolean.define", (bool b, string name) => { DefineConstant(name, b); });

    DefineInstruction("name.=", (string a, string b) => a == b);
    DefineInstruction("name.+", (string a, string b) => a + b);
    DefineInstruction("name.quote", () => { quoting = true; });

    DefineInstruction("code.quote", new Quote());
    DefineInstruction("code.rand", randCode = new RandomPushCode(_codeStack));
    DefineInstruction("code.fromboolean", new CodeFromBoolean());
    DefineInstruction("code.frominteger", new CodeFromInteger());
    DefineInstruction("code.fromfloat", new CodeFromFloat());
    DefineInstruction("code.noop", new ExecNoop());
    DefineInstruction("exec.k", new ExecK(_execStack));
    DefineInstruction("exec.s", execS = new ExecS(_execStack, 100/* default max points in program */));
    DefineInstruction("exec.y", new ExecY(_execStack));
    DefineInstruction("exec.yield", new ExecYield());
    DefineInstruction("exec.noop", new ExecNoop());
    DefineInstruction("exec.do*times", new ExecDoTimes(this));
    DefineInstruction("code.do*times", new CodeDoTimes(this));
    DefineInstruction("exec.do*count", new ExecDoCount(this));
    DefineInstruction("code.do*count", new CodeDoCount(this));
    DefineInstruction("exec.do*range", new ExecDoRange(this));
    DefineInstruction("code.do*range", new CodeDoRange(this));
    DefineInstruction("code.=", new ObjectEquals(_codeStack));
    DefineInstruction("exec.=", new ObjectEquals(_execStack));
    DefineInstruction("code.if", new IF(_codeStack));
    DefineInstruction("exec.if", new IF(_execStack));
    DefineInstruction("exec.rand", new RandomPushCode(_execStack));
    DefineInstruction("true", new Constant<bool>(true));
    DefineInstruction("false", new Constant<bool>(false));
    DefineInstruction("input.index", new InputIndex(_inputStack));
    DefineInstruction("input.inall", new InputInAll(_inputStack));
    DefineInstruction("input.inallrev", new InputInRev(_inputStack));
    DefineInstruction("input.stackdepth", new Depth(_inputStack));
    // What's the difference between generators and instructions?
  }

  // XXX What are frames?
  // YYY We're removing frames for now.
  /// <summary>
  /// Enables experimental Push "frames"
  /// When frames are enabled, each Push subtree is given a fresh set of stacks
  /// (a "frame") when it executes.
  /// </summary>
  /// <remarks>
  /// Enables experimental Push "frames"
  /// When frames are enabled, each Push subtree is given a fresh set of stacks
  /// (a "frame") when it executes. When a frame is pushed, the top value from
  /// each stack is passed to the new frame, and likewise when the frame pops,
  /// allowing for input arguments and return values.
  /// </remarks>
  // public void SetUseFrames(bool inUseFrames) {
  //   _useFrames = inUseFrames;
  // }

  /// <summary>
  /// Defines the instruction set used for random code generation in this Push
  /// interpreter.
  /// </summary>
  /// <param name="inInstructionList">
  /// A program consisting of a list of string instruction names to
  /// be placed in the instruction set.
  /// </param>
  /// <exception cref="Sharpen.Exception"/>

  /*
    Define a new instruction and add it to the random generators.
   */
  // public void AddInstruction(string inName, Instruction inInstruction) {
  //   // inName = InstructionCase(inName);
  //   _instructions[inName] = inInstruction;
  //   _generators[inName] = new Interpreter.InstructionAtomGenerator(inName);
  //   // randCode._randomGenerators[inName] = (iag);
  // }

  public void DefineConstant<T>(string inName, T value) {
    // DefineInstruction(inName, new Constant<T>(value));
    DefineInstruction(inName, () => value);
  }

  public void DefineInstruction(string inName, Action f) {
    DefineInstruction(inName, new NullaryAction(f));
  }

  public void DefineInstruction<T>(string inName, Func<T> f) {
    DefineInstruction(inName, new NullaryInstruction<T>(f));
  }

  public void DefineInstruction<T>(string inName, Action<T> f) {
    DefineInstruction(inName, new UnaryAction<T>(f));
  }

  public void DefineInstruction<X,Y>(string inName, Func<X,Y> f) {
    DefineInstruction(inName, new UnaryInstruction<X,Y>(f));
  }

  public void DefineInstruction<X,Y,Z>(string inName, Func<X,Y,Z> f) {
    DefineInstruction(inName, new BinaryInstruction<X,Y,Z>(f));
  }

  public void DefineInstruction<X,Y>(string inName, Action<X,Y> f) {
    DefineInstruction(inName, new BinaryAction<X,Y>(f));
  }

  public void DefineInstruction<X,Y,Z>(string inName, Action<X,Y,Z> f) {
    DefineInstruction(inName, new TrinaryAction<X,Y,Z>(f));
  }

  public void DefineInstruction<X,Y,Z,W>(string inName, Func<X,Y,Z,W> f) {
    DefineInstruction(inName, new TrinaryInsruction<X,Y,Z,W>(f));
  }

  // These don't make sense.
  // public void DefinePeekInstruction(string inName, Action f) {
  //   DefineInstruction(inName, new NullaryAction(f) { peek = true });
  // }

  // public void DefinePeekInstruction<T>(string inName, Func<T> f) {
  //   DefineInstruction(inName, new NullaryInstruction<T>(f) { peek = true });
  // }

  public void DefinePeekInstruction<T>(string inName, Action<T> f) {
    DefineInstruction(inName, new UnaryAction<T>(f) { peek = true });
  }

  public void DefinePeekInstruction<X,Y>(string inName, Func<X,Y> f) {
    DefineInstruction(inName, new UnaryInstruction<X,Y>(f) { peek = true });
  }

  public void DefinePeekInstruction<X,Y,Z>(string inName, Func<X,Y,Z> f) {
    DefineInstruction(inName, new BinaryInstruction<X,Y,Z>(f) { peek = true });
  }

  public void DefinePeekInstruction<X,Y>(string inName, Action<X,Y> f) {
    DefineInstruction(inName, new BinaryAction<X,Y>(f) { peek = true });
  }

  public void DefinePeekInstruction<X,Y,Z>(string inName, Action<X,Y,Z> f) {
    DefineInstruction(inName, new TrinaryAction<X,Y,Z>(f) { peek = true });
  }

  public void DefinePeekInstruction<X,Y,Z,W>(string inName, Func<X,Y,Z,W> f) {
    DefineInstruction(inName, new TrinaryInsruction<X,Y,Z,W>(f) { peek = true });
  }

  public void DefineInstruction(string inName, Instruction inInstruction) {
    // inName = InstructionCase(inName);
    _instructions[inName] = inInstruction;
    _generators[inName] = new Interpreter.InstructionAtomGenerator(inName);
  }

  protected internal void DefineStackInstructions(string inTypeName, Stack inStack) {
    DefineInstruction(inTypeName + ".pop",        new Pop(inStack));
    DefineInstruction(inTypeName + ".swap",       new Swap(inStack));
    DefineInstruction(inTypeName + ".rot",        new Rot(inStack));
    DefineInstruction(inTypeName + ".flush",      new Flush(inStack));
    DefineInstruction(inTypeName + ".dup",        new Dup(inStack));
    DefineInstruction(inTypeName + ".stackdepth", new Depth(inStack));
    DefineInstruction(inTypeName + ".shove",      new Shove(inStack));
    DefineInstruction(inTypeName + ".yank",       new Yank(inStack));
    DefineInstruction(inTypeName + ".yankdup",    new YankDup(inStack));
    DefineInstruction(inTypeName + ".any",        new Any(inStack));
  }

  /// <summary>Adds instructions for each enum member.</summary>
  public void DefineEnumConstants(string prefix, Type enumType) {
    foreach(var name in Enum.GetNames(enumType))
      DefineConstant<int>(prefix + "." + name.ToLower(), (int) Enum.Parse(enumType, name));
  }

  public void DefineEnumConstants(Type enumType) {
    DefineEnumConstants(enumType.Name.ToLower(), enumType);
  }

  /// <summary>Executes a Push program with no execution limit.</summary>
  /// <returns>The number of instructions executed.</returns>
  public int Execute(Program inProgram) {
    return Execute(inProgram, -1);
  }

  /// <summary>Executes a Push program with a given instruction limit.</summary>
  /// <param name="inMaxSteps">The maximum number of instructions allowed to be executed.</param>
  /// <returns>The number of instructions executed.</returns>
  public int Execute(Program inProgram, int inMaxSteps) {
    _evaluationExecutions++;
    LoadProgram(inProgram);
    // Initializes program
    return Step(inMaxSteps);
  }

  /// <summary>Loads a Push program into the interpreter's exec and code stacks.</summary>
  /// <param name="inProgram">The program to load.</param>
  public void LoadProgram(Program inProgram) {
    _codeStack.Push(inProgram);
    _execStack.Push(inProgram);
  }

  /// <summary>Steps a Push interpreter forward with a given instruction limit.</summary>
  /// <remarks>
  /// Steps a Push interpreter forward with a given instruction limit.
  /// This method assumes that the intepreter is already setup with an active
  /// program (typically using \ref Execute).
  /// </remarks>
  /// <param name="inMaxSteps">The maximum number of instructions allowed to be executed.</param>
  /// <returns>The number of instructions executed.</returns>
  public int Step(int inMaxSteps) {
    stop = false;
    int executed = 0;
    while (inMaxSteps != 0 && _execStack.Size() > 0 && ! stop) {
      var inst = _execStack.Pop();
      switch (ExecuteInstruction(inst)) {
      case 0:
        break;
      case -1:
        throw new Exception("Unable to execute instruction " + inst);
      }
      inMaxSteps--;
      executed++;
    }
    _totalStepsTaken += executed;
    return executed;
  }

  /* XXX This is how things could be. */
  // public virtual int ExecuteInstruction(OneOf<ProgramTree, ProgramAtom> o) {
  //   o.Switch(
  //     tree => {
  //       foreach(var child in tree.children.Reverse()) {
  //         if (child.isLeaf)
  //           _execStack.Push(child.Value);
  //         else
  //           _execStack.Push(child);
  //       }
  //       _execStack.Push(tree.Value);
  //     },
  //     atom => {
  //       atom.Switch(
  //                   i => _intStack.Push(i),
  //                   f => _floatStack.Push(f),
  //                   b => _boolStack.Push(b),
  //                   s => {
  //                     Instruction i;
  //                     if (_instructions.TryGetValue(InstructionCase(s), out i)) {
  //                       i.Execute(this);
  //                     } else {
  //                       _nameStack.Push(s);
  //                     }
  //                     return 0;
  //                   },
  //                   instr => instr.Execute(this));
  //     });
  // }

  public virtual int ExecuteInstruction(object inObject) {
    switch (Type.GetTypeCode(inObject.GetType())) {
      case TypeCode.Int16:
      case TypeCode.Int32:
      case TypeCode.Int64:
        _intStack.Push((int)inObject);
        return 0;
      case TypeCode.Single:
      case TypeCode.Double:
        _floatStack.Push((float)inObject);
        return 0;
      case TypeCode.Boolean:
        _boolStack.Push((bool)inObject);
        return 0;
      case TypeCode.String:
        Instruction i;
        // if (_instructions.TryGetValue(InstructionCase((string)inObject), out i)) {
        if (! quoting && _instructions.TryGetValue((string)inObject, out i)) {
          i.Execute(this);
        } else {
          _nameStack.Push((string)inObject);
          quoting = false;
        }
        return 0;
      case TypeCode.Object:
        if (inObject is Program) {
          Program p = (Program)inObject;
          p.PushAllReverse(_execStack);
          return 0;
        } else if (inObject is Instruction) {
          ((Instruction)inObject).Execute(this);
          return 0;
        } else {
          throw new Exception("No idea how to execute instruction " + inObject);
        }
      default:
        throw new Exception("No idea how to execute instruction " + inObject);
        // return -1;
    }
  }

  public GenericStack<T> GetStack<T>() {
    return _stacks
      .Select(x => x.Item2)
      .Where(y => y is GenericStack<T>)
      .Cast<GenericStack<T>>()
      .Where(s => s.stackType == typeof(T))
      .Single();
  }

  /// <summary>Fetch the active integer stack.</summary>
  public Psh.IntStack IntStack() {
    return _intStack;
  }

  /// <summary>Fetch the active float stack.</summary>
  public Psh.FloatStack FloatStack() {
    return _floatStack;
  }

  /// <summary>Fetch the active exec stack.</summary>
  public ObjectStack ExecStack() {
    return _execStack;
  }

  /// <summary>Fetch the active code stack.</summary>
  public ObjectStack CodeStack() {
    return _codeStack;
  }

  /// <summary>Fetch the active bool stack.</summary>
  public BooleanStack BoolStack() {
    return _boolStack;
  }

  /// <summary>Fetch the active name stack.</summary>
  public GenericStack<string> NameStack() {
    return _nameStack;
  }

  /// <summary>Fetch the active input stack.</summary>
  public ObjectStack InputStack() {
    return _inputStack;
  }

  /// <summary>Fetch the indexed custom stack</summary>
  public Stack GetStack(int inIndex) {
    return _stacks[inIndex].Item2;
  }

  public Stack GetStack(string stackName) {
    return _stacks.Where(x => x.Item1 == stackName).Single().Item2;
  }

  /// <summary>Add a custom stack, and return that stack's index</summary>
  public int AddStack(string stackName, Stack inStack) {
    _stacks.Add(Tuple.Create(stackName, inStack));
    DefineStackInstructions(stackName, inStack);
    return _stacks.Count - 1;
  }

  /// <summary>Prints out the current stack states.</summary>
  public void PrintStacks() {
    Console.Out.WriteLine(this);
  }

  /// <summary>Returns a string containing the current Interpreter stack states.</summary>
  public override string ToString() {
    var sb = new StringBuilder();
    foreach (var t in _stacks) {
      sb.Append(t.Item1);
      sb.Append(" stack: ");
      sb.Append(t.Item2);
      sb.Append("\n");
    }
    return sb.ToString();
  }

  /// <summary>Resets the Push interpreter state by clearing all of the stacks.</summary>
  public void ClearStacks() {
    // Clear all custom stacks
    foreach (var t in _stacks) {
      t.Item2.Clear();
    }
  }

  /// <summary>Returns a string list of all instructions enabled in the interpreter.</summary>
  public string GetRegisteredInstructionsString() {
    return _instructions
           .OrderBy(kv => kv.Key)
           .Select(kv => kv.Key.ToString())
           .Aggregate((current, next) => current + " " + next);
    // object[] keys = SharpenMinimal.Collections.ToArray(_instructions.Keys);
    // Arrays.Sort(keys);
    // string list = string.Empty;
    // for (int n = 0; n < keys.Length; n++)
    // {
    //   list += keys[n] + " ";
    // }
    // return list;
  }

  // public string InstructionCase(string instructionName) {
  //   return caseSensitive ? instructionName : instructionName.ToLower();
  // }


  /// <summary>Returns the Instruction whose name is given in instr.</summary>
  /// <param name="instr"/>
  /// <returns>the Instruction or null if no such Instruction.</returns>
  public Instruction GetInstruction(string instr) {
    return _instructions[instr];
  }

  /// <summary>Returns the number of evaluation executions so far this run.</summary>
  /// <returns>The number of evaluation executions during this run.</returns>
  public long GetEvaluationExecutions() {
    return _evaluationExecutions;
  }

  public InputPusher GetInputPusher() {
    return _inputPusher;
  }

  public void SetInputPusher(InputPusher _inputPusher) {
    this._inputPusher = _inputPusher;
  }

  private class InstructionAtomGenerator : AtomGenerator {
    internal string _instruction;

    internal InstructionAtomGenerator(string inInstructionName) {
      this._instruction = inInstructionName;
    }

    public object Generate() {
      return this._instruction;
    }
  }

  public abstract class RandAtomGenerator<T> : AtomGenerator, AtomGenerator<T> {
    protected Random Rng = new Random();

    public T min;
    public T max;
    public T resolution;

    public abstract T GenerateT();// { return default(T); }

    public object Generate() {
      // return null;
      return GenerateT();
      // return (object) AtomGenerator<T>.Generate();
    }
  }

  public class FloatAtomGenerator : Interpreter.RandAtomGenerator<float> {
    public FloatAtomGenerator() {
      min = 0f;
      max = 1f;
      resolution = -1f;
    }
    public override float GenerateT() {
      float r = ((float) Rng.NextDouble()) * (max - min);
      if (resolution > 0f) 
        r -= (r % resolution);
      return r + min;
    }
  }

  public class BoolAtomGenerator : AtomGenerator<bool>, AtomGenerator {
    Random Rng = new Random();
    public bool GenerateT() {
      return (Rng.Next(2) == 1);
    }
    public object Generate() {
      return (object) GenerateT();
    }
  }

  public class IntAtomGenerator : Interpreter.RandAtomGenerator<int> {
    public IntAtomGenerator() {
      min = 0;
      max = 100;
      resolution = -1;
    }
    public override int GenerateT() {
      int r = Rng.Next(max - min);
      if (resolution > 0)
        r -= (r % resolution);
      return r + min;
    }
  }

  /*
    Make the interpreter yield. The interpreter stops even if there are more
    instructions left.
   */
  public void Yield() {
    stop = true;
  }
}

public interface AtomGenerator {
  object Generate();
}

// XXX OMG, I tried to make a new T Generate(), but it's just not worth it.
public interface AtomGenerator<out T> : AtomGenerator {
  T GenerateT();
}

}
