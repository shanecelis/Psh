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
using Sharpen;

namespace Psh
{
  /// <summary>The Push stack type for generic data (Strings, Programs, etc.)</summary>
  [System.Serializable]
  public class GenericStack<T> : Stack
  {
    private const long serialVersionUID = 1L;

    protected internal T[] _stack;

    internal const int _blocksize = 16;

    public virtual void PushAllReverse(GenericStack<T> inOther)
    {
      for (int n = _size - 1; n >= 0; n--)
      {
        inOther.Push(_stack[n]);
      }
    }

    public override bool Equals(object inOther)
    {
      if (this == inOther)
      {
        return true;
      }
      // Sadly, because generics are implemented using type erasure,
      // a GenericStack<A> will be the same class as a GenericStack<B>,
      // this being GenericStack. So the best we can do here is be assured
      // that inOther is at least a GenericStack.
      //
      // This just means that every empty stack is the same as every other
      // empty stack.
      if (inOther.GetType() != GetType())
      {
        return false;
      }
      return ((GenericStack<T>)inOther).Comparestack(_stack, _size);
    }

    public override int GetHashCode()
    {
      int hash = GetType().GetHashCode();
      hash = 37 * hash + /*Arrays.*/DeepHashCode(this._stack);
      return hash;
    }

    private int DeepHashCode(T[] array) {
      int hash = 0;
      for (int i = 0; i < array.Length; i++)
        hash ^= array[i].GetHashCode();
      return hash;
    }

    internal virtual bool Comparestack(T[] inOther, int inOtherSize)
    {
      if (inOtherSize != _size)
      {
        return false;
      }
      for (int n = 0; n < _size; n++)
      {
        if (!_stack[n].Equals(inOther[n]))
        {
          return false;
        }
      }
      return true;
    }

    internal override void Resize(int inSize)
    {
      T[] newstack = new T[inSize];
      if (_stack != null)
      {
        System.Array.Copy(_stack, 0, newstack, 0, _size);
      }
      _stack = newstack;
      _maxsize = inSize;
    }

    public virtual T Peek(int inIndex)
    {
      if (inIndex >= 0 && inIndex < _size)
      {
        return _stack[inIndex];
      }
      return default(T);
    }

    public virtual T Top()
    {
      return Peek(_size - 1);
    }

    public virtual T Pop()
    {
      T result = default(T);
      if (_size > 0)
      {
        result = _stack[_size - 1];
        _size--;
      }
      return result;
    }

    public virtual void Push(T inValue)
    {
      // XXX I do not get what this is supposed to be doing here:
      // if (inValue is Program)
      // {
      //   inValue = (T)new Program((Program)inValue);
      // }
      _stack[_size] = inValue;
      _size++;
      if (_size >= _maxsize)
      {
        Resize(_maxsize + _blocksize);
      }
    }

    internal override void Dup()
    {
      if (_size > 0)
      {
        Push(_stack[_size - 1]);
      }
    }

    public virtual void Shove(T obj, int n)
    {
      if (n > _size)
      {
        n = _size;
      }
      // n = 0 is the same as push, so
      // the position in the array we insert at is
      // _size-n.
      n = _size - n;
      for (int i = _size; i > n; i--)
      {
        _stack[i] = _stack[i - 1];
      }
      _stack[n] = obj;
      _size++;
      if (_size >= _maxsize)
      {
        Resize(_maxsize + _blocksize);
      }
    }

    internal override void Shove(int inIndex)
    {
      if (_size > 0)
      {
        if (inIndex < 0)
        {
          inIndex = 0;
        }
        if (inIndex > _size - 1)
        {
          inIndex = _size - 1;
        }
        T toShove = Top();
        int shovedIndex = _size - inIndex - 1;
        for (int i = _size - 1; i > shovedIndex; i--)
        {
          _stack[i] = _stack[i - 1];
        }
        _stack[shovedIndex] = toShove;
      }
    }

    internal override void Swap()
    {
      if (_size > 1)
      {
        T tmp = _stack[_size - 2];
        _stack[_size - 2] = _stack[_size - 1];
        _stack[_size - 1] = tmp;
      }
    }

    internal override void Rot()
    {
      if (_size > 2)
      {
        T tmp = _stack[_size - 3];
        _stack[_size - 3] = _stack[_size - 2];
        _stack[_size - 2] = _stack[_size - 1];
        _stack[_size - 1] = tmp;
      }
    }

    internal override void Yank(int inIndex)
    {
      if (_size > 0)
      {
        if (inIndex < 0)
        {
          inIndex = 0;
        }
        if (inIndex > _size - 1)
        {
          inIndex = _size - 1;
        }
        int yankedIndex = _size - inIndex - 1;
        T toYank = Peek(yankedIndex);
        for (int i = yankedIndex; i < _size - 1; i++)
        {
          _stack[i] = _stack[i + 1];
        }
        _stack[_size - 1] = toYank;
      }
    }

    internal override void Yankdup(int inIndex)
    {
      if (_size > 0)
      {
        if (inIndex < 0)
        {
          inIndex = 0;
        }
        if (inIndex > _size - 1)
        {
          inIndex = _size - 1;
        }
        int yankedIndex = _size - inIndex - 1;
        Push(Peek(yankedIndex));
      }
    }

    public override string ToString()
    {
      string result = "[";
      for (int n = _size - 1; n >= 0; n--)
      {
        if (n == _size - 1)
        {
          result += _stack[n];
        }
        else
        {
          result += " " + _stack[n];
        }
      }
      result += "]";
      return result;
    }
  }
}
