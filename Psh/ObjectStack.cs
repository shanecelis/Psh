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
public class ObjectStack : GenericStack<object> {

  public override void Push(object inValue)
  {
    // XXX I do not get what this is supposed to be doing here:
    // Maybe it's supposed to make a program copy.
    if (inValue is Program)
    {
      inValue = (object)new Program((Program)inValue);
    }
    base.Push(inValue);
  }
}
}
