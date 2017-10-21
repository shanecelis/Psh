using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpenMinimal
{
    public static class Extensions
    {

        public static bool AddItem<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return true;
        }

        public static bool AddItem<T>(this ICollection<T> list, T item)
        {
            list.Add(item);
            return true;
        }

        public static bool ContainsKey(this IDictionary d, object key)
        {
            return d.Contains(key);
        }

        public static U Get<T, U>(this IDictionary<T, U> d, T key)
        {
            U val;
            d.TryGetValue(key, out val);
            return val;
        }

        public static object Get(this IDictionary d, object key)
        {
            return d[key];
        }

        public static U Put<T, U>(this IDictionary<T, U> d, T key, U value)
        {
            U old;
            d.TryGetValue(key, out old);
            d[key] = value;
            return old;
        }

        public static object Put(this IDictionary d, object key, object value)
        {
            object old = d[key];
            d[key] = value;
            return old;
        }

        public static void PutAll<T, U>(this IDictionary<T, U> d, IDictionary<T, U> values)
        {
            foreach (KeyValuePair<T, U> val in values)
                d[val.Key] = val.Value;
        }

        public static object Put(this Hashtable d, object key, object value)
        {
            object old = d[key];
            d[key] = value;
            return old;
        }

        public static string[] Split(this string str, string regex)
        {
            return str.Split(regex, 0);
        }

        public static string[] Split(this string str, string regex, int limit)
        {
            Regex rgx = new Regex(regex);
            List<string> list = new List<string>();
            int startIndex = 0;
            if (limit != 1)
            {
                int nm = 1;
                foreach (Match match in rgx.Matches(str))
                {
                    list.Add(str.Substring(startIndex, match.Index - startIndex));
                    startIndex = match.Index + match.Length;
                    if (limit > 0 && ++nm == limit)
                        break;
                }
            }
            if (startIndex < str.Length)
            {
                list.Add(str.Substring(startIndex));
            }
            if (limit >= 0)
            {
                int count = list.Count - 1;
                while ((count >= 0) && (list[count].Length == 0))
                {
                    count--;
                }
                list.RemoveRange(count + 1, (list.Count - count) - 1);
            }
            return list.ToArray();
        }

      public static bool Matches(this string str, string regex)
      {
        Regex regex2 = new Regex(regex);
        return regex2.IsMatch(str);
      }

      public static T Remove<T>(this IList<T> list, int i)
      {
        T old;
        try
        {
          old = list[i];
          list.RemoveAt(i);
        }
        catch (ArgumentOutOfRangeException)
        {
          throw new Exception("No such element");
        }
        return old;
      }
   }
}
