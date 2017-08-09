using System;
using System;
using Psh;
using Sharpen;

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
        FilePath f = new FilePath("tools/PushInstructionSet.txt");
        string line = Params.ReadFileString(f);
        string @out = line.Replace(' ', '\n');
        System.Console.Out.Println(@out);
      }
      catch (Exception e)
      {
        Sharpen.Runtime.PrintStackTrace(e);
      }
    }
  }
}
