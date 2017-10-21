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
using System.IO;
using Sharpen;

namespace Psh
{
  /// <summary>An abstract class for running genetic algorithms.</summary>
  [System.Serializable]
  public abstract class GA
  {

    protected internal GAIndividual[][] _populations;

    protected internal int _currentPopulation;

    protected internal int _generationCount;

    protected internal float _mutationPercent;

    protected internal float _crossoverPercent;

    protected internal float _bestMeanFitness;

    protected internal double _populationMeanFitness;

    protected internal int _bestIndividual;

    protected internal List<float> _bestErrors;

    protected internal int _maxGenerations;

    protected internal int _tournamentSize;

    protected internal int _trivialGeographyRadius;

    protected internal Random Rng;

    protected internal Dictionary<string, string> _parameters;

    public List<GATestCase> _testCases;

    protected internal Type _individualClass;

    [System.NonSerialized]
    protected internal TextWriter _outputStream;

    protected internal Psh.Checkpoint _checkpoint;

    protected internal string _checkpointPrefix;

    protected internal string _outputfile;

    /// <summary>
    /// Factor method for creating a GA object, with the GA class specified by
    /// the problem-class parameter.
    /// </summary>
    /// <exception cref="System.Exception"/>
    public static GA GAWithParameters(Dictionary<string, string> inParams)
    {
      Type cls = Sharpen.Runtime.GetType(inParams.Get("problem-class"));
      object gaObject = System.Activator.CreateInstance(cls);
      if (!(gaObject is GA))
      {
        throw (new Exception("problem-class must inherit from class GA"));
      }
      GA ga = (GA)gaObject;
      ga.SetParams(inParams);
      ga.InitFromParameters();
      return ga;
    }

    /// <exception cref="System.Exception"/>
    public static GA GAWithCheckpoint(string checkpoint)
    {
      FilePath checkpointFile = new FilePath(checkpoint);
      FileInputStream zin = new FileInputStream(checkpointFile);
      GZIPInputStream @in = new GZIPInputStream(zin);
      ObjectInputStream oin = new ObjectInputStream(@in);
      Psh.Checkpoint ckpt = (Psh.Checkpoint)oin.ReadObject();
      GA ga = ckpt.ga;
      ga._checkpoint = ckpt;
      ckpt.checkpointNumber++;
      // because it gets increased only after ckpt is
      // written
      oin.Close();
      System.Console.Out.Println(ckpt.report.ToString());
      // Do we want to append to the file if it exists? Or just overwrite it?
      // Heu! Quae enim quaestio animas virorum vero pertemptit.
      // Wowzers! This is, indeed, a question that truly tests mens' souls.
      if (ga._outputfile != null)
      {
        ga._outputStream = new StreamWriter(new FilePath(ga._outputfile));
      }
      else
      {
        ga._outputStream = System.Console.Out;
      }
      return ga;
    }

    protected internal GA()
    {
      Rng = new Random();
      _testCases = new AList<GATestCase>();
      _bestMeanFitness = float.MaxValue;
      _outputStream = System.Console.Out;
    }

    /// <summary>Sets the parameters dictionary for this GA run.</summary>
    protected internal virtual void SetParams(Dictionary<string, string> inParams)
    {
      _parameters = inParams;
    }

    /// <summary>
    /// Utility function to fetch an non-optional string value from the parameter
    /// list.
    /// </summary>
    /// <param name="inName">the name of the parameter.</param>
    /// <exception cref="System.Exception"/>
    protected internal virtual string GetParam(string inName)
    {
      return GetParam(inName, false);
    }

    /// <summary>
    /// Utility function to fetch a string value from the parameter list,
    /// throwing an exception.
    /// </summary>
    /// <param name="inName">the name of the parameter.</param>
    /// <param name="inOptional">
    /// whether the parameter is optional. If a parameter is not
    /// optional, this method will throw an exception if the parameter
    /// is not found.
    /// </param>
    /// <returns>the string value for the parameter.</returns>
    /// <exception cref="System.Exception"/>
    protected internal virtual string GetParam(string inName, bool inOptional)
    {
      string value = _parameters.Get(inName);
      if (value == null && !inOptional)
      {
        throw new Exception("Could not locate required parameter \"" + inName + "\"");
      }
      return value;
    }

