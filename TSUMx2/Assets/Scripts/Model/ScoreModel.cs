using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreModel : MonoBehaviour {

    public delegate void ChangeTotalScoreEventHandler();
    public delegate void AddedBonusScoreEventHandler(int score);
    public event ChangeTotalScoreEventHandler ChangeTotalScore;
    public event AddedBonusScoreEventHandler AddedBonusScore;

    public int TotalScore {
        get {
            return _totalScore;
        }
        private set {
            _totalScore = value;
            // Call Event
            if(ChangeTotalScore != null) {
                ChangeTotalScore();
            }
        }
    }
    private int _totalScore;

    public int BonusScore {
        get {
            return _bonusScore;
        }
        private set {
            var diff = value - _bonusScore;
            // Call Event
            if (AddedBonusScore != null && diff > 0) {
                AddedBonusScore(diff); // 今回"追加される"ボーナス点を表示
            }
            _bonusScore = value;
        }
    }
    private int _bonusScore;


    private PrefabModel _prefabModel;
    private Dictionary<Sprite, int> _tsumsScore;

    // Use this for initialization
    void Start () {
        _tsumsScore = new Dictionary<Sprite, int>();
        _prefabModel = GetComponentInParent<PrefabModel>();
        
        // None TSUM
        if(_prefabModel.Tsums.Length == 0) {
            _tsumsScore.Add(null, 0);
        }
        else {
            foreach(var tsum in _prefabModel.Tsums) {
                var sprite = tsum.GetComponent<SpriteRenderer>().sprite;

                if (!_tsumsScore.ContainsKey(sprite)) {
                    _tsumsScore.Add(sprite, 0);
                }
            }
        }
        ResetScore();
	}
	
	// Update is called once per frame
	void Update () {
		// nop
	}

    void ResetScore() {
        List<Sprite> keys = new List<Sprite>(_tsumsScore.Keys);
        foreach(Sprite key in keys) {
            _tsumsScore[key] = 0;
        }
        TotalScore = 0;
    }

    public void AddScore(Sprite tsumSprite, int deletedCnt) {
        TotalScore += deletedCnt;
        if(_tsumsScore.ContainsKey(tsumSprite)) {
            _tsumsScore[tsumSprite] += deletedCnt;
        }
        else {
            _tsumsScore.Add(tsumSprite, deletedCnt);
        }
        // Add Bonus Score
        AddBonusScore(deletedCnt);
    }

    private void AddBonusScore(int deletedCnt) {
        int bonusScore = 0;

        if(deletedCnt <= 3) {
            return;
        }

        // Calc Bonus Score
        if(deletedCnt > 10) {
            bonusScore = deletedCnt * 10;
        }
        else {
            bonusScore = deletedCnt * (deletedCnt - 3);
        }
        // Add Score
        BonusScore += bonusScore;
        TotalScore += bonusScore;
    }
}
