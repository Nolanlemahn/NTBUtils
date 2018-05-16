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
using NTBUtils;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SDbgPanel : SingletonBehavior<SDbgPanel>
{
  private static Dictionary<string, Pair<bool, object>> logs = new Dictionary<string, Pair<bool, object>>();

  public static void Log(string header, object message)
  {
    SDbgPanel.logs[header] = new Pair<bool, object>(false, message);
  }

  public static Dictionary<string, Pair<bool, object>> All
  {
    get { return SDbgPanel.logs; }
  }

  void LateUpdate()
  {
    Dictionary<string, Pair<bool, object>> panels = SDbgPanel.All;
    List<string> removeByTheseKeys = new List<string>();
    foreach (var item in panels)
    {
      if (item.Value.First)
      {
        removeByTheseKeys.Add(item.Key);
      }
      item.Value.First = true;
    }

    foreach (var k in removeByTheseKeys)
    {
      SDbgPanel.All.Remove(k);
    }
  }
}

#if UNITY_EDITOR
public class SDbgWindow : EditorWindow
{
  public float PaintTimeInterval = 0.5f;

  private ZTimer paintTimer = new ZTimer();

  [MenuItem("Tools/NTBUtils/SDbgPanels %DOWN", false, 0)]
  static void Init()
  {
    // Get existing open window or if none, make a new one:
    SDbgWindow window = (SDbgWindow)EditorWindow.GetWindow(typeof(SDbgWindow));
    window.Show();
  }

  void resetTimerRoutine()
  {
    this.paintTimer.Start(this.PaintTimeInterval);
    this.Repaint();
  }

  void Update()
  {
    if (EditorApplication.isPlaying && !EditorApplication.isPaused)
    {
      if (!this.paintTimer.Running())
      {
        this.paintTimer.Start(this.PaintTimeInterval);
        this.paintTimer.DoWhenDone(this.resetTimerRoutine);
      }
      this.paintTimer.Tick(Time.deltaTime);
    }
  }

  void OnGUI()
  {
    this.PaintTimeInterval = this.PaintTimeInterval.Clamp(0, 1);
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.LabelField("Time Per Paint (< 1.0)", EditorStyles.boldLabel);
    this.PaintTimeInterval = EditorGUILayout.FloatField(this.PaintTimeInterval);
    EditorGUILayout.EndHorizontal();

    if (SDbgPanel.Instance != null)
    {
      Dictionary<string, Pair<bool, object>> panels = SDbgPanel.All;
      foreach (var item in panels)
      {
        GUI.enabled = false;
        try
        {
          EditorGUILayout.TextField(item.Key, item.Value.Second.ToString());
        }
        catch (Exception)
        {
          EditorGUILayout.TextField(item.Key, "UNPRINTABLE");
        }
        GUI.enabled = true;
      }
    }
    else
    {
      GUILayout.Label("Engine is not running, or entries are not being made.", EditorStyles.boldLabel);
    }
  }
}
#endif