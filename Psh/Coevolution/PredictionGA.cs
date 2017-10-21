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
using Sharpen;

namespace Psh.Coevolution
{
  /// <summary>
  /// An abstract class for a population of co-evolving predictors of fitness,
  /// rank, or something similar.
  /// </summary>
  /// <remarks>
  /// An abstract class for a population of co-evolving predictors of fitness,
  /// rank, or something similar. In this class, "fitness" or "predicted fitness"
  /// of a solution individual may be referring to the actual fitness of the
  /// individual, or it may be referring to something similar, such as the
  /// individual's rank.
  /// </remarks>
  public abstract class PredictionGA : GA
  {

    protected internal AList<PushGPIndividual> _trainerPopulation;

    protected internal int _generationsBetweenTrainers;

    protected internal int _trainerPopulationSize;

    protected internal PushGP _solutionGA;

    // Note: Oldest trainer has the lowest index; newest trainer has the highest
    // index.
    // The solution population and genetic algorithm.
    /// <summary>
    /// Customizes GA.GAWithParameters to allow the inclusion of the solution GA,
    /// which is required for the initialization of the prediction GA.
    /// </summary>
    /// <param name="ceFloatSymbolicRegression"/>
    /// <param name="getPredictorParameters"/>
    /// <returns/>
    /// <exception cref="System.Exception"></exception>
    public static PredictionGA PredictionGAWithParameters(PushGP inSolutionGA, Dictionary<string, string> inParams)
    {
      Type cls = Sharpen.Runtime.GetType(inParams.Get("problem-class"));
      object gaObject = System.Activator.CreateInstance(cls);
      if (!(gaObject is PredictionGA))
      {
        throw (new Exception("Predictor problem-class must inherit from" + " class PredictorGA"));
      }
      PredictionGA ga = (PredictionGA)gaObject;
      // Must set the solution GA before InitFromParameters, since the latter
      // uses _solutionGA while creating the predictor population.
      ga.SetSolutionGA(inSolutionGA);
      ga.SetParams(inParams);
      ga.InitFromParameters();
      return ga;
    }

    /// <exception cref="System.Exception"/>
    protected internal override void InitFromParameters()
    {
      _generationsBetweenTrainers = (int)GetFloatParam("generations-between-trainers");
      _trainerPopulationSize = (int)GetFloatParam("trainer-population-size");
      InitTrainerPopulation();
      base.InitFromParameters();
    }

    /// <summary>Runs a single generation.</summary>
    /// <exception cref="System.Exception"/>
    public virtual void RunGeneration()
    {
      Run(1);
    }

    protected internal override void BeginGeneration()
    {
      if (_generationCount % _generationsBetweenTrainers == _generationsBetweenTrainers - 1)
      {
        // Time to add a new trainer
        PushGPIndividual newTrainer = (PushGPIndividual)ChooseNewTrainer().Clone();
        EvaluateSolutionIndividual(newTrainer);
        _trainerPopulation.Remove(0);
        _trainerPopulation.Add(newTrainer);
        EvaluateTrainerFitnesses();
      }
    }

    public override bool Terminate()
    {
      return false;
    }

    protected internal override bool Success()
    {
      return false;
    }

    /// <summary>
    /// Chooses a new trainer from the solution population to add to the trainer
    /// population.
    /// </summary>
    /// <remarks>
    /// Chooses a new trainer from the solution population to add to the trainer
    /// population. The solution individual is chosen with the highest variance
    /// of the predictions from the current predictor population.
    /// </remarks>
    protected internal virtual PushGPIndividual ChooseNewTrainer()
    {
      AList<float> individualVariances = new AList<float>();
      for (int i = 0; i < _solutionGA.GetPopulationSize(); i++)
      {
        PushGPIndividual individual = (PushGPIndividual)_solutionGA.GetIndividualFromPopulation(i);
        AList<float> predictions = new AList<float>();
        for (int j = 0; j < _populations[_currentPopulation].Length; j++)
        {
          PredictionGAIndividual predictor = (PredictionGAIndividual)_populations[_currentPopulation][j];
          predictions.Add(predictor.PredictSolutionFitness(individual));
        }
        individualVariances.Add(Variance(predictions));
      }
      // Find individual with the highest variance
      int highestVarianceIndividual = 0;
      float highestVariance = individualVariances[0];
      for (int i_1 = 0; i_1 < _solutionGA.GetPopulationSize(); i_1++)
      {
        if (highestVariance < individualVariances[i_1])
        {
          highestVarianceIndividual = i_1;
          highestVariance = individualVariances[i_1];
        }
      }
      return (PushGPIndividual)_solutionGA.GetIndividualFromPopulation(highestVarianceIndividual);
    }

