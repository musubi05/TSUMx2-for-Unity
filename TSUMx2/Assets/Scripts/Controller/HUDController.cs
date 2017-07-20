﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    [SerializeField]
    public Text LabelCenter;

    [SerializeField]
    public Text LabelScore;

    [SerializeField]
    public Text LabelTime;

	// Use this for initialization
	void Start () {
        LabelCenter.text = "";
        LabelScore.text = "0";
        LabelTime.text = GameController.TimeLimitSecond.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCountdown(int time) {
        if(time < 0) {
            return;
        }
        if(time == 0) {
            LabelCenter.text = "FINISH !!";
        }
        else if(time <= 5) {
            LabelCenter.text = time.ToString();
        }
        LabelTime.text = time.ToString();
    }

    public void SetScore(int score) {
        LabelScore.text = score.ToString();
    }


}
