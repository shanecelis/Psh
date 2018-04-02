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

namespace Psh {

public class IncludeException : Exception {
  internal IncludeException(string inStr) : base(inStr) { }
  internal IncludeException(string inStr, Exception e) : base(inStr, e) { }
}

/**
   Thrown by FuncInstruction and ActionInstruction to represent that no value
   should be returned.
 */
public class NoReturnValueException : Exception {
  internal NoReturnValueException(string message) : base(message) { }
  internal NoReturnValueException(string message, Exception e) : base(message, e) { }
}

}
