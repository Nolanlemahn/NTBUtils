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
using UnityEngine.UI;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerCanvasLink : MonoBehaviour
{
  public bool ForcePlay = false;
  public VideoClip clip = null;

  void Start()
  {
    if (this.ForcePlay)
    {
      this.Activate();
    }
  }

  public void Activate(VideoClip vc = null)
  {
    VideoPlayer vp = this.GetComponent<VideoPlayer>();

    // if we got a param, try to use it
    if (vc != null) vp.clip = vc;
    // if the player has no clip and we got no param, use our property
    if (vp.clip == null) vp.clip = this.clip;

    // Read info from the video file
    uint w = vp.clip.width;
    uint h = vp.clip.height;

    // Use that to make the texture
    RenderTexture rt = new RenderTexture((int)w, (int)h, 16, RenderTextureFormat.ARGB32);
    rt.Create();

    vp.targetTexture = rt;
    this.GetComponent<RawImage>().texture = rt;
    vp.Play();
  }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VideoPlayerCanvasLink))]
[CanEditMultipleObjects]
class VPCGUI : Editor
{
  public override void OnInspectorGUI()
  {
    this.DrawDefaultInspector();
    if (GUILayout.Button("Force Invoke"))
    {
      ((VideoPlayerCanvasLink)target).Activate();
    }
  }
}
#endif