    protected internal virtual PredictionGAIndividual GetBestPredictor()
    {
      float bestFitness = float.MaxValue;
      GAIndividual bestPredictor = _populations[_currentPopulation][0];
      foreach (GAIndividual ind in _populations[_currentPopulation])
      {
        if (ind.GetFitness() < bestFitness)
        {
          bestPredictor = ind;
          bestFitness = ind.GetFitness();
        }
      }
      return (PredictionGAIndividual)bestPredictor;
    }

    /// <summary>
    /// Calculates and sets the exact fitness from any individual of the
    /// _solutionGA population.
    /// </summary>
    /// <remarks>
    /// Calculates and sets the exact fitness from any individual of the
    /// _solutionGA population. This includes trainers.
    /// </remarks>
    /// <param name="inIndividual"/>
    protected internal virtual void EvaluateSolutionIndividual(PushGPIndividual inIndividual)
    {
      _solutionGA.EvaluateIndividual(inIndividual);
    }

    protected internal virtual void SetSolutionGA(PushGP inGA)
    {
      _solutionGA = inGA;
    }

    /// <summary>
    /// This must be private, since there must be a _solutionGA set before this
    /// method is invoked.
    /// </summary>
    /// <remarks>
    /// This must be private, since there must be a _solutionGA set before this
    /// method is invoked. Use SetGAandTrainers() instead.
    /// </remarks>
    private void InitTrainerPopulation()
    {
      _trainerPopulation = new AList<PushGPIndividual>();
      PushGPIndividual individual = new PushGPIndividual();
      for (int i = 0; i < _trainerPopulationSize; i++)
      {
        _trainerPopulation.Add((PushGPIndividual)individual.Clone());
        _solutionGA.InitIndividual(_trainerPopulation[i]);
      }
      EvaluateTrainerFitnesses();
    }

    protected internal override string Report()
    {
      string report = base.Report();
      report = report.Replace('-', '#');
      report = report.Replace("Report for", " Predictor");
      report += ";; Best Predictor: " + _populations[_currentPopulation][_bestIndividual] + "\n";
      report += ";; Best Predictor Fitness: " + _bestMeanFitness + "\n\n";
      report += ";; Mean Predictor Fitness: " + _populationMeanFitness + "\n";
      // The following code prints all of the predictors.
      /*
      report += "\n;; Mean Predictor Fitness: \n";
      for(GAIndividual predictor : _populations[_currentPopulation]){
      report += "          " + predictor + "\n";
      
      }
      */
      report += ";;########################################################;;\n\n";
      return report;
    }

    protected internal override string FinalReport()
    {
      return string.Empty;
    }

    private float Variance(AList<float> list)
    {
      float sampleMean = SampleMean(list);
      float sum = 0;
      foreach (float element in list)
      {
        sum += (element - sampleMean) * (element - sampleMean);
      }
      return (sum / (list.Count - 1));
    }

    private float SampleMean(AList<float> list)
    {
      float total = 0;
      foreach (float element in list)
      {
        total += element;
      }
      return (total / list.Count);
    }

    /// <summary>Initiates inIndividual as a random predictor individual.</summary>
    protected internal abstract override void InitIndividual(GAIndividual inIndividual);

    /// <summary>
    /// Evaluates a PredictionGAIndividual's fitness, based on the difference
    /// between the predictions of the fitnesses of the trainers and the actual
    /// fitnesses of the trainers.
    /// </summary>
    protected internal abstract override void EvaluateIndividual(GAIndividual inIndividual);

    /// <summary>
    /// Determines the predictor's fitness on a trainer, where the trainer is the
    /// inInput, and the trainer's actual fitness (or rank, whatever is to be
    /// predicted) is inOutput.
    /// </summary>
    /// <returns>Predictor's fitness (or rank, etc.) for the given trainer.</returns>
    public abstract override float EvaluateTestCase(GAIndividual inIndividual, object inInput, object inOutput);

    /// <summary>
    /// Actual fitnesses of trainers will always be stored as part of the
    /// PushGPIndividual object.
    /// </summary>
    /// <remarks>
    /// Actual fitnesses of trainers will always be stored as part of the
    /// PushGPIndividual object. Some predictor types, such as rank predictors,
    /// will also need a separate storage of data, such as a method of storing
    /// the ranking of the predictors. Others, such as fitness predictors, may
    /// just need the fitness information directly from the trainers. This
    /// function may be used to make sure fitnesses or ranks are updated, i.e. to
    /// recalculate rank order with the addition of a new trainer.
    /// </remarks>
    protected internal abstract void EvaluateTrainerFitnesses();

    protected internal abstract override GAIndividual ReproduceByMutation(int inIndex);

    protected internal abstract override GAIndividual ReproduceByCrossover(int inIndex);
  }
}
