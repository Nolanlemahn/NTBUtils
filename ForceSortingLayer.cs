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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[NTBUtils.OverrideExecOrder(-101)]
[ExecuteInEditMode]
public class ForceSortingLayer : MonoBehaviour
{
  public string SortingLayerName = "Default";
  public int SortingOrder = 0;

  private string newLayer;

  public void Awake()
  {
    Renderer r = this.gameObject.GetComponent<Renderer>();
    if (r != null)
    {
      r.sortingLayerName = SortingLayerName;
      r.sortingOrder = SortingOrder;
    }
  }

  public void ChangeLayer(string layer)
  {
    this.newLayer = layer;
    NTBUtils.ForRecursive(this.gameObject, this.ObjChangeLayer, false);
  }

  void ObjChangeLayer(Transform child)
  {
    Renderer r = child.gameObject.GetComponent<Renderer>();
    if(r != null) r.sortingLayerName = this.newLayer;
  }

  void OnGUI()
  {
    this.Awake();
  }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ForceSortingLayer))]
//[CanEditMultipleObjects]
class ForceSortingLayerGUI : Editor
{
  private List<string> choices;

  void OnEnable()
  {
    List<string> knownLayers = new List<string>();
    foreach (SortingLayer l in SortingLayer.layers)
    {
      knownLayers.Add(l.name);
    }
    this.choices = knownLayers;
  }

  public override void OnInspectorGUI()
  {
    ForceSortingLayer component = target as ForceSortingLayer;

    int slChoice = this.choices.IndexOf(component.SortingLayerName);
    int edit_slChoice = EditorGUILayout.Popup("Sorting Layer Name", slChoice, this.choices.ToArray());

    int soChoice = component.SortingOrder;
    int edit_soChoice = EditorGUILayout.IntField("Sorting Order", soChoice);

    if (soChoice != edit_soChoice || slChoice != edit_slChoice)
    {
      Undo.RecordObject(target, "Changed Force Sorting Layer on " + target.name);
      component.SortingLayerName = this.choices[edit_slChoice];
      component.SortingOrder = edit_soChoice;
      component.Awake();
    }

    EditorGUILayout.BeginHorizontal();
    if (component.gameObject.GetComponent<Renderer>() != null)
    {
      EditorGUILayout.LabelField("Perceived Layer: ");
      EditorGUILayout.LabelField(component.gameObject.GetComponent<Renderer>().sortingLayerName);
    }
    EditorGUILayout.EndHorizontal();
  }
}
#endif