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
using Sharpen;

namespace Psh
{
  /// <summary>A Push program.</summary>
  [System.Serializable]
  public class Program : ObjectStack
  {
    private const long serialVersionUID = 1L;

    internal Interpreter _interpreter = null;

    public virtual Interpreter GetInterpreter()
    {
      return _interpreter;
    }

    public virtual void SetInterpreter(Interpreter _interpreter)
    {
      this._interpreter = _interpreter;
    }

    /// <summary>Constructs an empty Program.</summary>
    [System.ObsoleteAttribute]
    public Program()
    {
    }

    /// <summary>Constructs an empty Program with an associated Interpreter.</summary>
    public Program(Interpreter inInterpreter)
    {
      _interpreter = inInterpreter;
    }

    /// <summary>Constructs a copy of an existing Program.</summary>
    /// <param name="inOther">The Push program to copy.</param>
    public Program(Psh.Program inOther)
    {
      inOther.CopyTo(this);
      _interpreter = inOther._interpreter;
    }

    /// <summary>Constructs a Push program by parsing a String.</summary>
    /// <param name="inString">The Push program string to parse.</param>
    /// <exception cref="System.Exception"/>
    public Program(string inString)
    {
      Parse(inString);
    }

    /// <summary>Constructs a Push program by parsing a String.</summary>
    /// <param name="inString">The Push program string to parse.</param>
    /// <exception cref="System.Exception"/>
    public Program(Interpreter _interpreter, string inString)
    {
      this._interpreter = _interpreter;
      Parse(inString);
    }

    /// <summary>Sets this program to the parsed program string.</summary>
    /// <param name="inString">The Push program string to parse.</param>
    /// <returns>The point size of the new program.</returns>
    /// <exception cref="System.Exception"/>
    public virtual int Parse(string inString)
    {
      Clear();
      inString = inString.Replace("(", " ( ");
      inString = inString.Replace(")", " ) ");
      string[] tokens = inString.Split("\\s+");
      Parse(tokens, 0);
      return Programsize();
    }

    /// <exception cref="System.Exception"/>
    private int Parse(string[] inTokens, int inStart)
    {
      bool first = (inStart == 0);
      for (int n = inStart; n < inTokens.Length; n++)
      {
        string token = inTokens[n];
        if (!token.Equals(string.Empty))
        {
          if (token.Equals("("))
          {
            // Found an open paren -- begin a recursive Parse, though
            // the very first
            // token in the list is a special case -- no need to create
            // a sub-program
            if (!first)
            {
              Psh.Program p = new Psh.Program(_interpreter);
              n = p.Parse(inTokens, n + 1);
              Push(p);
            }
          }
          else
          {
            if (token.Equals(")"))
            {
              // End of the program -- return the advanced token index to
              // the caller
              return n;
            }
            else
            {
              if (char.IsLetter(token[0]))
              {
                Push(token);
              }
              else
              {
                // This makes printing stacks very ugly. For now, will store
                // program instructions as strings, as was done before.
                /*
                Instruction i = _interpreter._instructions.get(token);
                if (i != null)
                push(i);
                else
                push(token);
                */
                object number;
                if (token.IndexOf('.') != -1)
                {
                  number = float.Parse(token);
                }
                else
                {
                  number = System.Convert.ToInt32(token);
                }
                Push(number);
              }
            }
          }
          first = false;
        }
      }
      // If we're here, there was no closing brace for one of the programs
      throw new Exception("no closing brace found for program");
    }

    /// <summary>Returns the size of the program and all subprograms.</summary>
    /// <returns>The size of the program.</returns>
    public virtual int Programsize()
    {
      int size = _size;
      for (int n = 0; n < _size; n++)
      {
        object o = _stack[n];
        if (o is Psh.Program)
        {
          size += ((Psh.Program)o).Programsize();
        }
      }
      return size;
    }

    /// <summary>Returns the size of a subtree.</summary>
    /// <param name="inIndex">The index of the requested subtree.</param>
    /// <returns>The size of the subtree.</returns>
    public virtual int SubtreeSize(int inIndex)
    {
      object sub = Subtree(inIndex);
      if (sub == null)
      {
        return 0;
      }
      if (sub is Psh.Program)
      {
        return ((Psh.Program)sub).Programsize();
      }
      return 1;
    }

