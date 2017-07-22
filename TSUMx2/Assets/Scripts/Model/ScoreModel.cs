using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PrefabModel))]
public class ScoreModel : MonoBehaviour {

    public delegate void ChangeTotalScoreEventHandler();
    public delegate void AddedBonusScoreEventHandler(int score);
    public event ChangeTotalScoreEventHandler ChangeTotalScore;
    public event AddedBonusScoreEventHandler AddedBonusScore;

    // Singleton
    public static ScoreModel Instance {
        get;
        private set;
    }

    #region TotalScore
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
    #endregion

    #region BonusScore
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
    #endregion

    #region TumsScore
    public Dictionary<Sprite, int> TsumsScore {
        get;
        private set;
    }
    #endregion

    private PrefabModel _prefabModel;

    private void Awake() {
        // Singleton
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            // Reset Events
            Instance.ChangeTotalScore = null;
            Instance.AddedBonusScore = null;
            // Destroy Me
            Destroy(this.gameObject);
            return;
        }

        TsumsScore = new Dictionary<Sprite, int>();
        _prefabModel = GetComponent<PrefabModel>();

        // Initialze TSUM score list
        if (_prefabModel.Tsums.Length == 0) {
            TsumsScore.Add(null, 0);
        }
        else {
            foreach (var tsum in _prefabModel.Tsums) {
                var sprite = tsum.GetComponent<SpriteRenderer>().sprite;

                if (!TsumsScore.ContainsKey(sprite)) {
                    TsumsScore.Add(sprite, 0);
                }
            }
        }
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// nop
	}

    public void ResetScore() {
        List<Sprite> keys = new List<Sprite>(TsumsScore.Keys);
        foreach(Sprite key in keys) {
            TsumsScore[key] = 0;
        }
        TotalScore = 0;
        BonusScore = 0;
    }

    public void AddScore(Sprite tsumSprite, int deletedCnt) {
        TotalScore += deletedCnt;
        if(TsumsScore.ContainsKey(tsumSprite)) {
            TsumsScore[tsumSprite] += deletedCnt;
        }
        else {
            TsumsScore.Add(tsumSprite, deletedCnt);
        }
        // Add Bonus Score
        AddBonusScore(deletedCnt);
    }

    private void AddBonusScore(int deletedCnt) {
        int bonusScore = 0;

        if(deletedCnt <= GameController.MimTsumDelete) {
            return;
        }

        // Calc Bonus Score
        if(deletedCnt > 10) {
            bonusScore = deletedCnt * 10;
        }
        else {
            bonusScore = deletedCnt * (deletedCnt - GameController.MimTsumDelete);
        }
        // Add Score
        BonusScore += bonusScore;
        TotalScore += bonusScore;
    }
}
