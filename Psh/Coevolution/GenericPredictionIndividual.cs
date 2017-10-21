using System;
using Psh;

namespace Psh.Coevolution
{
  public class GenericPredictionIndividual : PredictionGAIndividual
  {
    protected internal Program _program;

    protected internal PushGP _solutionGA;

    public GenericPredictionIndividual()
    {
      _solutionGA = null;
    }

    public GenericPredictionIndividual(Program inProgram, PushGP inSolutionGA)
    {
      _program = inProgram;
      _solutionGA = inSolutionGA;
    }

    public override float PredictSolutionFitness(PushGPIndividual pgpIndividual)
    {
      //TODO implement _program being run to predict fitness
      return -2999;
    }

    public override GAIndividual Clone()
    {
      return new Psh.Coevolution.GenericPredictionIndividual(_program, _solutionGA);
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
  }
}
