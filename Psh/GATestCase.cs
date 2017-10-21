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
  /// <summary>An abstract container for a GATestCase containing an input and output object.</summary>
  [System.Serializable]
  public class GATestCase
  {

    public GATestCase(object inInput, object inOutput)
    {
      _input = inInput;
      _output = inOutput;
    }

    public object _input;

    public object _output;
  }
}
