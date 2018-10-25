using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSTextController : MonoBehaviour
{
  public Text Display;
  public int Polling = 0;
  private int polled = 0;

	void LateUpdate()
	{
    //this.startReportNTBU();
	  if (this.polled >= this.Polling)
	  {
	    this.polled = 0;
	    this.Display.text = Convert.ToString(1.0f / Time.deltaTime);
	    return;
	  }
	  this.polled++;
    //this.endReportNTBU();
	}
}
