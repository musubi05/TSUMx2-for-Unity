using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabModel : MonoBehaviour {

    [SerializeField]
    private GameObject[] _tsums;
    
    public Dictionary<TsumTypeId, GameObject> Tsums {
        get;
        private set;
    }
   
    // Singleton
    public static PrefabModel Instance {
        get;
        private set;
    }

    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
            return;
        }
        // Create Type Ids
        Tsums = new Dictionary<TsumTypeId, GameObject>();
        for(int i = 0; i < _tsums.Length; i++) {
            Tsums.Add(new TsumTypeId(i), _tsums[i]);
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

    public GameObject CreateTsum() {
        var ids = new List<TsumTypeId>(Tsums.Keys);
        var id = ids[Random.Range(0, ids.Count)];

        var tsum = Instantiate(Tsums[id]);

        var controller = tsum.GetComponent<TsumController>();
        controller.SetId(id);

        return tsum;
    }
}
