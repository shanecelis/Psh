using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpenMinimal {

public static class Runtime {

  public static string Substring (string str, int index) {
    return str.Substring (index);
  }

  public static string Substring(string str, int index, int endIndex) {
    return str.Substring (index, endIndex - index);
  }
}

public static class Extensions {


  /// <summary>
  /// Call this trim method instead of standard .Net string.Trim(),
  /// becase .Net string.Trim() method removes only spaces and the Java String.Trim()
  /// removes all chars less than space ' '
  /// </summary>
  /// <remarks>Implementation ported from openjdk source</remarks>
  /// <param name="str">Source string</param>
  /// <returns>Trimmed string</returns>
  public static string Trim(this string str) {
    if (string.IsNullOrEmpty(str)) {
      return str;
    }

    int len = str.Length;
    int st = 0;

    while ((st < len) && (str[st] <= ' ')) {
      st++;
    }

    while ((st < len) && (str[len - 1] <= ' ')) {
      len--;
    }

    return ((st > 0) || (len < str.Length)) ? str.Substring(st, len - st) : str;
  }

  public static bool AddItem<T>(this IList<T> list, T item) {
    list.Add(item);
    return true;
  }

  public static bool AddItem<T>(this ICollection<T> list, T item) {
    list.Add(item);
    return true;
  }

  public static bool ContainsKey(this IDictionary d, object key) {
    return d.Contains(key);
  }

  public static U Get2<T, U>(this IDictionary<T, U> d, T key) {
    U val;
    d.TryGetValue(key, out val);
    return val;
  }

  public static object Get2(this IDictionary d, object key) {
    return d[key];
  }

  public static U Put2<T, U>(this IDictionary<T, U> d, T key, U value) {
    U old;
    d.TryGetValue(key, out old);
    d[key] = value;
    return old;
  }

  public static object Put2(this IDictionary d, object key, object value) {
    object old = d[key];
    d[key] = value;
    return old;
  }

  public static void PutAll<T, U>(this IDictionary<T, U> d, IDictionary<T, U> values) {
    foreach (KeyValuePair<T, U> val in values)
      d[val.Key] = val.Value;
  }

  public static object Put2(this Hashtable d, object key, object value) {
    object old = d[key];
    d[key] = value;
    return old;
  }

  public static string[] Split(this string str, string regex) {
    return str.Split(regex, 0);
  }

  public static string[] Split(this string str, string regex, int limit) {
    Regex rgx = new Regex(regex);
    List<string> list = new List<string>();
    int startIndex = 0;
    if (limit != 1) {
      int nm = 1;
      foreach (Match match in rgx.Matches(str)) {
        list.Add(str.Substring(startIndex, match.Index - startIndex));
        startIndex = match.Index + match.Length;
        if (limit > 0 && ++nm == limit)
          break;
      }
    }
    if (startIndex < str.Length) {
      list.Add(str.Substring(startIndex));
    }
    if (limit >= 0) {
      int count = list.Count - 1;
      while ((count >= 0) && (list[count].Length == 0)) {
        count--;
      }
      list.RemoveRange(count + 1, (list.Count - count) - 1);
    }
    return list.ToArray();
  }

  public static bool Matches(this string str, string regex) {
    Regex regex2 = new Regex(regex);
    return regex2.IsMatch(str);
  }

  public static T Remove<T>(this IList<T> list, int i) {
    T old;
    try {
      old = list[i];
      list.RemoveAt(i);
    } catch (ArgumentOutOfRangeException) {
      throw new Exception("No such element");
    }
    return old;
  }
}
}
