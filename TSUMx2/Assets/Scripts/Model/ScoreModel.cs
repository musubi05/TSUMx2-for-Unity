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
    public Dictionary<TsumTypeId, int> TsumsScore {
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

        TsumsScore = new Dictionary<TsumTypeId, int>();
        _prefabModel = GetComponent<PrefabModel>();
    }

    // Use this for initialization
    void Start () {
        // Initialze TSUM score list
        if (_prefabModel == null) {
            return;
        }
        foreach(var id in _prefabModel.Tsums.Keys) {
            if(!TsumsScore.ContainsKey(id)) {
                TsumsScore.Add(id, 0);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		// nop
	}

    /// <summary>
    /// Reset all score
    /// </summary>
    public void ResetScore() {
        var keys = new List<TsumTypeId>(TsumsScore.Keys);
        foreach(var key in keys) {
            TsumsScore[key] = 0;
        }
        TotalScore = 0;
        BonusScore = 0;
    }

    /// <summary>
    /// Add score
    /// </summary>
    /// <param name="tsumId">deleted TSUM's type-id</param>
    /// <param name="deletedCnt">The number of deleted TSUMs</param>
    public void AddScore(TsumTypeId tsumId, int deletedCnt) {
        TotalScore += deletedCnt;
        if(TsumsScore.ContainsKey(tsumId)) {
            TsumsScore[tsumId] += deletedCnt;
        }
        else {
            TsumsScore.Add(tsumId, deletedCnt);
        }
        // Add Bonus Score
        AddBonusScore(deletedCnt);
    }

    /// <summary>
    /// Add bonus score
    /// </summary>
    /// <param name="deletedCnt">The number of deleted TSUMs</param>
    private void AddBonusScore(int deletedCnt) {
        int bonusScore = 0;

        if(deletedCnt <= GameController.MinTsumDelete) {
            return;
        }

        // Calc Bonus Score
        if(deletedCnt > 10) {
            bonusScore = deletedCnt * 10;
        }
        else {
            bonusScore = deletedCnt * (deletedCnt - GameController.MinTsumDelete);
        }
        // Add Score
        BonusScore += bonusScore;
        TotalScore += bonusScore;
    }
}
