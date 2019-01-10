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

﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace NTBUtils
{
    public struct ResolutionPair
    {
        public ResolutionPair(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        public double width;
        public double height;
    }

    public class RPList : List<ResolutionPair>
    {
        public int Find(int x, int y)
        {
            int i = 0;
            foreach (ResolutionPair resolution in this)
            {
                // "close enough"
                if (Math.Abs(x - resolution.width) < 0.05 &&
                    Math.Abs(y - resolution.height) < 0.05)
                    return i;
                i++;
            }

            return -1;
        }
    }

    public class ResolutionSet
    {
        public string AspectRatio = "0:0";
        public double RawRatio = 0.0;

        public ResolutionSet(double width, double height)
        {
            this.RawRatio = width / height;
            this.AspectRatio = "" + width + ":" + height;
        }

        public RPList GetData()
        {
            return this.data;
        }

        private RPList data = new RPList();

        public void ForceAdd(int x, int y)
        {
            this.data.Add(new ResolutionPair(x, y));
            this.data.Sort(ResolutionSupport.ResolutionSetSort);
        }

        // An add will fail if the aspect ratio is inappropriate for this ResolutionSet
        public bool Add(ResolutionPair res)
        {
            if (Math.Abs(this.RawRatio - (res.width / res.height)) > 0.05)
            {
                return false;
            }

            this.data.Add(res);
            return true;
        }
    }

// ResolutionSupport.Screens() returns 3 arrays,
// corresponding to 3 aspect ratios. In order,
// these are [4:3] [16:9] and [16:10]. Unity dictates
// a bucketton more aspect ratios, but I strip these out.
// The menu should change ResolutionSupport.FullScreen
// and then call ChangeResolution(int x, int y) using
// one of the StrippedResolutions returned.

    public static class ResolutionSupport
    {
        public static bool FullScreen = false;

        public static void ChangeResolution(ResolutionPair res)
        {
            Screen.SetResolution((int) res.width, (int) res.height, ResolutionSupport.FullScreen);
        }

        public static void ChangeResolution(int x, int y)
        {
            Screen.SetResolution(x, y, ResolutionSupport.FullScreen);
        }

        public static int ResolutionSetSort(ResolutionPair x, ResolutionPair y)
        {
            if (x.width < y.width) return -1;
            else if (x.height < y.height) return -1;
            else return 0;
        }

        public static ResolutionSet[] Screens()
        {
            ResolutionSet[] ret = new ResolutionSet[3];
            Resolution[] allData = Screen.resolutions;

            // Find and rewrite legal resolutions
            List<ResolutionPair> resolutions = new List<ResolutionPair>();
            foreach (Resolution res in allData)
            {
                ResolutionPair entry =
                    new ResolutionPair(res.width, res.height);
                if (!resolutions.Contains(entry))
                    resolutions.Add(entry);
            }

            // Separate by aspect ratios
            ResolutionSet fourVthree = new ResolutionSet(4, 3);
            ResolutionSet sixteenVnine = new ResolutionSet(16, 9);
            ResolutionSet sixteenVten = new ResolutionSet(16, 10);
            foreach (var res in resolutions)
            {
                // strip small and semi-illegal resolutions
                if (res.width < 800) continue;
                if (fourVthree.Add(res) || sixteenVnine.Add(res) || sixteenVten.Add(res)) continue;
            }

            // Sort by ascending width and ascending height
            ret[0] = fourVthree;
            ret[1] = sixteenVnine;
            ret[2] = sixteenVten;
            foreach (var set in ret)
            {
                set.GetData().Sort(ResolutionSetSort);
            }

            return ret;
        }
    }
}