    /// <summary>
    /// Utility function to fetch an non-optional float value from the parameter
    /// list.
    /// </summary>
    /// <param name="inName">the name of the parameter.</param>
    /// <exception cref="System.Exception"/>
    protected internal virtual float GetFloatParam(string inName)
    {
      return GetFloatParam(inName, false);
    }

    /// <summary>
    /// Utility function to fetch a float value from the parameter list, throwing
    /// an exception.
    /// </summary>
    /// <param name="inName">the name of the parameter.</param>
    /// <param name="inOptional">
    /// whether the parameter is optional. If a parameter is not
    /// optional, this method will throw an exception if the parameter
    /// is not found.
    /// </param>
    /// <returns>the float value for the parameter.</returns>
    /// <exception cref="System.Exception"/>
    protected internal virtual float GetFloatParam(string inName, bool inOptional)
    {
      string value = _parameters.Get(inName);
      if (value == null && !inOptional)
      {
        throw new Exception("Could not locate required parameter \"" + inName + "\"");
      }
      if (value == null)
      {
        return float.NaN;
      }
      return float.Parse(value);
    }

    /// <summary>Sets up the GA object with the previously set parameters.</summary>
    /// <remarks>
    /// Sets up the GA object with the previously set parameters. This method is
    /// typically overridden to read in custom parameters associated with custom
    /// subclasses. Subclasses must always call the base class implementation
    /// first to ensure that all base parameters are setup.
    /// </remarks>
    /// <exception cref="System.Exception"/>
    protected internal virtual void InitFromParameters()
    {
      // Default parameters to be used when optional parameters are not
      // given.
      int defaultTrivialGeographyRadius = 0;
      string defaultIndividualClass = "Psh.PushGPIndividual";
      string individualClass = GetParam("individual-class", true);
      if (individualClass == null)
      {
        individualClass = defaultIndividualClass;
      }
      _individualClass = Sharpen.Runtime.GetType(individualClass);
      _mutationPercent = GetFloatParam("mutation-percent");
      _crossoverPercent = GetFloatParam("crossover-percent");
      _maxGenerations = (int)GetFloatParam("max-generations");
      _tournamentSize = (int)GetFloatParam("tournament-size");
      // trivial-geography-radius is an optional parameter
      if (float.IsNaN(GetFloatParam("trivial-geography-radius", true)))
      {
        _trivialGeographyRadius = defaultTrivialGeographyRadius;
      }
      else
      {
        _trivialGeographyRadius = (int)GetFloatParam("trivial-geography-radius", true);
      }
      _checkpointPrefix = GetParam("checkpoint-prefix", true);
      _checkpoint = new Psh.Checkpoint(this);
      ResizeAndInitialize((int)GetFloatParam("population-size"));
      _outputfile = GetParam("output-file", true);
      if (_outputfile != null)
      {
        _outputStream = new StreamWriter(new FilePath(_outputfile));
      }
    }

    /// <summary>
    /// Sets the population size and resets the GA generation count, as well as
    /// initializing the population with random individuals.
    /// </summary>
    /// <param name="inSize">the size of the new GA population.</param>
    /// <exception cref="System.Exception"/>
    protected internal virtual void ResizeAndInitialize(int inSize)
    {
      _populations = new GAIndividual[][] { new GAIndividual[inSize], new GAIndividual[inSize] };
      _currentPopulation = 0;
      _generationCount = 0;
      object iObject = System.Activator.CreateInstance(_individualClass);
      if (!(iObject is GAIndividual))
      {
        throw new Exception("individual-class must inherit from class GAIndividual");
      }
      GAIndividual individual = (GAIndividual)iObject;
      for (int i = 0; i < inSize; i++)
      {
        _populations[0][i] = individual.Clone();
        InitIndividual(_populations[0][i]);
      }
    }

    /// <summary>Run the main GP run loop until the generation limit it met.</summary>
    /// <returns>true, indicating the the execution of the GA is complete.</returns>
    /// <exception cref="System.Exception"/>
    public virtual bool Run()
    {
      return Run(-1);
    }

