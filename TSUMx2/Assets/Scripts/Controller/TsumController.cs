﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Dispose() {
        var particle = GetComponentInChildren<ParticleSystem>();
        if (particle != null) {
            particle.Play();
            Destroy(this.gameObject, particle.main.duration);
        }
    }
}
