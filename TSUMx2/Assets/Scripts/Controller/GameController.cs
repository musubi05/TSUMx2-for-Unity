﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [SerializeField]
    public PrefabModel Prefab;

    [SerializeField]
    public HUDController HUD;

    [SerializeField]
    public int DropTsumCnt = 55;

    [SerializeField]
    public GameObject LabelTsumTraceCnt;

    public const int MimTsumDelete = 3;
    public const int TimeLimitSecond = 60;
    public const float DropTsumHeight = 6.0f;

    private bool _isClicked = false;
    private bool _isGameFinished = false;
    private List<GameObject> _clickedTsums;
    private GameObject _lastClickedTsum;
    private TextMesh _txtTsumTraceCnt;

    private float time;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _clickedTsums = new List<GameObject>();

        StartCoroutine(DropTsum(DropTsumCnt));
        _txtTsumTraceCnt = LabelTsumTraceCnt.GetComponent<TextMesh>();
        _txtTsumTraceCnt.text = "";

        time = TimeLimitSecond;
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        time -= Time.deltaTime;
        // GAME OVER
        if((int)time <= 0) {
            _isGameFinished = true;
            StartCoroutine(ChangeResultScene());
        }
        else {
            HUD.SetCountdown((int)time);
        }

        // TSUM Click/Drag Detection
        if (_isGameFinished == false && Input.GetMouseButton(0)) {
            if (_isClicked == false) {
                OnDragStart();
            }
            else {
                OnDragging();
            }
        }
        else if(_isGameFinished == false && Input.GetMouseButtonUp(0) && _isClicked == true) {
            OnDragFinish();
        }
	}
    /// <summary>
    /// TSUMs Drop Method (
    /// </summary>
    /// <param name="cnt">The number of tsum</param>
    IEnumerator DropTsum(int cnt) {
        for(int i = 0; i < cnt; i++) {
            var tsum = Instantiate(Prefab.Tsums[Random.Range(0, Prefab.Tsums.Length)]);
            tsum.transform.position = new Vector3(
                Random.Range(-2.0f, 2.0f),
                DropTsumHeight,
                1f
                );
            tsum.transform.eulerAngles = new Vector3(
                0,
                0,
                Random.Range(-40, 40));

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// Drag Start Method
    /// </summary>
    private void OnDragStart() {
        // Object Detection
        var clickedObject = GetClickedObject();
        if(clickedObject == null) {
            return;
        }
        // TSUM Detection
        foreach(var tsumPrefab in Prefab.Tsums) {
            // Detected!!
            if(clickedObject.name.Contains(tsumPrefab.name)) {
                AddClickedTsums(clickedObject);
                _lastClickedTsum = clickedObject;
                _isClicked = true;
                ShowTraceCnt();
                return;
            }
        }
    }

    private void OnDragFinish() {
        // Delete clicked TSUM 
        if(_clickedTsums.Count >= MimTsumDelete) {
            StartCoroutine(DropTsum(_clickedTsums.Count));
            ScoreModel.Instance.AddScore(_clickedTsums[0].GetComponent<SpriteRenderer>().sprite, _clickedTsums.Count);

            foreach(var tsum in _clickedTsums) {
                //Destroy(tsum);
                tsum.GetComponent<TsumController>().Dispose();
            }
        }
        // else ... reset TSUM color
        else {
            foreach(var tsum in _clickedTsums) {
                ChangeTsumOpacity(tsum, 1);
            }
        }
        
        _isClicked = false;
        _clickedTsums.Clear();
        _lastClickedTsum = null;
        _txtTsumTraceCnt.text = "";
    }

    private void OnDragging() {
        var clickedObject = GetClickedObject();
        // not click
        if (clickedObject == null) {
            return;
        }
        // not tsum
        if (clickedObject.GetComponent<SpriteRenderer>() == null) {
            return;
        }
         // defferent type
        var clickedSprite = _clickedTsums[0].GetComponent<SpriteRenderer>().sprite;
        if(clickedObject.GetComponent<SpriteRenderer>().sprite != clickedSprite) {
            return; 
        }
        // back track
        if(_clickedTsums.Count >= 2 && _clickedTsums[_clickedTsums.Count - 2] == clickedObject) {
            RemoveClickedTsums(_lastClickedTsum);
            _lastClickedTsum = _clickedTsums[_clickedTsums.Count - 1];
            ShowTraceCnt();
            return;
        }
        // already exist
        if(_clickedTsums.Contains(clickedObject)) {
            return;
        }

        // Check to straddle another color's TSUMs
        Vector2 tsumsVector = clickedObject.transform.position - _lastClickedTsum.transform.position;
        var raycastHitCollider = Physics2D.RaycastAll(
            _lastClickedTsum.transform.position,
             tsumsVector,
             tsumsVector.magnitude
            );

        // Straddle another color's TSUMs
        if (raycastHitCollider.Length >= 3) {
            return;
        }

        AddClickedTsums(clickedObject);
        _lastClickedTsum = clickedObject;
        ShowTraceCnt();
    }

    private GameObject GetClickedObject() {
        var raycastHitCollider = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).collider;
        if (raycastHitCollider == null) {
            return null;
        }
        return raycastHitCollider.gameObject;
        
    }
    private void AddClickedTsums(GameObject tsum) {
        // change transparent value
        ChangeTsumOpacity(tsum, 0.5f);
        // add list
        _clickedTsums.Add(tsum);
    }

    private void RemoveClickedTsums(GameObject tsum) {
        if(_clickedTsums.Contains(tsum)) {
            // change transparent value
            ChangeTsumOpacity(tsum, 1);
            _clickedTsums.Remove(tsum);
        }
    }

    private void ChangeTsumOpacity(GameObject tsum, float a) {
        var renderer = tsum.GetComponent<SpriteRenderer>();
        if(renderer != null) {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
        }
    } 
    private void ShowTraceCnt() {
        _txtTsumTraceCnt.text = _clickedTsums.Count.ToString();

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LabelTsumTraceCnt.transform.position = new Vector3(
            mousePosition.x,
            mousePosition.y,
            0
            );
    }

    private IEnumerator ChangeResultScene() {
        yield return HUD.DisplayLabelCenterPeriod("FINISH", 3f);
        SceneManager.LoadSceneAsync("result");
    }
}
