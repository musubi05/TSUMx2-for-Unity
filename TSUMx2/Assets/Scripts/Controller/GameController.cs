using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    [SerializeField]
    private GameObject[] _tsumPrefabs;

    [SerializeField]
    public int DropTsumCnt = 55;

    [SerializeField]
    public GameObject LabelTsumTraceCnt;


    const float StartHeight = 6.0f;
    private bool _isClicked = false;

    private List<GameObject> _clickedTsums;
    private GameObject _lastClickedTsum;
    private TextMesh _txtTsumTraceCnt;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _clickedTsums = new List<GameObject>();

        StartCoroutine(DropTsum(DropTsumCnt));
        _txtTsumTraceCnt = LabelTsumTraceCnt.GetComponent<TextMesh>();
        _txtTsumTraceCnt.text = "";
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
            var tsum = Instantiate(_tsumPrefabs[Random.Range(0, _tsumPrefabs.Length)]);
            tsum.transform.position = new Vector2(
                Random.Range(-2.0f, 2.0f),
                StartHeight);
            tsum.transform.eulerAngles = new Vector3(
                0,
                0,
                Random.Range(-40, 40));

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnDragStart() {
        // Object Detection
        var clickedObject = GetClickedObject();
        if(clickedObject == null) {
            return;
        }
        // TSUM Detection
        foreach(var tsumPrefab in _tsumPrefabs) {
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
        // Dispose clicked TSUM 
        if(_clickedTsums.Count >= 3) {
            StartCoroutine(DropTsum(_clickedTsums.Count));
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
}
