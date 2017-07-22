using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : CanvasMonoBehaviour {

	// Use this for initialization
	void Start () {
        AdjustCanvasScale(this.GetComponent<CanvasScaler>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
