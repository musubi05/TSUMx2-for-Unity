﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumController : MonoBehaviour {

    [SerializeField]
    private GameObject _tsumPrefab;

    [SerializeField]
    private Sprite[] _tsumSprites;

    [SerializeField]
    public int DropTsumCnt = 55;


    const float StartHeight = 6.0f;
    private bool _isClicked = false;

    private List<GameObject> _clickedTsums;
    private GameObject _lastClickedTsum;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _clickedTsums = new List<GameObject>();

        StartCoroutine(DropTsum(DropTsumCnt));
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
		if(Input.GetMouseButton(0)) {
            if (_isClicked == false) {
                OnDragStart();
            }
            else {
                OnDragging();
            }
        }
        else if(Input.GetMouseButtonUp(0) && _isClicked == true) {
            OnDragFinish();
        }
	}

    IEnumerator DropTsum(int cnt) {
        for(int i = 0; i < cnt; i++) {
            var tsum = Instantiate(_tsumPrefab);
            tsum.transform.position = new Vector2(
                Random.Range(-2.0f, 2.0f),
                StartHeight);
            tsum.transform.eulerAngles = new Vector3(
                0,
                0,
                Random.Range(-40, 40));

            var tsumTexture = tsum.GetComponent<SpriteRenderer>();
            tsumTexture.sprite = _tsumSprites[Random.Range(0, _tsumSprites.Length)];

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnDragStart() {
        // Tsum Detection
        var clickedObject = GetClickedObject();
        if(clickedObject == null) {
            return;
        }

        if (!clickedObject.name.Contains(_tsumPrefab.name)) {
            return;
        }
        _clickedTsums.Clear();
        AddClickedTsums(clickedObject);
        _lastClickedTsum = clickedObject;

        _isClicked = true;
    }

    private void OnDragFinish() {
        // Dispose clicked TSUM 
        if(_clickedTsums.Count >= 3) {
            StartCoroutine(DropTsum(_clickedTsums.Count));
            foreach(var tsum in _clickedTsums) {
                DestroyImmediate(tsum);
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
}
