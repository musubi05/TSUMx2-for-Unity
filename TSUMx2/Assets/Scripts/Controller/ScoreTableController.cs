using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTableController : MonoBehaviour {

    [SerializeField]
    public GameObject BasicScoreCellPrefub;
    [SerializeField]
    public GameObject TsumScoreCellPrefub;

    private ScoreModel _score;

	// Use this for initialization
	void Start () {
        _score = ScoreModel.Instance;

        // Add Total Score Cell
        AddBasicCell("Total", _score.TotalScore.ToString());

        // Add Bonus Score Cell
        AddBasicCell("Bonus Score", _score.BonusScore.ToString());

        // Add Blank Cell
        AddBasicCell("", "");

        // Add TSUM's Score Cell
        foreach (var scoreKey in _score.TsumsScore.Keys) {
            var cell = Instantiate(TsumScoreCellPrefub);
            cell.GetComponentInChildren<Image>().sprite = scoreKey;
            cell.GetComponentInChildren<Text>().text = _score.TsumsScore[scoreKey].ToString();

            cell.transform.SetParent(this.gameObject.transform, false);
            //cell.transform.localScale = new Vector3(1f, 1f, 1f);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void AddBasicCell(string title, string value) {
        var cell = Instantiate(BasicScoreCellPrefub);
        var titleText = cell.transform.Find("Title").gameObject.GetComponent<Text>();
        var valueText = cell.transform.Find("Value").gameObject.GetComponent<Text>();
        titleText.text = title;
        valueText.text = value;
        cell.transform.SetParent(this.gameObject.transform, false);
    }
}
