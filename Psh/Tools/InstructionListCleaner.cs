using System;
using Psh;

namespace Psh.Tools
{
  public class InstructionListCleaner
  {
    /// <summary>
    /// Cleans the file PushInstructionSet.text from a single line of
    /// instructions to a list of instructions.
    /// </summary>
    /// <param name="args"/>
    public static void Main(string[] args)
    {
      try
      {
        string line = Params.ReadFileString("tools/PushInstructionSet.txt");
        string @out = line.Replace(' ', '\n');
        Console.Out.WriteLine(@out);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.StackTrace);
        // Sharpen.Runtime.PrintStackTrace(e);
      }
    }
  }
}
