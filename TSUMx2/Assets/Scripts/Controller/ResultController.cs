﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : CanvasMonoBehaviour {

	// Use this for initialization
	void Start () {
        AdjustCanvasScale();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickBtnContinue() {
        TSUMx2.SceneManager.LoadGameScene();
    }

    public void OnClickBtnExit() {
        TSUMx2.SceneManager.LoadTitleScene();
    }
}
