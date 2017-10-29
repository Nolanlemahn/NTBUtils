using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class EditorInvokable : Attribute { }

[Serializable]
public class EditorInvokablesList : List<Action>
{}

[CustomPropertyDrawer(typeof(EditorInvokablesList))]
public class MethodFieldDrawer : PropertyDrawer
{
  private int numLines;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (property == null)
      return;
    

    var methodField = fieldInfo.GetValue(property.serializedObject.targetObject) as EditorInvokablesList;
    if (methodField == null) return;
    this.numLines = methodField.Count + 1;


    position.height /= numLines;
    EditorGUI.LabelField(position, $"{label.text}", EditorStyles.centeredGreyMiniLabel);
    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    foreach (var method in methodField)
    {
      if (GUI.Button(position, $"{method.Method.Name}()"))
      {
        method.Invoke();
      }
      position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }    
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * this.numLines;
  }

}

