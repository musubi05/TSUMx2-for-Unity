using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PrefabModel))]
public class ScoreModel : MonoBehaviour {

    public class ScoreEventArgs {
        public int TotalScore {
            get;
            private set;
        }
        public int DiffScore {
            get;
            private set;
        }
        public ScoreEventArgs(int totalScore, int diffScore) {
            TotalScore = totalScore;
            DiffScore = diffScore;
        }
    }
    public delegate void ChangeTotalScoreEventHandler(ScoreEventArgs args);
    public delegate void AddedBonusScoreEventHandler(ScoreEventArgs args);
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
            var diff = value - _totalScore;
            // Call Event
            if(ChangeTotalScore != null && diff > 0) {
                ChangeTotalScore(new ScoreEventArgs(_totalScore, diff));
            }
            _totalScore = value;
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
                AddedBonusScore(new ScoreEventArgs(value, diff));
            }
            _bonusScore = value;
        }
    }
    private int _bonusScore;
    #endregion
    
    public Dictionary<TsumTypeId, int> TsumsScore {
        get;
        private set;
    }

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

        if(deletedCnt <= TSUMx2.Shared.Values.MinTsumDelete) {
            return;
        }

        // Calc Bonus Score
        if(deletedCnt > 10) {
            bonusScore = deletedCnt * 10;
        }
        else {
            bonusScore = deletedCnt * (deletedCnt - TSUMx2.Shared.Values.MinTsumDelete);
        }
        // Add Score
        BonusScore += bonusScore;
        TotalScore += bonusScore;
    }
}