    /// <summary>
    /// Run the main GP run loop until the generation limit it met, or until the
    /// provided number of generations has elapsed.
    /// </summary>
    /// <param name="inGenerations">
    /// The maximum number of generations to run during this call.
    /// This is distinct from the hard generation limit which
    /// determines when the GA is actually complete.
    /// </param>
    /// <returns>true if the the execution of the GA is complete.</returns>
    /// <exception cref="System.Exception"/>
    public virtual bool Run(int inGenerations)
    {
      // inGenerations below must have !=, not >, since often inGenerations
      // is called at -1
      while (!Terminate() && inGenerations != 0)
      {
        BeginGeneration();
        Evaluate();
        Reproduce();
        EndGeneration();
        Print(Report());
        Checkpoint();
        System.GC.Collect();
        _currentPopulation = (_currentPopulation == 0 ? 1 : 0);
        _generationCount++;
        inGenerations--;
      }
      if (Terminate())
      {
        // Since this value was changed after termination conditions were
        // set, revert back to previous state.
        _currentPopulation = (_currentPopulation == 0 ? 1 : 0);
        Print(FinalReport());
      }
      return (_generationCount < _maxGenerations);
    }

    /// <summary>Determine whether the GA should terminate.</summary>
    /// <remarks>
    /// Determine whether the GA should terminate. This method may be overridden
    /// by subclasses to customize GA behavior.
    /// </remarks>
    public virtual bool Terminate()
    {
      return (_generationCount >= _maxGenerations || Success());
    }

    /// <summary>Determine whether the GA has succeeded.</summary>
    /// <remarks>
    /// Determine whether the GA has succeeded. This method may be overridden by
    /// subclasses to customize GA behavior.
    /// </remarks>
    protected internal virtual bool Success()
    {
      return _bestMeanFitness == 0.0;
    }

    /// <summary>Evaluates the current population and updates their fitness values.</summary>
    /// <remarks>
    /// Evaluates the current population and updates their fitness values. This
    /// method may be overridden by subclasses to customize GA behavior.
    /// </remarks>
    protected internal virtual void Evaluate()
    {
      double totalFitness = 0;
      _bestMeanFitness = float.MaxValue;
      for (int n = 0; n < _populations[_currentPopulation].Length; n++)
      {
        GAIndividual i = _populations[_currentPopulation][n];
        EvaluateIndividual(i);
        totalFitness += i.GetFitness();
        if (i.GetFitness() < _bestMeanFitness)
        {
          _bestMeanFitness = i.GetFitness();
          _bestIndividual = n;
          _bestErrors = i.GetErrors();
        }
      }
      _populationMeanFitness = totalFitness / _populations[_currentPopulation].Length;
    }

