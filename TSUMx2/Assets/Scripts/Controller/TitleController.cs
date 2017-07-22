using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : CanvasMonoBehaviour {

    [SerializeField]
    public Fade Fade;

	// Use this for initialization
	void Start () {
        AdjustCanvasScale();
        Fade.FadeOut(0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickBtnStart() {
        Fade.FadeIn(0.5f, () => {
            TSUMx2.SceneManager.LoadGameScene();
        });
    }
}
