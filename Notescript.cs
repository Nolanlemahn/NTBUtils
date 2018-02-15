using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Notescript : MonoBehaviour
{
  public string Note = "";
}

#if UNITY_EDITOR
[CustomEditor(typeof(Notescript))]
[CanEditMultipleObjects]
class NotescriptGUI : Editor
{
  public override void OnInspectorGUI()
  {
    Notescript component = target as Notescript;

    string oldNote = component.Note;
    string newNote = GUILayout.TextArea(component.Note, int.MaxValue);

    if (oldNote != newNote)
    {
      component.Note = newNote;
      Undo.RecordObject(target, "Typing on Notescript on " + target.name + " [NTBU]");
    }
  }
}
#endif