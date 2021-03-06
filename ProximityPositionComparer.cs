﻿/*
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

using System.Collections;
using UnityEngine;

namespace NTBUtils
{
    public class ProximityPositionComparer<T> : IComparer
        where T : Component
    {
        private readonly Transform center;

        public ProximityPositionComparer(Transform center)
        {
            this.center = center;
        }

        public int Compare(object x, object y)
        {
            var xobj = (T) x;
            var yobj = (T) y;

            var distx = Vector3.Distance(this.center.transform.position,
                xobj.transform.position);
            var disty = Vector3.Distance(this.center.transform.position,
                yobj.transform.position);

            if (distx < disty) return -1;
            if (distx > disty) return 1;
            return 0; //yeah, right.
        }
    }

    public class ProximityPositionComparer : IComparer
    {
        private readonly Transform center;

        public ProximityPositionComparer(Transform center)
        {
            this.center = center;
        }

        public int Compare(object x, object y)
        {
            var xobj = (RaycastHit) x;
            var yobj = (RaycastHit) y;

            var distx = Vector3.Distance(this.center.transform.position,
                xobj.transform.position);
            var disty = Vector3.Distance(this.center.transform.position,
                yobj.transform.position);

            if (distx < disty) return -1;
            if (distx > disty) return 1;
            return 0; //yeah, right.
        }
    }
}