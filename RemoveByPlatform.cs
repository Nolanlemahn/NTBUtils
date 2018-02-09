using UnityEngine;
using System.Collections;
using System.Linq;

public class RemoveByPlatform : MonoBehaviour
{
  public RuntimePlatform[] DeleteIf;
  // Use this for initialization
  void Start()
  {
    if (this.DeleteIf.Contains(Application.platform))
    {
      Object.Destroy(this.gameObject);
    }
  }

}