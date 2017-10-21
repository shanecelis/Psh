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
  /// <summary>
  /// A PushGA individual class which is a simple wrapper around a Push Program
  /// object.
  /// </summary>
  public class PushGPIndividual : GAIndividual
  {

    public Program _program;

    public PushGPIndividual()
    {
    }

    internal PushGPIndividual(Program inProgram)
    {
      SetProgram(inProgram);
      _fitnessSet = false;
    }

    internal virtual void SetProgram(Program inProgram)
    {
      if (inProgram != null)
      {
        _program = new Program(inProgram);
      }
    }

    public override string ToString()
    {
      return _program.ToString();
    }

    public override GAIndividual Clone()
    {
      return new Psh.PushGPIndividual(_program);
    }
  }
}
