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
//TODO: Doxygen? This being deployed as a single file is intentional by design.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace NTBUtils
{
  public class CoroutineWithCallback : IEnumerator
  {
    private IEnumerator Coroutine;
    private Action[] OnEnds;

    public CoroutineWithCallback(IEnumerator Coroutine, Action OnEnd)
    {
      this.Coroutine = Coroutine;
      this.OnEnds = new Action[] {OnEnd};
    }

    public CoroutineWithCallback(IEnumerator Coroutine, Action[] OnEnds)
    {
      this.Coroutine = Coroutine;
      this.OnEnds = OnEnds;
    }

    public bool MoveNext()
    {
      bool next = this.Coroutine.MoveNext();
      if (!next)
      {
        foreach (Action action in this.OnEnds)
        {
          action.Invoke();
        }
      }
      return next;
    }

    public void Reset()
    {
      this.Coroutine.Reset();
    }

    public object Current
    {
      get { return this.Coroutine.Current; }
    }
  }

  public class CoroutineWithCallback<T> : IEnumerator
  {
    private IEnumerator Coroutine;
    private Action<T> OnEnd;
    private T Actionable;

    public CoroutineWithCallback(IEnumerator Coroutine, Action<T> OnEnd, T Actionable = default(T))
    {
      this.Coroutine = Coroutine;
      this.OnEnd = OnEnd;
      this.Actionable = Actionable;
    }

    public bool MoveNext()
    {
      bool next = this.Coroutine.MoveNext();
      if (!next)
      {
        this.OnEnd.Invoke(this.Actionable);
      }
      return next;
    }

    public void Reset()
    {
      this.Coroutine.Reset();
    }

    public object Current
    {
      get { return this.Coroutine.Current; }

    }
  }

  public static class GameObjectExtensions
  {
    public static List<GameObject> GetChildren(this Transform t)
    {
      List<GameObject> ret = new List<GameObject>();
      foreach (Transform ct in t)
      {
        ret.Add(ct.gameObject);
      }
      return ret;
    }
  }

#if UNITY_EDITOR
  public static class PropertyExtensions
  {
    public static object GetParent(this SerializedProperty prop)
    {
      var path = prop.propertyPath.Replace(".Array.data[", "[");
      object obj = prop.serializedObject.targetObject;
      var elements = path.Split('.');
      foreach (var element in elements.Take(elements.Length - 1))
      {
        if (element.Contains("["))
        {
          var elementName = element.Substring(0, element.IndexOf("["));
          var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
          obj = GetValue(obj, elementName, index);
        }
        else
        {
          obj = GetValue(obj, element);
        }
      }
      return obj;
    }

    public static object GetValue(object source, string name)
    {
      if (source == null)
        return null;
      var type = source.GetType();
      var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      if (f == null)
      {
        var p = type.GetProperty(name,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (p == null)
          return null;
        return p.GetValue(source, null);
      }
      return f.GetValue(source);
    }

    public static object GetValue(object source, string name, int index)
    {
      var enumerable = GetValue(source, name) as IEnumerable;
      var enm = enumerable.GetEnumerator();
      while (index-- >= 0)
        enm.MoveNext();
      return enm.Current;
    }
  }
#endif

  public static class ListExtensions
  {
    public static void Resize<T>(this List<T> list, int newSize, T c)
    {
      int currSize = list.Count;
      if (newSize < currSize)
        list.RemoveRange(newSize, currSize - newSize);
      else if (newSize > currSize)
      {
        if (newSize > list.Capacity)
          list.Capacity = newSize;
        list.AddRange(Enumerable.Repeat(c, newSize - currSize));
      }
    }

    public static void Resize<T>(this List<T> list, int sz)
    {
      ListExtensions.Resize(list, sz, default(T));
    }

    public static void UShuffle<T>(this IList<T> list)
    {
      int count = list.Count;
      int last = count - 1;
      for (int i = 0; i < last; ++i)
      {
        int r = UnityEngine.Random.Range(i, count);
        T tmp = list[i];
        list[i] = list[r];
        list[r] = tmp;
      }
    }
  }

  public static class QuaternionExtensions
  {
    public static Quaternion Pow(this Quaternion input, float power)
    {
      float inputMagnitude = input.Magnitude();
      Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
      Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
        .ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
        .Exp();
      return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
    }

    public static Quaternion Exp(this Quaternion input)
    {
      float inputA = input.w;
      Vector3 inputV = new Vector3(input.x, input.y, input.z);
      float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
      Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
      return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
    }

    public static float Magnitude(this Quaternion input)
    {
      return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
    }

    public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
    {
      return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
  }

  public static partial class UsingUtils
  {
    private static class InternalHelpers
    {
      public static void PrintName(Transform target)
      {
        Debug.Log(target.name);
      }
    }

    #region Logging

    // NTBUtils.Log() will be silenced if NTBUtils.logging is false
    private static bool logging = true;

    public static void Log(object message)
    {
      if (UsingUtils.logging) Debug.Log(message);
    }

    public static void LogWarning(object message)
    {
      if (UsingUtils.logging) Debug.LogWarning(message);
    }

    public static void LogError(object message)
    {
      if (UsingUtils.logging) Debug.LogError(message);
    }

    public static string haltString<T>(this T component) where T : MonoBehaviour
    {
      return "[Component] " + component.GetType() + " in [Scene] " + SceneManager.GetActiveScene().name +
             " will not function.";
    }

    #endregion

    #region GameHelpers

    public static List<T> FindAllComponents<T>(bool IncludeOtherHierarchies = false) where T : MonoBehaviour
    {
      GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
      List<T> findList = new List<T>();
      foreach (GameObject gObj in allObjects)
      {
        // either look everywhere, or check if it's in our current hierarchy
        if (IncludeOtherHierarchies || gObj.activeInHierarchy)
        {
          T[] compsFound = gObj.GetComponents<T>();
          if (compsFound.Length > 0)
          {
            findList.AddRange(compsFound);
          }
        }
      }
      return findList;
    }

    public static bool ComponentExists<T>(bool IncludeOtherHierarchies = false) where T : MonoBehaviour
    {
      return (UsingUtils.FindAllComponents<T>(IncludeOtherHierarchies).Count > 0);
    }

    public delegate void TransformAction(Transform target);

    public static void ForRecursive(GameObject target, TransformAction action, bool inclParent = true)
    {
      if (inclParent) action(target.transform);
      UsingUtils.ForRecursiveHelper(target.transform, action);
    }

    private static void ForRecursiveHelper(Transform target, TransformAction action)
    {
      foreach (Transform child in target.transform)
      {
        action(child);
        UsingUtils.ForRecursiveHelper(child, action);
      }
    }

    // Mostly provided as an example
    public static void PrintAllChildren(GameObject target)
    {
      UsingUtils.ForRecursive(target, InternalHelpers.PrintName);
    }

    #endregion

    #region ArrayHelpers

    // Fill an array with a value
    public static void Populate<T>(this T[] arr, T val)
    {
      for (int i = 0; i < arr.Length; i++)
      {
        arr[i] = val;
      }
    }

    // Turn a 1D array into a 2D array
    public static T[,] Make2D<T>(T[] input, int height, int width)
    {
      T[,] output = new T[height, width];
      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < width; j++)
        {
          output[i, j] = input[i * width + j];
        }
      }
      return output;
    }

    public static void Print2D<T>(T[,] matrix)
    {
      string line = "";
      for (int i = 0; i < matrix.GetLength(0); i++)
      {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
          line += (matrix[i, j] + "\t");
        }
        line += "\n";
      }
      Debug.Log(line);
    }

    #endregion

    #region OtherHelpers

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
      T temp;
      temp = lhs;
      lhs = rhs;
      rhs = temp;
    }

    // Time-related nonsense
    public static string GetTimestamp(DateTime value)
    {
      return string.Format("{0:yyyy-MM-dd_hh-mm-ss}", value);
    }

    // Instance Setting helper for SingletonBehaviors
    public static bool SetInstance<T>(ref T target, T instance) where T : MonoBehaviour
    {
      // If the target is null, set it equal to the instance
      if (Object.ReferenceEquals(target, null))
      {
        target = instance;
        return true;
      }
      else if (!Object.ReferenceEquals(target, instance))
      {
        Object.Destroy(instance);
        return false;
      }
      return false;
    }

    public static string Wrap(this string text, int maxLength, bool rich = false)
    {
      if (text.Length == 0) return "";

      string[] words = text.Split(' ');
      string lines = "";
      string currentLine = "";

      foreach (var currentWord in words)
      {
        var wordCopy = currentWord;
        if (rich)
        {
          int lindex = wordCopy.IndexOf("<");
          int rindex = wordCopy.IndexOf(">");
          while (lindex != -1 && rindex != -1 && rindex - lindex > 0)
          {
            wordCopy = Regex.Replace(wordCopy, "<.*?>", string.Empty);

            lindex = wordCopy.IndexOf("<");
            rindex = wordCopy.IndexOf(">");
          }
        }

        if ((currentLine.Length > maxLength) ||
            ((currentLine.Length + wordCopy.Length) > maxLength))
        {
          lines += (currentLine + "\n");
          currentLine = "";
        }

        if (currentLine.Length > 0)
          currentLine += " " + currentWord;
        else
          currentLine += currentWord;
      }

      if (currentLine.Length > 0)
        lines += (currentLine);
      return lines;
    }

    // Usage: VAR = NUMERIC.Clamp(min, max)
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
      if (val.CompareTo(min) < 0) return min;
      else if (val.CompareTo(max) > 0) return max;
      else return val;
    }

    #endregion
  }

