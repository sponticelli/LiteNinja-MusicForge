using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MusicForge.Utils
{
  internal static class IListExtensions
  {
    /// <summary>
    /// Shuffles the elements of a list in place.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    /// <param name="list">The list to shuffle.</param>
    public static void Shuffle<T>(this IList<T> list)
    {
      var n = list.Count;
      while (n > 1)
      {
        n--;
        var k = Random.Range(0, n);
        (list[k], list[n]) = (list[n], list[k]);
      }
    }
  }
}