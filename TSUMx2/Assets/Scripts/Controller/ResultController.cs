using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : CanvasMonoBehaviour {

    [SerializeField]
    private Fade _fade;

	// Use this for initialization
	void Start () {
        AdjustCanvasScale();
        _fade.FadeOut(0.5f, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// BtnContinue clicked method
    /// </summary>
    public void OnClickBtnContinue() {
        _fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadGameScene();
        });
    }

    /// <summary>
    /// BtnExit clicked method
    /// </summary>
    public void OnClickBtnExit() {
        _fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadTitleScene();
        });
    }
}
