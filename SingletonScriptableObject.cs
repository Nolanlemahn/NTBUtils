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

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;
    public static string PathForNonpreloaded = ""; //Hide this in inheritance if desired

    public static T Get()
    {
        if (!_instance)
        {
            if (Application.isEditor)
            {
                #if UNITY_EDITOR
                // preloadeds aren't necessarily preloaded in editor
                var objIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name);
                _instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(objIDs[0]));
                return _instance;
                #endif
            }
            else
            {
                T[] objs = Resources.FindObjectsOfTypeAll<T>();
                if (objs.Length == 0)
                {
                    _instance = Resources.Load<T>(PathForNonpreloaded);
                    return _instance;
                }
                else
                {
                    return objs[0];
                }
            }
        }
        return _instance;
    }
}