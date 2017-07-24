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

    [SerializeField]
    private Text _labelBonus;

    private IEnumerator _labelBonusCoroutine;


    void Awake () {
        AdjustCanvasScale();

        _labelCenter.text = "";
        _labelScore.text = "0";
        _labelTime.text = TSUMx2.Shared.Values.GameOverSeconds.ToString();

        _labelBonus.gameObject.SetActive(false);
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        ScoreModel.Instance.ChangeTotalScore += (args) => {
            _labelScore.text = ScoreModel.Instance.TotalScore.ToString();
        };
        ScoreModel.Instance.AddedBonusScore += (args) => {
            if (_labelBonusCoroutine != null) {
                StopCoroutine(_labelBonusCoroutine);
                _labelBonusCoroutine = null;
            }
            _labelBonusCoroutine = DisplayLabelBonus(1f);
            StartCoroutine(_labelBonusCoroutine);
        };
    }
	
	// Update is called once per frame
	void Update () {
	}

    /// <summary>
    /// Display time at LabelCenter
    /// </summary>
    /// <param name="time">Time</param>
    /// 
    public void DisplayLabelCenter(string txt) {
        DisplayLabelCenter(txt, -1);
    }

    /// <summary>
    /// Display text at LabelCenter and disappear automatically
    /// </summary>
    /// <param name="txt">Display text</param>
    /// <param name="period">The period of appearance</param>
    public void DisplayLabelCenter(string txt, float period) {
        _labelCenter.text = txt;
        if(period >= 0) {
            StartCoroutine(DisappearLabelCenterPeriod(period));
        }
    }

    /// <summary>
    /// Display text at LabelCenter and disappear automatically (Wait to disappear)
    /// </summary>
    /// <param name="txt">Display text</param>
    /// <param name="period">The period of appearance</param>
    public IEnumerator DisplayLabelCenterWait(string txt, float period) {
        _labelCenter.text = txt;
        yield return StartCoroutine(DisappearLabelCenterPeriod(period));
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
    /// Disappear labelCenter
    /// </summary>
    /// <param name="period">The period of appearance</param>
    private IEnumerator DisappearLabelCenterPeriod(float period) {
        yield return new WaitForSeconds(period);
        _labelCenter.text = "";
    }

    /// <summary>
    /// Display LabelBonus with frame in/out animation
    /// </summary>
    /// <param name="time">The time of appearance</param>
    private IEnumerator DisplayLabelBonus(float time) {
        float fadeTime = time / 10;
        _labelBonus.gameObject.SetActive(true);

        // Frame in
        for(float t = 0; t < fadeTime; t += Time.deltaTime) {
            _labelBonus.transform.localScale = new Vector3(t / fadeTime, t / fadeTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Wait
        yield return new WaitForSeconds(time - fadeTime * 2);

        // Frame out
        for(float t = fadeTime; t >= 0; t -= Time.deltaTime) {
            _labelBonus.transform.localScale = new Vector3(t / fadeTime, t / fadeTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _labelBonus.gameObject.SetActive(false);
    }
}
