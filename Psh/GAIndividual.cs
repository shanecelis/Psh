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
  /// <summary>An abstract GP individual class containing a fitness value.</summary>
  /// <remarks>
  /// An abstract GP individual class containing a fitness value. The fitness value
  /// represents an individual's <i>error</i> values, such that <i>lower</i>
  /// fitness values are better and such that a fitness value of 0 indicates a
  /// perfect solution.
  /// </remarks>
  public abstract class GAIndividual
  {

    internal float _fitness;

    internal AList<float> _errors;

    internal bool _fitnessSet;

    public virtual bool FitnessIsSet()
    {
      return _fitnessSet;
    }

    public virtual float GetFitness()
    {
      return _fitness;
    }

    public virtual void SetFitness(float inFitness)
    {
      _fitness = inFitness;
      _fitnessSet = true;
    }

    public virtual AList<float> GetErrors()
    {
      return _errors;
    }

    public virtual void SetErrors(AList<float> inErrors)
    {
      _errors = inErrors;
    }

    public abstract GAIndividual Clone();
  }
}
