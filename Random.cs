using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
