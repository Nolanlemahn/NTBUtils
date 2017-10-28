/*******************************************************************************
File:         NTBGUIDemo.cs
Author:       Nolan T Yoo
Date Created: 9/19/2017

Description: Demos NTBGUI features.

Copyright: All content © 2017 DigiPen (USA) Corporation, all rights reserved.
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class NTBGUIDemo : MonoBehaviour
{
  public string SceneName;
}

#if UNITY_EDITOR
[CustomEditor(typeof(NTBGUIDemo))]
[CanEditMultipleObjects]
class NTBGUIDemoGUI : Editor //I like GUIs...
{
  public override void OnInspectorGUI()
  {
    NTBGUIDemo component = target as NTBGUIDemo;

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel("SceneName");
    component.SceneName = NTBGUI.SceneSelection(component.SceneName, target);
    EditorGUILayout.EndHorizontal();

    this.DrawDefaultInspector();
  }
}
#endif