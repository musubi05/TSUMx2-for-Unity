using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMesh))]
public class GameController : MonoBehaviour {

    [SerializeField]
    private PrefabModel _prefab;

    [SerializeField]
    private HUDController _hud;

    [SerializeField]
    private int _dropTsumCnt = 55;

    [SerializeField]
    private GameObject _labelTsumTraceCnt;

    [SerializeField]
    private Fade _fade;
    
    public const int MimTsumDelete = 3;
    public const int TimeLimitSecond = 60;
    private const float _dropTsumHeight = 6.0f;

    private enum State {
        NotStarted,
        Started,
        Finished
    }

    private bool _isClicked = false;
    private State _state;
    private List<GameObject> _clickedTsums;
    private GameObject _lastClickedTsum;
    private TextMesh _txtTsumTraceCnt;

    private float _time;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _clickedTsums = new List<GameObject>();

        // TSUM Drop
        StartCoroutine(DropTsum(_dropTsumCnt));

        // Initialize TraceCnt Text
        _txtTsumTraceCnt = _labelTsumTraceCnt.GetComponent<TextMesh>();
        _txtTsumTraceCnt.text = "";

        // Initialize
        _time = TimeLimitSecond;
        ScoreModel.Instance.ResetScore();

        // Start Countdown
        _state = State.NotStarted;
        StartCoroutine(GameStart(3));
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {

        if(_state != State.Started) {
            return;
        }

        _time -= Time.deltaTime;
        _hud.ChangeTime((int)_time);

        // GAME OVER
        if ((int)_time <= 0) {
            _state = State.Finished;
            StartCoroutine(ChangeResultScene());
        }
        else {
            _hud.DisplayLabelCenter((int)_time);
        }

        // TSUM Click/Drag Detection
        if (Input.GetMouseButton(0)) {
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

    /// <summary>
    /// TSUMs drop method
    /// </summary>
    /// <param name="cnt">The number of TSUMs</param>
    IEnumerator DropTsum(int cnt) {
        for(int i = 0; i < cnt; i++) {
            var tsum = Instantiate(_prefab.Tsums[Random.Range(0, _prefab.Tsums.Length)]);
            tsum.transform.position = new Vector3(
                Random.Range(-1.5f, 1.5f),
                _dropTsumHeight,
                1f
                );
            tsum.transform.eulerAngles = new Vector3(
                0,
                0,
                Random.Range(-40, 40)
                );

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// FadeIn display and start countdown
    /// </summary>
    /// <param name="cnt">Countdown seconds</param>
    private IEnumerator GameStart(int cnt) {
        yield return _fade.FadeOut(1f, 1);

        for(int i = cnt; i > 0; i--) {
            _hud.DisplayLabelCenter(i);
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(_hud.DisplayLabelCenterPeriod("START", 1f));
        _state = State.Started;
    }

    /// <summary>
    /// Drag start method
    /// </summary>
    private void OnDragStart() {
        // Object Detection
        var clickedObject = GetClickingObject();
        if(clickedObject == null) {
            return;
        }
        // TSUM Detection
        foreach(var tsumPrefab in _prefab.Tsums) {
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

    /// <summary>
    /// Drag finished method
    /// </summary>
    private void OnDragFinish() {
        // Delete clicked TSUM 
        if(_clickedTsums.Count >= MimTsumDelete) {
            StartCoroutine(DropTsum(_clickedTsums.Count));
            ScoreModel.Instance.AddScore(_clickedTsums[0].GetComponent<SpriteRenderer>().sprite, _clickedTsums.Count);

            foreach(var tsum in _clickedTsums) {
                tsum.GetComponent<TsumController>().Dispose();
            }
        }
        // else ... reset TSUM color
        else {
            foreach(var tsum in _clickedTsums) {
                ChangeTsumOpacity(tsum, 1);
            }
        }
        
        // Reset
        _isClicked = false;
        _clickedTsums.Clear();
        _lastClickedTsum = null;
        _txtTsumTraceCnt.text = "";
    }

    /// <summary>
    /// Dragging method
    /// </summary>
    private void OnDragging() {
        var clickedObject = GetClickingObject();
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

    /// <summary>
    /// Get clicking object
    /// </summary>
    /// <returns>The object which is being clicked now</returns>
    private GameObject GetClickingObject() {
        var raycastHitCollider = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).collider;
        if (raycastHitCollider == null) {
            return null;
        }
        return raycastHitCollider.gameObject;
        
    }

    /// <summary>
    /// Add clicked TSUM to clicked list
    /// </summary>
    /// <param name="tsum">clicked TSUM</param>
    private void AddClickedTsums(GameObject tsum) {
        // change transparent value
        ChangeTsumOpacity(tsum, 0.5f);
        // add list
        _clickedTsums.Add(tsum);
    }

    /// <summary>
    /// Remove TSUM from clicked list
    /// </summary>
    /// <param name="tsum">remove TSUM</param>
    private void RemoveClickedTsums(GameObject tsum) {
        if(_clickedTsums.Contains(tsum)) {
            // change transparent value
            ChangeTsumOpacity(tsum, 1);
            _clickedTsums.Remove(tsum);
        }
    }

    /// <summary>
    /// Change TSUM's opacity
    /// </summary>
    /// <param name="tsum">Target TSUM</param>
    /// <param name="a">Normalize opacity</param>
    private void ChangeTsumOpacity(GameObject tsum, float a) {
        var renderer = tsum.GetComponent<SpriteRenderer>();
        if(renderer != null) {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
        }
    } 

    /// <summary>
    /// Show the label indicating the number of tracing TSUMs
    /// </summary>
    private void ShowTraceCnt() {
        _txtTsumTraceCnt.text = _clickedTsums.Count.ToString();

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _labelTsumTraceCnt.transform.position = new Vector3(
            mousePosition.x,
            mousePosition.y,
            0
            );
    }

    /// <summary>
    /// Change result-scene
    /// </summary>
    private IEnumerator ChangeResultScene() {
        yield return _hud.DisplayLabelCenterPeriod("FINISH", 2f);
        yield return _fade.FadeIn(0.5f);

        TSUMx2.SceneManager.LoadResultScene();
    }
}
