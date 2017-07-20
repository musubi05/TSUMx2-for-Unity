using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumController : MonoBehaviour {

    [SerializeField]
    private GameObject _tsumPrefab;

    [SerializeField]
    private Sprite[] _tsumSprites;

    const float StartHeight = 7.0f;

    [SerializeField]
    public int DropTsumCnt = 55;

	// Use this for initialization
	void Start () {
        StartCoroutine(DropTsum(DropTsumCnt));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

     IEnumerator DropTsum(int cnt) {
        for(int i = 0; i < cnt; i++) {
            var tsum = Instantiate(_tsumPrefab);
            tsum.transform.position = new Vector3(
                Random.Range(-2.0f, 2.0f),
                StartHeight,
                Random.Range(-40, 40));

            var tsumTexture = tsum.GetComponent<SpriteRenderer>();
            tsumTexture.sprite = _tsumSprites[Random.Range(0, _tsumSprites.Length)];

            yield return new WaitForSeconds(0.05f);
        }
    }
}
