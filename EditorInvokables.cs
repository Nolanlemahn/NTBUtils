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
using UnityEngine;
using NTBUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IHasEditorInvokables
{
  void setInvokables();
}

[AttributeUsage(AttributeTargets.Method)]
public class EditorInvokable : Attribute { }

[Serializable]
public class EditorInvokablesList : List<Action>
{
  public EditorInvokablesList() : base()
  {}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EditorInvokablesList))]
public class MethodFieldDrawer : PropertyDrawer
{
  private int numLines;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (property == null)
      return;

    IHasEditorInvokables objInterface = property.GetParent() as IHasEditorInvokables;
    if (objInterface == null) return;
    objInterface.setInvokables();

    var methodField = fieldInfo.GetValue(property.serializedObject.targetObject) as EditorInvokablesList;
    if (methodField == null) return;

    this.numLines = methodField.Count + 1;

    position.height /= numLines;
    EditorGUI.LabelField(position, string.Format("{0}", label.text), EditorStyles.centeredGreyMiniLabel);
    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    try
    {
      foreach (var method in methodField)
      {
        if (GUI.Button(position, string.Format("{0}()", method.Method.Name)))
        {
          method.Invoke();
        }
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
      }
    }
    catch (IndexOutOfRangeException)
    {
      // ignored
    }
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * this.numLines;
  }

}
#endif