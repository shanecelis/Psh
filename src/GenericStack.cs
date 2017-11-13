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
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Psh {
/// <summary>The Push stack type for generic data (Strings, Programs, etc.)</summary>
public class GenericStack<T> : List<T>, Stack {
  // protected internal T[] _stack;

  // internal const int _blocksize = 16;

  public Type stackType {
    get { return typeof(T); }
  }

  public int _size {
    get { return Count; }
  }

  public virtual void PushAllReverse(GenericStack<T> inOther) {
    for (int n = Count - 1; n >= 0; n--) {
      inOther.Push(this[n]);
    }
  }

  public int Size() {
    return Count;
  }

  public override bool Equals(object inOther) {
    if (this == inOther) {
      return true;
    }
    // Sadly, because generics are implemented using type erasure,
    // a GenericStack<A> will be the same class as a GenericStack<B>,
    // this being GenericStack. So the best we can do here is be assured
    // that inOther is at least a GenericStack.
    //
    // This just means that every empty stack is the same as every other
    // empty stack.
    if (inOther.GetType() != GetType()) {
      return false;
    }
    return this.SequenceEqual((GenericStack<T>)inOther);
  }

  public override int GetHashCode() {
    int hash = GetType().GetHashCode();
    hash = 37 * hash + /*Arrays.*/DeepHashCode(this);
    return hash;
  }

  private int DeepHashCode(List<T> array) {
    int hash = 0;
    for (int i = 0; i < array.Count; i++)
      hash ^= array[i].GetHashCode();
    return hash;
  }

  // internal virtual bool Comparestack(T[] inOther, int inOtherSize) {
  //   if (inOtherSize != Count) {
  //     return false;
  //   }
  //   this.SequenceEqual(
  //   for (int n = 0; n < Count; n++) {
  //     if (!this[n].Equals(inOther[n])) {
  //       return false;
  //     }
  //   }
  //   return true;
  // }

  // internal void Resize(int inSize) {
  //   T[] newstack = new T[inSize];
  //   if (this != null) {
  //     System.Array.Copy(this, 0, newstack, 0, Count);
  //   }
  //   this = newstack;
  //   _maxsize = inSize;
  // }

  // XXX This seems like strange semantics for Peek().
  // I'd expect Peek() == Pop()
  public virtual T Peek(int n) {

    if (n < 0)
      n = 0;
    if (n >= Count) {
      n = Count - 1;
    }
    // n = 0 is the same as push, so
    // the position in the array we insert at is
    // Count-n.
    return this[Count - n - 1];
    // if (inIndex >= 0 && inIndex < Count) {
    //   return this[inIndex];
    // }
    // return default(T);
  }

  public virtual T DeepPeek(int n) {

    if (n < 0)
      n = 0;
    if (n >= Count) {
      n = Count - 1;
    }
    // n = 0 is the same as push, so
    // the position in the array we insert at is
    // Count-n.
    return this[n];
    // if (inIndex >= 0 && inIndex < Count) {
    //   return this[inIndex];
    // }
    // return default(T);
  }


  public virtual T Top() {
    return Count != 0 ? this.Last() : default(T);
    // return Peek(Count - 1);
  }

  public virtual T Pop() {
    T result = default(T);
    if (Count > 0) {
      result = this[Count - 1];
      RemoveAt(Count - 1);
    }
    return result;
  }

  public void Popdiscard() {
    Pop();
  }

  public virtual void Push(T inValue) {
    // XXX I do not get what this is supposed to be doing here:
    // Maybe it's supposed to make a program copy.
    // if (inValue is Program)
    // {
    //   inValue = (T)new Program((Program)inValue);
    // }
    // this[Count] = inValue;
    // Count++;
    // if (Count >= _maxsize) {
    //   Resize(_maxsize + _blocksize);
    // }
    // XXX one test actually relies on this behavior.  Moving to Program.
    // if (inValue == null)
    //   throw new ArgumentNullException("inValue");
    Add(inValue);
  }

  public void Dup() {
    if (Count > 0) {
      Push(this[Count - 1]);
    }
  }

  public int ReverseIndex(int reverseIndex) {
    if (reverseIndex < 0)
      reverseIndex = 0;
    if (reverseIndex >= Count) {
      reverseIndex = Count - 1;
    }
    return Count - 1 - reverseIndex;
  }

  // count = 2
  //   n = 1

  //   i = 2 - 1 == 1;
  // Shove(x, 0) = Push(x)
  // Shove(x, 1) = y = Pop(); Push(x); Push(y);
  public virtual void Shove(T obj, int n) {
    // n = 0 is the same as push, so
    // the position in the array we insert at is
    // Count-n.
    int i = ReverseIndex(n);
    try {
      if (Count == 0 || i == Count - 1)
        Add(obj);
      else
        Insert(i + 1, obj);
    } catch (Exception e) {
      throw new Exception("shove fail on count " + Count + " arg " + n + " index " + i, e);
    }
  }

  public void Shove(int inIndex) {
    Shove(Top(), inIndex);
    // if (Count > 0) {
    //   if (inIndex < 0) {
    //     inIndex = 0;
    //   }
    //   if (inIndex > Count - 1) {
    //     inIndex = Count - 1;
    //   }
    //   T toShove = Top();
    //   int shovedIndex = Count - inIndex - 1;
    //   for (int i = Count - 1; i > shovedIndex; i--) {
    //     this[i] = this[i - 1];
    //   }
    //   this[shovedIndex] = toShove;
    // }
  }

  public void Swap() {
    if (Count > 1) {
      T tmp = this[Count - 2];
      this[Count - 2] = this[Count - 1];
      this[Count - 1] = tmp;
    }
  }

  public void Rot() {
    if (Count > 2) {
      T tmp = this[Count - 3];
      this[Count - 3] = this[Count - 2];
      this[Count - 2] = this[Count - 1];
      this[Count - 1] = tmp;
    }
  }

  public void Yank(int inIndex) {
    if (Count > 0) {
      if (inIndex < 0) {
        inIndex = 0;
      }
      if (inIndex > Count - 1) {
        inIndex = Count - 1;
      }
      int yankedIndex = Count - inIndex - 1;
      T toYank = DeepPeek(yankedIndex);
      for (int i = yankedIndex; i < Count - 1; i++) {
        this[i] = this[i + 1];
      }
      this[Count - 1] = toYank;
    }
  }

  public void YankDup(int inIndex) {
    if (Count > 0) {
      if (inIndex < 0) {
        inIndex = 0;
      }
      if (inIndex > Count - 1) {
        inIndex = Count - 1;
      }
      int yankedIndex = Count - inIndex - 1;
      Push(DeepPeek(yankedIndex));
    }
  }

  public override string ToString() {
    var result = new StringBuilder("[");
    // Why does this happen backwards?
    for (int n = Count - 1; n >= 0; n--) {
    // for (int n = 0; n < Count; n++) {
      // if (n != 0) {
      if (n != Count - 1) {
        result.Append(" ");
      }
      result.Append(this[n].ToString());
    }
    result.Append("]");
    return result.ToString();
  }
}
}
