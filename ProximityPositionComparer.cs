using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ProximityPositionComparer : IComparer
{
  private Transform center;
  public ProximityPositionComparer(Transform center)
  {
    this.center = center;
  }

  public int Compare(object x, object y)
  {
    GameObject xobj = (GameObject)x;
    GameObject yobj = (GameObject)y;

    float distx = Vector3.Distance(this.center.transform.position,
      xobj.transform.position);
    float disty = Vector3.Distance(this.center.transform.position,
      yobj.transform.position);

    if (distx < disty) return -1;
    if (distx > disty) return 1;
    else return 0; //yeah, right.
  }
}