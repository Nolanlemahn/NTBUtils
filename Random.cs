/*
NTBUtils (Not Too Big Utilities) is a small suite of Unity classes provided
with the intent of simplifying the usage of commonly-used game implementation
paradigms. This is version 0.1.3.

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
using System.Collections.Generic;

namespace NTBUtils
{
  public static partial class NTBUtils
  {

    public class Random
    {
      private static Random PR = null;
      private System.Random gen;

      public static void Init(bool onlyIfNecessary = false)
      {
        if (onlyIfNecessary && Random.Valid()) return;
        int seed = Environment.TickCount;
        UnityEngine.Debug.Log("Seed set as " + seed);
        NTBUtils.Random.PR = new NTBUtils.Random(seed);
      }

      public static void Init(int seed, bool onlyIfNecessary = false)
      {
        if (onlyIfNecessary && Random.Valid()) return;
        UnityEngine.Debug.Log("Seed set as " + seed);
        NTBUtils.Random.PR = new NTBUtils.Random(seed);
      }

      public static bool Valid()
      {
        return (NTBUtils.Random.PR != null);
      }

      public static int Range(int min, int max)
      {
        return NTBUtils.Random.PR.gen.Next(min, max);
      }

      public static float Range(float min, float max)
      {
        System.Random random = NTBUtils.Random.PR.gen;
        return (float) (random.NextDouble() * (max - min) + min);
        //double mantissa = (random.NextDouble() * 2.0) - 1.0;
        //double exponent = Math.Pow(2.0, random.Next(-126, 128));
        //return (float)(mantissa * exponent);
      }

      public static void Shuffle<T>(IList<T> list)
      {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i)
        {
          int r = NTBUtils.Random.Range(i, count);
          T tmp = list[i];
          list[i] = list[r];
          list[r] = tmp;
        }
      }

      public Random(int seed, bool primary = true)
      {
        this.gen = new System.Random(seed);
        if (primary) NTBUtils.Random.PR = this;
      }
    }
  }
}
