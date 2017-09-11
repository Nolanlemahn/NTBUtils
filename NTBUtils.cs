/*
NTBUtils (Not Too Big Utilities) is a small suite of Unity classes provided
with the intent of simplifying the usage of commonly-used game implementation
paradigms. This is version 0.1.2.

Copyright (c) 2017 Nolan T Yoo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Except as contained in this notice, the name(s) of the above copyright holders
shall not be used in advertising or otherwise to promote the sale, use or other
dealings in this Software without prior written authorization.
*/

using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public static class NTBUtils
{
  private static class InternalHelpers
  {
    public static void PrintName(Transform target)
    {
      Debug.Log(target.name);
    }
  }

  #region Logging
  // NTBUtils.Log() will be silenced if NTBUtils.logging is false
  private static bool logging = false;

  public static void Log(object message)
  {
    if (NTBUtils.logging) Debug.Log(message);
  }

  public static void LogWarning(object message)
  {
    if (NTBUtils.logging) Debug.LogWarning(message);
  }
  #endregion

  #region RecursiveAction
  public delegate void TransformAction(Transform target);

  public static void ForRecursive(GameObject target, TransformAction action, bool inclParent = true)
  {
    if (inclParent) action(target.transform);
    ForRecursiveHelper(target.transform, action);
  }

  private static void ForRecursiveHelper(Transform target, TransformAction action)
  {
    foreach (Transform child in target.transform)
    {
      action(child);
      NTBUtils.ForRecursiveHelper(child, action);
    }
  }

  // Mostly provided as an example
  public static void PrintAllChildren(GameObject target)
  {
    ForRecursive(target, InternalHelpers.PrintName);
  }
  #endregion

  #region ArrayHelpers
  // Fill an array with a value
  public static void Populate<T>(this T[] arr, T val)
  {
    for (int i = 0; i < arr.Length; i++)
    {
      arr[i] = val;
    }
  }

  // Turn a 1D array into a 2D array
  public static T[,] Make2D<T>(T[] input, int height, int width)
  {
    T[,] output = new T[height, width];
    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        output[i, j] = input[i * width + j];
      }
    }
    return output;
  }

  public static void Print2D<T>(T[,] matrix)
  {
    string line = "";
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
      for (int j = 0; j < matrix.GetLength(1); j++)
      {
        line += (matrix[i, j] + "\t");
      }
      line += "\n";
    }
    Debug.Log(line);
  }
  #endregion

  #region OtherHelpers

  // Instance Setting helper for SingletonBehaviors
  public static bool SetInstance<T>(ref T target, T instance) where T : MonoBehaviour
  {
    // If the target is null, set it equal to the instance
    if (Object.ReferenceEquals(target, null))
    {
      target = instance;
      return true;
    }
    else if (!Object.ReferenceEquals(target, instance))
    {
      Object.Destroy(instance.gameObject);
      return false;
    }
    return false;
  }

  // Usage: VAR = NUMERIC.Clamp(min, max)
  public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
  {
    if (val.CompareTo(min) < 0) return min;
    else if (val.CompareTo(max) > 0) return max;
    else return val;
  }
  #endregion
}

public class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
  // Ensure that there's only one!
  public static T Instance;

  protected virtual void Awake()
  {
    // What ensures this is a Singleton.
    NTBUtils.SetInstance(ref Instance, this as T);
  }
  protected virtual void OnDestroy()
  {
    SingletonBehavior<T>.Instance = null;
  }
}

#region DataClasses
public class Pair<X, Y>
{
  public Pair()
  {
  }

  public Pair(X first, Y second)
  {
    this.First = first;
    this.Second = second;
  }

  public X First { get; set; }
  public Y Second { get; set; }
};

public class Ref<T>
{
  private readonly Func<T> getter;
  private readonly Action<T> setter;

  public Ref(Func<T> getter, Action<T> setter)
  {
    this.getter = getter;
    this.setter = setter;
  }

  public T Value
  {
    get { return getter(); }
    set { setter(value); }
  }
}
#endregion