    /// <summary>Returns a subtree of the program.</summary>
    /// <param name="inIndex">The index of the requested subtree.</param>
    /// <returns>The program subtree.</returns>
    public virtual object Subtree(int inIndex)
    {
      if (inIndex < _size)
      {
        return _stack[inIndex];
      }
      else
      {
        int startIndex = _size;
        for (int n = 0; n < _size; n++)
        {
          object o = _stack[n];
          if (o is Psh.Program)
          {
            Psh.Program sub = (Psh.Program)o;
            int length = sub.Programsize();
            if (inIndex - startIndex < length)
            {
              return sub.Subtree(inIndex - startIndex);
            }
            startIndex += length;
          }
        }
      }
      return null;
    }

    /// <summary>Replaces a subtree of this Program with a new object.</summary>
    /// <param name="inIndex">The index of the subtree to replace.</param>
    /// <param name="inReplacement">The replacement for the subtree</param>
    /// <returns>True if a replacement was made (the index was valid).</returns>
    public virtual bool ReplaceSubtree(int inIndex, object inReplacement)
    {
      if (inIndex < _size)
      {
        _stack[inIndex] = Cloneforprogram(inReplacement);
        return true;
      }
      else
      {
        int startIndex = _size;
        for (int n = 0; n < _size; n++)
        {
          object o = _stack[n];
          if (o is Psh.Program)
          {
            Psh.Program sub = (Psh.Program)o;
            int length = sub.Programsize();
            if (inIndex - startIndex < length)
            {
              return sub.ReplaceSubtree(inIndex - startIndex, inReplacement);
            }
            startIndex += length;
          }
        }
      }
      return false;
    }

    public virtual void Flatten(int inIndex)
    {
      if (inIndex < _size)
      {
        // If here, the index to be flattened is in this program. So, push
        // the rest of the program onto a new program, and replace this with
        // that new program.
        Psh.Program replacement = new Psh.Program(this);
        Clear();
        for (int i = 0; i < replacement._size; i++)
        {
          if (inIndex == i)
          {
            if (replacement._stack[i] is Psh.Program)
            {
              Psh.Program p = (Psh.Program)replacement._stack[i];
              for (int j = 0; j < p._size; j++)
              {
                this.Push(p._stack[j]);
              }
            }
            else
            {
              this.Push(replacement._stack[i]);
            }
          }
          else
          {
            this.Push(replacement._stack[i]);
          }
        }
      }
      else
      {
        int startIndex = _size;
        for (int n = 0; n < _size; n++)
        {
          object o = _stack[n];
          if (o is Psh.Program)
          {
            Psh.Program sub = (Psh.Program)o;
            int length = sub.Programsize();
            if (inIndex - startIndex < length)
            {
              sub.Flatten(inIndex - startIndex);
              break;
            }
            startIndex += length;
          }
        }
      }
    }

    /// <summary>Copies this program to another.</summary>
    /// <param name="inOther">The program to receive the copy of this program</param>
    public virtual void CopyTo(Psh.Program inOther)
    {
      for (int n = 0; n < _size; n++)
      {
        inOther.Push(_stack[n]);
      }
    }

    public override string ToString()
    {
      string result = "(";
      for (int n = 0; n < _size; n++)
      {
        if (result[result.Length - 1] == '(')
        {
          result += (string)_stack[n];
        }
        else
        {
          result += " " + _stack[n];
        }
      }
      result += ")";
      return result;
    }

    /// <summary>Creates a copy of an object suitable for adding to a Push Program.</summary>
    /// <remarks>
    /// Creates a copy of an object suitable for adding to a Push Program. Java's
    /// clone() is unfortunately useless for this task.
    /// </remarks>
    private object Cloneforprogram(object inObject)
    {
      // Java clone() is useless :(
      if (inObject is string)
      {
        return new string((string)inObject);
      }
      if (inObject is int)
      {
        return System.Convert.ToInt32((int)inObject);
      }
      if (inObject is float)
      {
        return System.Convert.ToSingle((float)inObject);
      }
      if (inObject is Psh.Program)
      {
        return new Psh.Program((Psh.Program)inObject);
      }
      if (inObject is Instruction)
      {
        return inObject;
      }
      // no need to copy; instructions are singletons
      return null;
    }
  }
}