// Does not survive scene (un)loads!
  public class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
  {
    // Ensure that there's only one!
    public static T Instance;

    protected virtual void Awake()
    {
      // What ensures this is a Singleton.
      bool state = UsingUtils.SetInstance(ref Instance, this as T);
      if (!state)
      {
        UsingUtils.LogWarning("You attempted to create two " + typeof(T) + ".\n" + this.haltString());
      }
    }

    protected virtual void OnDestroy()
    {
      if (SingletonBehavior<T>.Instance == this)
        SingletonBehavior<T>.Instance = null; //just in case
    }
  }

// WILL survive scene (un)loads!
  public class PersistentBehavior<T> : MonoBehaviour where T : MonoBehaviour
  {
    // Ensure that there's only one!
    public static T Instance;

    protected virtual void Awake()
    {
      // What ensures this is a Singleton.
      if (UsingUtils.SetInstance(ref Instance, this as T))
        DontDestroyOnLoad(this.gameObject);
    }
  }

// A simple class that can be en masse deactivated through the NTBUtils window
  public class DebugBehavior : MonoBehaviour
  {
  }

  #region CodeHelpers

  public static class FAKEUSE
  {
    private static bool triggered = false;

    public static void NTBUnused<T>(this T val)
    {
      if (!triggered)
      {
        FAKEUSE.triggered = true;
        UsingUtils.Log("At least one unused variable warning was manually squashed." +
                     "Please... Keep that in mind.");
      }
    }
  }

  #endregion

  #region DataClasses

  public class Pair<X, Y> : object
  {
    public Pair()
    {
      this.First = default(X);
      this.Second = default(Y);
    }

    public Pair(X first, Y second)
    {
      this.First = first;
      this.Second = second;
    }

    public X First { get; set; }
    public Y Second { get; set; }

    private static readonly IEqualityComparer<X> XComparer = EqualityComparer<X>.Default;
    private static readonly IEqualityComparer<Y> YComparer = EqualityComparer<Y>.Default;

    public override int GetHashCode()
    {
      var hc = 0;
      if (!object.ReferenceEquals(First, null))
        hc = XComparer.GetHashCode(First);
      if (!object.ReferenceEquals(Second, null))
        hc = (hc << 3) ^ YComparer.GetHashCode(Second);
      return hc;
    }

    public override bool Equals(object o)
    {
      return this.Equals(o as Pair<X, Y>);
    }

    public bool Equals(Pair<X, Y> a)
    {
      return a != null && a == this;
    }

    public static bool operator ==(Pair<X, Y> a, Pair<X, Y> b)
    {
      if (object.ReferenceEquals(a, null) != object.ReferenceEquals(b, null))
      {
        return false;
      }

      if (a.First == null && b.First != null) return false;
      if (a.Second == null && b.Second != null) return false;
      return
        a.First.Equals(b.First) &&
        a.Second.Equals(b.Second);
    }

    public static bool operator !=(Pair<X, Y> a, Pair<X, Y> b)
    {
      return !(a == b);
    }
  };

  public class Ref<T>
  {
    private readonly Func<T> getter;
    private readonly Action<T> setter;

    public Ref(Func<T> getter, Action<T> setter)
    {
      this.getter = getter;
      this.setter = setter;
    }

    public T Value
    {
      get { return getter(); }
      set { setter(value); }
    }
  }

  #endregion

  public static partial class NTBUtils
  {
    public class OverrideExecOrder : Attribute
    {
      public int order;

      public OverrideExecOrder(int order)
      {
        this.order = order;
      }
    }
  }

  public static partial class NTBUtils
  {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class SetExecOrder
    {
      static SetExecOrder()
      {
#if UNITY_EDITOR
        foreach (MonoScript ms in MonoImporter.GetAllRuntimeMonoScripts())
        {
          if (ms.GetClass() != null)
          {
            foreach (Attribute a
              in Attribute.GetCustomAttributes(ms.GetClass(), typeof(OverrideExecOrder)))
            {
              var currentOrder = MonoImporter.GetExecutionOrder(ms);
              var newOrder = ((OverrideExecOrder) a).order;
              if (currentOrder != newOrder)
              {
                MonoImporter.SetExecutionOrder(ms, newOrder);
              }
            }
          }
        }
#endif
      }
    }
  }

  [Serializable]
  public class SceneString
  {
    public string Data;
    public static implicit operator string(SceneString d)
    {
        return d.Data;
    }
  };


