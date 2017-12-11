using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Psh {

public class RandomProgram {

  internal Random Rng = new Random();
  protected Dictionary<string, AtomGenerator> _randomGenerators
    = new Dictionary<string, AtomGenerator>();

  protected Dictionary<string, AtomGenerator> availableGenerators;

  public IEnumerable<string> instructions {
    get { return _randomGenerators.Keys; }
  }

  public IEnumerable<string> availableInstructions {
    get { return availableGenerators.Keys; }
  }

  /// <summary>
  /// Generates a single random Push atom (instruction name, integer, float,
  /// etc) for use in random code generation algorithms.
  /// </summary>
  /// <returns>
  /// A random atom based on the interpreter's current active
  /// instruction set.
  /// </returns>
  public virtual object RandomAtom() {
    var generators = _randomGenerators.Values.ToList();
    int index = Rng.Next(generators.Count);
    try {
      return generators[index].Generate();
    } catch (Exception e) {
      throw new Exception("got bad generator for index " + index + " with count " + generators.Count + " and " + _randomGenerators.Count(), e);
    }
  }

  /// <summary>Generates a random Push program of a given size.</summary>
  /// <param name="inSize">The requested size for the program to be generated.</param>
  /// <returns>A random Push program of the given size.</returns>
  public Program RandomCode(int inSize) {
    Program p = new Program();
    IList<int> distribution = RandomCodeDistribution(inSize - 1, inSize - 1);
    foreach(int count in distribution) {
      p.Push(count == 1 ? RandomAtom() : RandomCode(count));
    }
    return p;
  }

  /// <summary>
  /// Generates a list specifying a size distribution to be used for random
  /// code.
  /// </summary>
  /// <remarks>
  /// Generates a list specifying a size distribution to be used for random
  /// code.
  /// Note: This method is called "decompose" in the lisp implementation.
  /// </remarks>
  /// <param name="inCount">The desired resulting program size.</param>
  /// <param name="inMaxElements">The maxmimum number of elements at this level.</param>
  /// <returns>A list of integers representing the size distribution.</returns>
  public IList<int> RandomCodeDistribution(int inCount, int inMaxElements) {
    List<int> result = new List<int>();
    RandomCodeDistribution(result, inCount, inMaxElements);
    Shuffle(result, Rng);
    return result;
  }

  // Fisher-Yates-Durstenfeld shuffle
  //$ cite -ma 1653204 -t excerpt -u \
  // http://stackoverflow.com/questions/1651619/optimal-linq-query-to-get-a-random-sub-collection-shuffle
  public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source, Random rng = null) {
    if (rng == null)
      rng = new Random();
    var buffer = source.ToList();
    for (int i = 0; i < buffer.Count; i++) {
      int j = rng.Next(i, buffer.Count);
      yield return buffer[j];
      buffer[j] = buffer[i];
    }
  }

  /// <summary>The recursive worker function for the public RandomCodeDistribution.</summary>
  /// <param name="ioList">The working list of distribution values to append to.</param>
  /// <param name="inCount">The desired resulting program size.</param>
  /// <param name="inMaxElements">The maxmimum number of elements at this level.</param>
  private void RandomCodeDistribution(IList<int> ioList, int inCount, int inMaxElements) {
    if (inCount < 1) {
      return;
    }
    int thisSize = inCount < 2 ? 1 : (Rng.Next(inCount) + 1);
    ioList.Add(thisSize);
    RandomCodeDistribution(ioList, inCount - thisSize, inMaxElements - 1);
  }

  public void SetInstructions(Interpreter interp, Program inInstructionList) {
    _randomGenerators.Clear();
    for (int n = 0; n < inInstructionList.Size(); n++) {
      object o = inInstructionList.DeepPeek(n);
      string name = null;
      if (o is Instruction) {
        if (interp._instructions.Keys.Contains(o))
          break;
      } else {
        if (o is string) {
          name = (string)o;
        } else {
          throw new Exception("Instruction list must contain a list of Push instruction names only");
        }
      }
      // Check for registered
      // I don't understand this "registered.X" bit.
      if (name.IndexOf("registered.") == 0) {
        string registeredType = SharpenMinimal.Runtime.Substring(name, 11);
        if (   !registeredType.Equals("integer")
            && !registeredType.Equals("float")
            && !registeredType.Equals("boolean")
            && !registeredType.Equals("exec")
            && !registeredType.Equals("code")
            && !registeredType.Equals("name")
            && !registeredType.Equals("input")
            // && !registeredType.Equals("frame")
               ) {
          Console.Error.WriteLine("Unknown instruction \"" + name + "\" in instruction set");
        } else {
          // Legal stack type, so add all generators matching
          // registeredType to _randomGenerators.
          // object[] keys = SharpenMinimal.Collections.ToArray(_instructions.Keys);
          // for (int i = 0; i < keys.Length; i++)
          // {
          //   string key = (string)keys[i];
          foreach (string key in interp._instructions.Keys) {
            // So we're searching for anything with a type prefix.
            if (key.IndexOf(registeredType) == 0) {
              AtomGenerator g = interp._generators[key];
              _randomGenerators[key] = (g);
            }
          }
          if (registeredType.Equals("boolean")) {
            var t = interp._generators["true"];
            _randomGenerators["true"] = (t);
            var f = interp._generators["false"];
            _randomGenerators["false"] = (f);
          }
          if (registeredType.Equals("integer")) {
            var g = interp._generators["integer.erc"];
            _randomGenerators["integer.erc"] = (g);
          }
          if (registeredType.Equals("float")) {
            var g = interp._generators["float.erc"];
            _randomGenerators["float.erc"] = (g);
          }
        }
      } else {
        if (name.IndexOf("input.makeinputs") == 0) {
          string strnum = SharpenMinimal.Runtime.Substring(name, 16);
          int num = System.Convert.ToInt32(strnum);
          for (int i = 0; i < num; i++) {
            interp.DefineInstruction("input.in" + i, new InputInN(i));
            var g = interp._generators["input.in" + i];
            _randomGenerators["input.in" + i] = (g);
          }
        } else {
          var g = interp._generators[name];
          if (g == null) {
            throw new Exception("Unknown instruction \"" + name + "\" in instruction set");
          } else {
            _randomGenerators[name] = (g);
          }
        }
      }
    }
  }

  public virtual void LoadInstructions(Interpreter interp) {
    availableGenerators = new Dictionary<string, AtomGenerator>(interp._generators);
  }

  public virtual void SetInstructions(Interpreter interp, params string[] patterns) {
    LoadInstructions(interp);
    _randomGenerators.Clear();
    AddInstructions(patterns);
  }

  /*
    Provide some regex patterns to white list the instructions.
   */
  public virtual void AddInstructions(params string[] patterns) {
    foreach (var pattern in patterns) {
      var regex = new Regex(pattern);
      // foreach (var instructionName in interp._instructions.Keys.Where(k => regex.IsMatch(k))) {
      //   _randomGenerators[instructionName] = (interp._generators[instructionName]);
      // }
      foreach (var instructionName in availableGenerators.Keys.Where(k => regex.IsMatch(k))) {
        _randomGenerators[instructionName] = availableGenerators[instructionName];
      }
    }
  }

  /*
    Provide some regex patterns to black list the instructions.
  */
  public virtual void RemoveInstructions(params string[] patterns) {
    foreach (var pattern in patterns) {
      var regex = new Regex(pattern);
      // foreach (var instructionName in interp._instructions.Keys.Where(k => regex.IsMatch(k))) {
      //   _randomGenerators[instructionName] = (interp._generators[instructionName]);
      // }
      foreach (var instructionName in _randomGenerators.Keys.Where(k => regex.IsMatch(k)).ToList()) {
        _randomGenerators.Remove(instructionName);
      }
    }
  }

  // <summary>Returns a string of all the instructions used in this run.</summary>
  // <returns/>
  public string GetInstructionsString(Interpreter interp) {
    var strings = _randomGenerators.Keys;
    // List<string> strings = interp._instructions.Keys.ToList();
    // object[] keys = SharpenMinimal.Collections.ToArray(_instructions.Keys);
    // List<string> strings = new List<string>();
    // List<string> strings = _instructions.Keys.Where(key => _randomGenerators.Contains(_generators[key])).ToList();
    // string str = string.Empty;
    // for (int i = 0; i < keys.Length; i++)
    // {
    //   string key = (string)keys[i];
    //   if (_randomGenerators.Contains(_generators[key))]
    //   {
    //     strings.Add(key);
    //   }
    // }

    // XXX But float.erc isn't an instruction is it?
    // if (_randomGenerators.Contains(interp._generators["float.erc"])) {
    //   strings.Add("float.erc");
    // }
    // if (_randomGenerators.Contains(interp._generators["integer.erc"])) {
    //   strings.Add("integer.erc");
    // }
    // return strings.OrderBy(x => x).Aggregate((current, next) => current + next);
    // XXX The performance for this sucks.  Replace it.
    if (strings.Any())
      return strings.OrderBy(x => x).Aggregate((current, next) => current + " " + next);
    else
      return "";
    // strings.Sort();
    // foreach (string s in strings)
    // {
    //   str += s + " ";
    // }
    // return SharpenMinimal.Runtime.Substring(str, 0, str.Length - 1);
  }
}
}
