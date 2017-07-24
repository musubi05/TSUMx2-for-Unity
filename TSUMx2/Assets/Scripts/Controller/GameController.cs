using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMesh))]
public class GameController : MonoBehaviour {

    [SerializeField]
    private HUDController _hud;

    [SerializeField]
    private int _dropTsumCnt = 55;

    [SerializeField]
    private GameObject _labelTsumTraceCnt;

    [SerializeField]
    private Fade _fade;
    
    public const int MinTsumDelete = 3;
    public const int TimeLimitSecond = 10;
    private const float _dropTsumHeight = 6.0f;

    private enum State {
        WaitingPlay,
        Playing,
        Finished
    }

    private bool _isClicked = false;
    private State _state;
    private List<TsumController> _clickedTsums;
    private TextMesh _txtTsumTraceCnt;

    private float _time;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _clickedTsums = new List<TsumController>();

        // TSUM Drop
        StartCoroutine(DropTsum(_dropTsumCnt));

        // Initialize TraceCnt Text
        _txtTsumTraceCnt = _labelTsumTraceCnt.GetComponent<TextMesh>();
        _txtTsumTraceCnt.text = "";

        // Initialize
        _time = TimeLimitSecond;
        ScoreModel.Instance.ResetScore();

        // Start Countdown
        _state = State.WaitingPlay;
        StartCoroutine(GameStart(3));
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {

        if(_state != State.Playing) {
            return;
        }

        _time -= Time.deltaTime;
        _hud.ChangeTime((int)_time);

        // GAME OVER
        if ((int)_time <= 0) {
            _state = State.Finished;
            StartCoroutine(ChangeResultScene());
        }
        // Finish countdown
        else if((int)_time < 5) {
            _hud.DisplayLabelCenter(((int)_time).ToString());
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
            var tsum = PrefabModel.Instance.CreateTsum();
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
            _hud.DisplayLabelCenter(i.ToString());
            yield return new WaitForSeconds(1f);
        }
        _hud.DisplayLabelCenter("START", 1f);
        _state = State.Playing;
    }

    /// <summary>
    /// Drag start method
    /// </summary>
    private void OnDragStart() {
        // TSUM Detection
        var clickedTsum = GetClickingTsum();
        if(clickedTsum == null) {
            return;
        }

        AddClickedTsums(clickedTsum);
        _isClicked = true;
        ShowTraceCnt();
    }

    /// <summary>
    /// Drag finished method
    /// </summary>
    private void OnDragFinish() {
        // Delete clicked TSUM 
        if(_clickedTsums.Count >= MinTsumDelete) {
            StartCoroutine(DropTsum(_clickedTsums.Count));
            ScoreModel.Instance.AddScore(_clickedTsums[0].TypeId, _clickedTsums.Count);

            foreach(var tsum in _clickedTsums) {
                tsum.GetComponent<TsumController>().Destroy();
            }
        }
        // else ... reset TSUM color
        else {
            foreach(var tsum in _clickedTsums) {
                RemoveClickedTsums(tsum, true);
            }
        }
        
        // Reset
        _isClicked = false;
        _clickedTsums.Clear();
        _txtTsumTraceCnt.text = "";
    }

    /// <summary>
    /// Dragging method
    /// </summary>
    private void OnDragging() {
        var clickedTsum = GetClickingTsum();
        // not click
        if (clickedTsum == null) {
            return;
        }
        // defferent type
        if(clickedTsum.TypeId != _clickedTsums[0].TypeId) {
            return; 
        }
        // back track
        if(_clickedTsums.Count >= 2 && _clickedTsums[_clickedTsums.Count - 2] == clickedTsum) {
            RemoveClickedTsums(_clickedTsums[_clickedTsums.Count - 1]);
            ShowTraceCnt();
            return;
        }
        // already exist
        if(_clickedTsums.Contains(clickedTsum)) {
            return;
        }

        // Check to straddle another color's TSUMs
        Vector2 tsumsVector = clickedTsum.gameObject.transform.position - _clickedTsums[_clickedTsums.Count - 1].gameObject.transform.position;
        var raycastHitCollider = Physics2D.RaycastAll(
            _clickedTsums[_clickedTsums.Count - 1].gameObject.transform.position,
             tsumsVector,
             tsumsVector.magnitude
            );

        // Straddle another color's TSUMs
        if (raycastHitCollider.Length >= 3) {
            return;
        }

        AddClickedTsums(clickedTsum);
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
    /// Get clicking Tsum
    /// </summary>
    /// <returns>The tsum which is begin clicked not (null...Not TSUM)</returns>
    private TsumController GetClickingTsum() {
        var clickingObject = GetClickingObject();
        
        if(clickingObject == null) {
            return null;
        }

        return clickingObject.GetComponent<TsumController>();
    }

    /// <summary>
    /// Add clicked TSUM to clicked list
    /// </summary>
    /// <param name="tsum">clicked TSUM</param>
    private void AddClickedTsums(TsumController tsum) {
        // change transparent value
        ChangeTsumOpacity(tsum, 0.5f);
        // add list
        _clickedTsums.Add(tsum);
    }

    /// <summary>
    /// Remove TSUM from clicked list
    /// </summary>
    /// <param name="tsum">remove TSUM</param>
    /// <param name="isOnlyOpacity">Only change opacity (not remove it from list)</param>
    private void RemoveClickedTsums(TsumController tsum, bool isOnlyOpacity = false) {
        if(_clickedTsums.Contains(tsum)) {
            // Change transparent value
            ChangeTsumOpacity(tsum, 1);
            // Remove
            if (!isOnlyOpacity) {
                _clickedTsums.Remove(tsum);
            }
        }
    }

    /// <summary>
    /// Change TSUM's opacity
    /// </summary>
    /// <param name="tsum">Target TSUM</param>
    /// <param name="a">Normalize opacity</param>
    private void ChangeTsumOpacity(TsumController tsum, float a) {
        var renderer = tsum.gameObject.GetComponent<SpriteRenderer>();
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
        yield return _hud.DisplayLabelCenterWait("FINISH", 2f);
        yield return _fade.FadeIn(0.5f);

        TSUMx2.SceneManager.LoadResultScene();
    }
}
