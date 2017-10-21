// using System;
// using Psh;
// using Sharpen;

// [System.Serializable]
// public class PshApplet : Java.Applet.Applet
// {

//   internal Interpreter _interpreter = new Interpreter();

//   /*
//   * Copyright 2009-2010 Jon Klein
//   *
//   * Licensed under the Apache License, Version 2.0 (the "License");
//   * you may not use this file except in compliance with the License.
//   * You may obtain a copy of the License at
//   *
//   *    http://www.apache.org/licenses/LICENSE-2.0
//   *
//   * Unless required by applicable law or agreed to in writing, software
//   * distributed under the License is distributed on an "AS IS" BASIS,
//   * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   * See the License for the specific language governing permissions and
//   * limitations under the License.
//   */
//   public override void Init()
//   {
//     try
//     {
//       System.Console.Out.Println(Run(GetParameter("program")));
//     }
//     catch (Exception)
//     {
//     }
//   }

//   public virtual string Run(string inValue)
//   {
//     _interpreter.ClearStacks();
//     try
//     {
//       Program p;
//       p = new Program(_interpreter, inValue);
//       _interpreter.Execute(p);
//     }
//     catch (Exception)
//     {
//     }
//     return _interpreter.ToString();
//   }

//   public virtual string GetInstructionString()
//   {
//     return "=> " + _interpreter.GetRegisteredInstructionsString();
//   }
// }
