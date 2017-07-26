using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TsumController : MonoBehaviour {

    /// <summary>
    /// TSUM Type Id
    /// </summary>
    public TsumTypeId TypeId {
        get;
        private set;
    }

    public Sprite Sprite {
        get {
            return GetComponent<SpriteRenderer>().sprite;
        }
    }

    public bool IsDestroying {
        get;
        private set;
    }

    private bool _isNotSetId = true;

	// Use this for initialization
	void Start () {
        IsDestroying = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Set TSUM Type-Id
    /// </summary>
    /// <param name="typeId">Type id</param>
    /// <returns>true...Ok, false...Already set</returns>
    public bool SetId(TsumTypeId typeId) {
        if(!_isNotSetId) {
            return false;
        }

        TypeId = typeId;
        _isNotSetId = false;
        return true;
    }

    /// <summary>
    /// TSUM dispose (display effect and destroy itself)
    /// </summary>
    public void Destroy() {
        IsDestroying = true;

        var particle = GetComponentInChildren<ParticleSystem>();
        if (particle != null) {
            particle.Play();
            Destroy(this.gameObject, particle.main.duration);
        }
        else {
            Destroy(this.gameObject);
        }
    }
}