#if UNITY_EDITOR
  public static partial class NTBUtils
  {
    //Get all editor building scenes
    public static List<string> GetBuildingScenes()
    {
      List<string> knownScenes = new List<string>();
      foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
      {
        if (S.enabled)
        {
          string name = S.path.Substring(S.path.LastIndexOf('/') + 1); //no trailing
          name = name.Substring(0, name.Length - 6); //".unity"
          knownScenes.Add(name);
        }
      }
      return knownScenes;
    }

      [CustomPropertyDrawer(typeof(SceneString))]
      public class SceneStringDrawer : PropertyDrawer
      {
          // Draw the property inside the given rect
          public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
          {
            SerializedProperty dataProp = property.FindPropertyRelative("Data");
            string currSelection = dataProp.stringValue;

            List<string> choices = NTBUtils.GetBuildingScenes();
            int panelChoice = choices.IndexOf(currSelection);
            if (panelChoice < 0)
            {
              panelChoice = 0;
            }
            panelChoice = EditorGUI.Popup(position, property.displayName, panelChoice, choices.ToArray());
            dataProp.stringValue = choices[panelChoice];
          }
      }
    }

  public static class NTBGUI
  {
    public static string SceneSelection(string currSelection, Object target = null)
    {
      List<string> choices = NTBUtils.GetBuildingScenes();

      int panelChoice = choices.IndexOf(currSelection);
      if (panelChoice < 0)
      {
        panelChoice = 0;
      }
      panelChoice = EditorGUILayout.Popup(panelChoice, choices.ToArray());
      if(!target)
        Undo.RecordObject(target, "Changed Scene Selection on " + target.name);
      return choices[panelChoice];
    }
  }

  public class NTBMenu
  {
    [MenuItem("Tools/NTBUtils/Stop all DebugBehavior(s)")]
    static void DebugBehaviorStop()
    {
      foreach (DebugBehavior dbgb in UsingUtils.FindAllComponents<DebugBehavior>())
      {
        dbgb.enabled = false;
      }
      EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/NTBUtils/Prepare for Source Control")]
    static void GIT_Prepare()
    {
      /* MOST IMPORTANT */
      {
        // We should always be seeing meta files.
        EditorSettings.externalVersionControl = "Visible Meta Files";
        // We should always serialize to text.
        EditorSettings.serializationMode = SerializationMode.ForceText;
        // We should be updating our atlas always.
        EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOn;
      }

      // Now that we've done all that, we should save our atlas.
      AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/NTBUtils/Mark All Scenes Dirty")]
    public static void MarkAllDirty()
    {
      EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("Tools/NTBUtils/Show Missing Scripts")]
    static void ShowMissingScripts()
    {
      GameObject[] allMRootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
      List<Transform> ts = new List<Transform>();
      foreach (GameObject gameObject in allMRootGameObjects)
      {
        ts.AddRange(gameObject.GetComponentsInChildren<Transform>(true));
      }
      List<GameObject> selection = new List<GameObject>();
      foreach (Transform t in ts)
      {
        // even if the components are different, this will still work for missing ones
        Component[] cs = t.gameObject.GetComponents<Component>();
        foreach (Component c in cs)
        {
          if (c == null)
          {
            selection.Add(t.gameObject);
            break;
          }
        }
      }
      Selection.objects = selection.ToArray();
    }

    [MenuItem("Tools/NTBUtils/Show Missing Materials")]
    static void ShowMissingMaterials()
    {
      Transform[] ts = Object.FindObjectsOfType<Transform>();
      List<GameObject> selection = new List<GameObject>();
      foreach (Transform t in ts)
      {
        MeshRenderer[] mrs = t.gameObject.GetComponents<MeshRenderer>();
        foreach (MeshRenderer m in mrs)
        {
          if (m != null && m.sharedMaterial == null)
          {
            selection.Add(t.gameObject);
          }
        }
      }
      Selection.objects = selection.ToArray();
    }

    [MenuItem("Tools/NTBUtils/Show All Text")]
    static void ShowMissingText()
    {
      GameObject[] allMRootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
      List<Transform> ts = new List<Transform>();
      foreach (GameObject gameObject in allMRootGameObjects)
      {
        ts.AddRange(gameObject.GetComponentsInChildren<Transform>(true));
      }
      List<GameObject> selection = new List<GameObject>();
      foreach (Transform t in ts)
      {
        // even if the components are different, this will still work for missing ones
        Text[] cs = t.gameObject.GetComponents<Text>();
        foreach (Text c in cs)
        {
          if (c != null)
          {
            //NTBUtils.Log("ping");
            selection.Add(t.gameObject);
            break;
          }
        }
      }
      Selection.objects = selection.ToArray();
    }
  }
#endif
}
