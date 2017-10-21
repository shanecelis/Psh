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
  /// <summary>Abstract class for implementing stacks.</summary>
  [System.Serializable]
public abstract class Stack
  {

    protected internal int _size;

    protected internal int _maxsize;

    internal Stack()
    {
      _size = 0;
      Resize(8);
    }

    internal abstract void Resize(int inSize);

    internal abstract void Dup();

    internal abstract void Rot();

    internal abstract void Shove(int inIndex);

    internal abstract void Swap();

    internal abstract void Yank(int inIndex);

    internal abstract void Yankdup(int inIndex);

    public virtual void Clear()
    {
      _size = 0;
    }

    public virtual int Size()
    {
      return _size;
    }

    public virtual void Popdiscard()
    {
      if (_size > 0)
      {
        _size--;
      }
    }
  }
}
