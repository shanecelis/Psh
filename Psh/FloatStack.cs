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
using Sharpen;

namespace Psh
{
  /// <summary>The Push stack type for object-based data (Strings, Programs, etc.)</summary>
  [System.Serializable]
  public class FloatStack : Stack
  {
    private const long serialVersionUID = 1L;

    protected internal float[] _stack;

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }
      if (GetType() != obj.GetType())
      {
        return false;
      }
      FloatStack other = (FloatStack)obj;
      if (_size != other._size)
      {
        return false;
      }
      for (int i = 0; i < _size; i++)
      {
        if (_stack[i] != other._stack[i])
        {
          return false;
        }
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hash = 7;
      for (int i = 0; i < _size; i++)
      {
        hash = 41 * hash + float.ValueOf(_stack[i]).GetHashCode();
      }
      return hash;
    }

    internal override void Resize(int inSize)
    {
      float[] newstack = new float[inSize];
      if (_stack != null)
      {
        System.Array.Copy(_stack, 0, newstack, 0, _size);
      }
      _stack = newstack;
      _maxsize = inSize;
    }

    public virtual float Accumulate()
    {
      float f = 0;
      for (int n = 0; n < _size; n++)
      {
        f += _stack[n];
      }
      return f;
    }

    public virtual float Top()
    {
      return Peek(_size - 1);
    }

    public virtual float Peek(int inIndex)
    {
      if (inIndex >= 0 && inIndex < _size)
      {
        return _stack[inIndex];
      }
      return 0.0f;
    }

    public virtual float Pop()
    {
      float result = 0.0f;
      if (_size > 0)
      {
        result = _stack[_size - 1];
        _size--;
      }
      return result;
    }

    public virtual void Push(float inValue)
    {
      _stack[_size] = inValue;
      _size++;
      if (_size >= _maxsize)
      {
        Resize(_maxsize * 2);
      }
    }

    internal override void Dup()
    {
      if (_size > 0)
      {
        Push(_stack[_size - 1]);
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
        float toShove = Top();
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
        float tmp = _stack[_size - 1];
        _stack[_size - 1] = _stack[_size - 2];
        _stack[_size - 2] = tmp;
      }
    }

    internal override void Rot()
    {
      if (_size > 2)
      {
        float tmp = _stack[_size - 3];
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
        float toYank = Peek(yankedIndex);
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

    public virtual void Set(int inIndex, float inValue)
    {
      if (inIndex >= 0 && inIndex < _size)
      {
        _stack[inIndex] = inValue;
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
