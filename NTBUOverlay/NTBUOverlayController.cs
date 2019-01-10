/*
NTBUtils (Not Too Big Utilities) is a small suite of Unity classes provided
with the intent of simplifying the usage of commonly-used game implementation
paradigms. This is version 0.1.4x.

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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class NTBUOverlay
{
  public static GameObject CanvasObj;
  public static bool Active = false;

  public static NTBUOverlayController Controller = null;

  public static string PreLog = "";

  public static void Log(object msg)
  {
    if(!NTBUOverlay.Controller)
    {
      NTBUOverlay.PreLog += (msg + "\n");
      return;
    }
    NTBUOverlay.Controller.Log(msg);
  }

  public static void WriteLine(object msg)
  {
    NTBUOverlay.Controller.WriteLine(msg);
  }
}

public class NTBUOverlayController : MonoBehaviour
{
  public GameObject ContainerObj;
  public Text RefreshedOutput;
  public Text StandardOutput;
  private string subBuffer = "";

  // Use this for initialization
  void Start()
  {
    NTBUOverlay.CanvasObj = this.ContainerObj;
    NTBUOverlay.Controller = this;
    Debug.Log("NTBUOverlay initiated.");
    NTBUOverlay.Log("NTBUOverlay initiated.");
  }

  // Update is called once per frame
  void Update()
  {
    this.RefreshedOutput.text = "Outed:\n" + this.subBuffer;
    this.subBuffer = "";

    if(NTBUOverlay.PreLog != "")
    {
      this.Log(NTBUOverlay.PreLog);
      NTBUOverlay.PreLog = "";
    }

    if (Input.GetButtonUp("NTBUDKey"))
    {
      NTBUOverlay.CanvasObj.SetActive(!NTBUOverlay.Active);
      NTBUOverlay.Active = !NTBUOverlay.Active;
    }
  }

  public void Log(object msg)
  {
    this.StandardOutput.text += (msg + "\n");
  }

  public void WriteLine(object msg)
  {
    this.subBuffer += (msg + "\n");
  }
}
