using System;
using System;
using Psh;
using Sharpen;

namespace Psh.Coevolution
{
  [System.Serializable]
  public class FloatRegFitPrediction : PredictionGA
  {
    private const long serialVersionUID = 1L;

    protected internal override void InitIndividual(GAIndividual inIndividual)
    {
      FloatRegFitPredictionIndividual i = (FloatRegFitPredictionIndividual)inIndividual;
      int[] samples = new int[FloatRegFitPredictionIndividual._sampleSize];
      for (int j = 0; j < samples.Length; j++)
      {
        samples[j] = Rng.Next(_solutionGA._testCases.Count);
      }
      i.SetSampleIndicesAndSolutionGA(_solutionGA, samples);
    }

    protected internal override void EvaluateIndividual(GAIndividual inIndividual)
    {
      FloatRegFitPredictionIndividual predictor = (FloatRegFitPredictionIndividual)inIndividual;
      AList<float> errors = new AList<float>();
      for (int i = 0; i < _trainerPopulationSize; i++)
      {
        float predictedError = predictor.PredictSolutionFitness(_trainerPopulation[i]);
        // Error is difference between predictedError and the actual fitness
        // of the trainer.
        float error = Math.Abs(predictedError) - Math.Abs(_trainerPopulation[i].GetFitness());
        errors.Add(error);
      }
      predictor.SetFitness(AbsoluteAverageOfErrors(errors));
      predictor.SetErrors(errors);
    }

    /// <summary>
    /// Determines the predictor's fitness on a trainer, where the trainer is the
    /// inInput, and the trainer's actual fitness is inOutput.
    /// </summary>
    /// <remarks>
    /// Determines the predictor's fitness on a trainer, where the trainer is the
    /// inInput, and the trainer's actual fitness is inOutput. The fitness of
    /// the predictor is the absolute error between the prediction and the
    /// trainer's actual fitness.
    /// </remarks>
    /// <returns>Predictor's fitness (i.e. error) for the given trainer.</returns>
    /// <exception cref="System.Exception"></exception>
    public override float EvaluateTestCase(GAIndividual inIndividual, object inInput, object inOutput)
    {
      PushGPIndividual trainer = (PushGPIndividual)inInput;
      float trainerFitness = (float)inOutput;
      float predictedTrainerFitness = ((PredictionGAIndividual)inIndividual).PredictSolutionFitness(trainer);
      return Math.Abs(predictedTrainerFitness - trainerFitness);
    }

    protected internal override void EvaluateTrainerFitnesses()
    {
      foreach (PushGPIndividual trainer in _trainerPopulation)
      {
        if (!trainer.FitnessIsSet())
        {
          EvaluateSolutionIndividual(trainer);
        }
      }
    }

    protected internal override void Reproduce()
    {
      int nextPopulation = _currentPopulation == 0 ? 1 : 0;
      for (int n = 0; n < _populations[_currentPopulation].Length; n++)
      {
        float method = Rng.Next(100);
        GAIndividual next;
        if (method < _mutationPercent)
        {
          next = ReproduceByMutation(n);
        }
        else
        {
          if (method < _crossoverPercent + _mutationPercent)
          {
            next = ReproduceByCrossover(n);
          }
          else
          {
            next = ReproduceByClone(n);
          }
        }
        // Make sure next isn't already in the population, so that all
        // predictors are unique.
        for (int k = 0; k < n; k++)
        {
          if (((FloatRegFitPredictionIndividual)next).EqualPredictors(_populations[nextPopulation][k]))
          {
            int index = Rng.Next(FloatRegFitPredictionIndividual._sampleSize);
            ((FloatRegFitPredictionIndividual)next).SetSampleIndex(index, Rng.Next(_solutionGA._testCases.Count));
          }
        }
        _populations[nextPopulation][n] = next;
      }
    }

    /// <summary>
    /// Mutates an individual by choosing an index at random and randomizing
    /// its training point among possible individuals.
    /// </summary>
    protected internal override GAIndividual ReproduceByMutation(int inIndex)
    {
      FloatRegFitPredictionIndividual i = (FloatRegFitPredictionIndividual)ReproduceByClone(inIndex);
      int index = Rng.Next(FloatRegFitPredictionIndividual._sampleSize);
      i.SetSampleIndex(index, Rng.Next(_solutionGA._testCases.Count));
      return i;
    }

    protected internal override GAIndividual ReproduceByCrossover(int inIndex)
    {
      FloatRegFitPredictionIndividual a = (FloatRegFitPredictionIndividual)ReproduceByClone(inIndex);
      FloatRegFitPredictionIndividual b = (FloatRegFitPredictionIndividual)TournamentSelect(_tournamentSize, inIndex);
      // crossoverPoint is the first index of a that will be changed to the
      // gene from b.
      int crossoverPoint = Rng.Next(FloatRegFitPredictionIndividual._sampleSize - 1) + 1;
      for (int i = crossoverPoint; i < FloatRegFitPredictionIndividual._sampleSize; i++)
      {
        a.SetSampleIndex(i, b.GetSampleIndex(i));
      }
      return a;
    }
  }
}
