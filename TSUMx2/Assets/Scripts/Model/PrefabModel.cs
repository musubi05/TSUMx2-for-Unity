using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabModel : MonoBehaviour {

    [SerializeField]
    private GameObject[] _tsums;
    
    public GameObject[] Tsums {
        get {
            return _tsums;
        }
    }

    // Use this for initialization
    void Start () {
		//nop
	}
	
	// Update is called once per frame
	void Update () {
		//nop
	}
}
