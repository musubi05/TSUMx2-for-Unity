using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : CanvasMonoBehaviour {

	// Use this for initialization
	void Start () {
        AdjustCanvasScale();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickBtnStart() {
        TSUMx2.SceneManager.LoadGameScene();
    }
}