    /// <summary>Reproduces the current population into the next population slot.</summary>
    /// <remarks>
    /// Reproduces the current population into the next population slot. This
    /// method may be overridden by subclasses to customize GA behavior.
    /// </remarks>
    protected internal virtual void Reproduce()
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
        _populations[nextPopulation][n] = next;
      }
    }

    /// <summary>Prints out population report statistics.</summary>
    /// <remarks>
    /// Prints out population report statistics. This method may be overridden by
    /// subclasses to customize GA behavior.
    /// </remarks>
    protected internal virtual string Report()
    {
      string report = "\n";
      report += ";;--------------------------------------------------------;;\n";
      report += ";;---------------";
      report += " Report for Generation " + _generationCount + " ";
      if (_generationCount < 10)
      {
        report += "-";
      }
      if (_generationCount < 100)
      {
        report += "-";
      }
      if (_generationCount < 1000)
      {
        report += "-";
      }
      report += "-------------;;\n";
      report += ";;--------------------------------------------------------;;\n";
      return report;
    }

    protected internal virtual string FinalReport()
    {
      bool success = Success();
      string report = "\n";
      report += "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n";
      report += "                        ";
      if (success)
      {
        report += "Success";
      }
      else
      {
        report += "Failure";
      }
      report += " at Generation " + (_generationCount - 1) + "\n";
      report += "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n";
      return report;
    }

    /// <summary>
    /// Logs output of the GA run to the appropriate location (which may be
    /// stdout, or a file).
    /// </summary>
    /// <exception cref="System.Exception"/>
    protected internal virtual void Print(string inStr)
    {
      if (_outputStream != null)
      {
        _outputStream.Write(inStr);//Sharpen.Runtime.GetBytesForString(inStr));
        _outputStream.Flush();
      }
      _checkpoint.report.Append(inStr);
    }

    /// <summary>
    /// Preforms a tournament selection, return the best individual from a sample
    /// of the given size.
    /// </summary>
    /// <param name="inSize">
    /// The number of individuals to consider in the tournament
    /// selection.
    /// </param>
    protected internal virtual GAIndividual TournamentSelect(int inSize, int inIndex)
    {
      int popsize = _populations[_currentPopulation].Length;
      int best = TournamentSelectionIndex(inIndex, popsize);
      float bestFitness = _populations[_currentPopulation][best].GetFitness();
      for (int n = 0; n < inSize - 1; n++)
      {
        int candidate = TournamentSelectionIndex(inIndex, popsize);
        float candidateFitness = _populations[_currentPopulation][candidate].GetFitness();
        if (candidateFitness < bestFitness)
        {
          best = candidate;
          bestFitness = candidateFitness;
        }
      }
      return _populations[_currentPopulation][best];
    }

    /// <summary>Produces an index for a tournament selection candidate.</summary>
    /// <param name="inIndex">
    /// The index which is to be replaced by the current reproduction
    /// event (used only if trivial-geography is enabled).
    /// </param>
    /// <param name="inPopsize">The size of the population.</param>
    /// <returns>the index for the tournament selection.</returns>
    protected internal virtual int TournamentSelectionIndex(int inIndex, int inPopsize)
    {
      if (_trivialGeographyRadius > 0)
      {
        int index = (Rng.Next(_trivialGeographyRadius * 2) - _trivialGeographyRadius) + inIndex;
        if (index < 0)
        {
          index += inPopsize;
        }
        return (index % inPopsize);
      }
      else
      {
        return Rng.Next(inPopsize);
      }
    }

    /// <summary>Clones an individual selected through tournament selection.</summary>
    /// <returns>the cloned individual.</returns>
    protected internal virtual GAIndividual ReproduceByClone(int inIndex)
    {
      return TournamentSelect(_tournamentSize, inIndex).Clone();
    }

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

    /// <summary>Retrieves GAIndividual at index i from the current population.</summary>
    /// <param name="i"/>
    /// <returns>GAIndividual at index i</returns>
    public virtual GAIndividual GetIndividualFromPopulation(int i)
    {
      return _populations[_currentPopulation][i];
    }

    /// <summary>Retrieves best individual from the current population.</summary>
    /// <returns>best GAIndividual in population</returns>
    public virtual GAIndividual GetBestIndividual()
    {
      if (Terminate())
      {
        return _populations[_currentPopulation][_bestIndividual];
      }
      int oldpop = (_currentPopulation == 0 ? 1 : 0);
      return _populations[oldpop][_bestIndividual];
    }

    /// <returns>population size</returns>
    public virtual int GetPopulationSize()
    {
      return _populations[_currentPopulation].Length;
    }

    /// <returns>generation count</returns>
    public virtual int GetGenerationCount()
    {
      return _generationCount;
    }

    public virtual int GetMaxGenerations()
    {
      return _maxGenerations;
    }

    /// <summary>Called at the beginning of each generation.</summary>
    /// <remarks>
    /// Called at the beginning of each generation. This method may be overridden
    /// by subclasses to customize GA behavior.
    /// </remarks>
    /// <exception cref="System.Exception"></exception>
    protected internal virtual void BeginGeneration()
    {
    }

    /// <summary>Called at the end of each generation.</summary>
    /// <remarks>
    /// Called at the end of each generation. This method may be overridden by
    /// subclasses to customize GA behavior.
    /// </remarks>
    protected internal virtual void EndGeneration()
    {
    }

    protected internal abstract void InitIndividual(GAIndividual inIndividual);

    protected internal abstract void EvaluateIndividual(GAIndividual inIndividual);

    public abstract float EvaluateTestCase(GAIndividual inIndividual, object inInput, object inOutput);

    protected internal abstract GAIndividual ReproduceByCrossover(int inIndex);

    protected internal abstract GAIndividual ReproduceByMutation(int inIndex);

    /// <exception cref="System.Exception"/>
    protected internal virtual void Checkpoint()
    {
      if (_checkpointPrefix == null)
      {
        return;
      }
      FilePath file = new FilePath(_checkpointPrefix + _checkpoint.checkpointNumber + ".gz");
      var @out = new StreamWriter(new GZIPOutputStream(new FileOutputStream(file)));
      @out.Write(_checkpoint);
      @out.Flush();
      @out.Close();
      System.Console.Out.Println("Wrote checkpoint file " + file.GetAbsolutePath());
      _checkpoint.checkpointNumber++;
    }
  }
}
