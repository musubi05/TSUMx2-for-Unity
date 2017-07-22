using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : CanvasMonoBehaviour {

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
    /// BtnStart clicked method
    /// </summary>
    public void OnClickBtnStart() {
        _fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadGameScene();
        });
    }
}
