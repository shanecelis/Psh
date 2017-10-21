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
using System.Collections.Generic;
using Psh;

namespace Psh.Coevolution
{
  /// <summary>
  /// An abstract CEPredictorGA individual class for developing co-evolved
  /// predictors.
  /// </summary>
  public abstract class PredictionGAIndividual : GAIndividual
  {
    /// <summary>Predicts the fitness of the input PushGPIndividual</summary>
    /// <param name="individual">to predict the fitness of</param>
    /// <returns>predicted fitness</returns>
    public abstract float PredictSolutionFitness(PushGPIndividual pgpIndividual);

    /// <summary>Computes the absolute-average-of-errors fitness from an error vector.</summary>
    /// <returns>the average error value for the vector.</returns>
    protected internal virtual float AbsoluteAverageOfErrors(List<float> inArray)
    {
      float total = 0.0f;
      for (int n = 0; n < inArray.Count; n++)
      {
        total += Math.Abs(inArray[n]);
      }
      if (float.IsInfinity(total))
      {
        return float.MaxValue;
      }
      return (total / inArray.Count);
    }
  }
}
