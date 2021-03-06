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

namespace NTBUtils
{
  public class ZTimer
  {
    private bool Ticking = false;
    public float StartAt;
    private float remainder;

    public delegate void ZTWhenDone();

    private ZTWhenDone del = null;

    public ZTimer()
    {
    }

    public ZTimer(float startTime)
    {
      this.Start(startTime);
    }

    public void Tick(float dt)
    {
      if (this.Ticking && this.remainder > 0.0)
      {
        this.remainder = this.remainder - dt;
      }
      if (this.remainder <= 0)
      {
        if (this.del != null)
        {
          this.del();
        }
      }
    }

    public void DoWhenDone(ZTWhenDone func)
    {
      this.del = func;
    }

    public float Remaining()
    {
      return this.remainder;
    }

    public float Elapsed()
    {
      return (this.StartAt - this.remainder);
    }

    public void Start(float time)
    {
      this.StartAt = time;
      this.remainder = time;
      this.Ticking = true;
    }

    public void Extend(float time)
    {
      if (!this.Ticking)
      {
        this.Start(time);
        return;
      }
      this.remainder += time;
      this.StartAt += time;
    }

    public void Reset()
    {
      this.remainder = this.StartAt;
      this.Ticking = true;
    }

    public void Stop()
    {
      this.Ticking = false;
    }

    public bool Running()
    {
      return this.Ticking;
    }

    public bool DoneOrNotTicking()
    {
      return (!this.Ticking || this.remainder <= 0);
    }

    public bool Done()
    {
      if (this.Ticking && this.remainder <= 0)
      {
        this.Ticking = false;
        return true;
      }
      return false;
    }
  }
}