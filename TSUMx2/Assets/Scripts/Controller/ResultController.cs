using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : CanvasMonoBehaviour {

    [SerializeField]
    public Fade Fade;

	// Use this for initialization
	void Start () {
        AdjustCanvasScale();
        Fade.FadeOut(0.5f, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickBtnContinue() {
        Fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadGameScene();
        });
    }

    public void OnClickBtnExit() {
        Fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadTitleScene();
        });
    }
}
