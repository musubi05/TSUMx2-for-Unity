using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : CanvasMonoBehaviour {
    
    [SerializeField]
    private Text _labelCenter;

    [SerializeField]
    private Text _labelScore;

    [SerializeField]
    private Text _labelTime;


    void Awake () {
        AdjustCanvasScale();

        _labelCenter.text = "";
        _labelScore.text = "0";
        _labelTime.text = GameController.TimeLimitSecond.ToString();
	}

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        ScoreModel.Instance.ChangeTotalScore += () => {
            _labelScore.text = ScoreModel.Instance.TotalScore.ToString();
        };
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Display time at LabelCenter
    /// </summary>
    /// <param name="time">Time</param>
    public void DisplayLabelCenter(int time) {
        if(time < 0) {
            return;
        }
        else if(time <= 5) {
            _labelCenter.text = time.ToString();
        }
    }

    /// <summary>
    /// Change display time at LabelTime
    /// </summary>
    /// <param name="time">Time</param>
    public void ChangeTime(int time) {
        if (time >= 0) {
            _labelTime.text = time.ToString();
        }
    }

    /// <summary>
    /// Display text at LabelCenter and disappear automatically
    /// </summary>
    /// <param name="txt">Display text</param>
    /// <param name="period">The period of appearance</param>
    public IEnumerator DisplayLabelCenterPeriod(string txt, float period) {
        _labelCenter.text = txt;
        yield return new WaitForSeconds(period);
        _labelCenter.text = "";
    }
}
