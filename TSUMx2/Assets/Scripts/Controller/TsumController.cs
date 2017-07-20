using System.Collections;
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
        //StartCoroutine(PlayParticleAndDestroy());
        var particle = GetComponentInChildren<ParticleSystem>();
        if (particle != null) {
            particle.Play();
            Destroy(this.gameObject, particle.main.duration);
        }
    }

    IEnumerator PlayParticleAndDestroy() {
        var particle = GetComponentInChildren<ParticleSystem>();
        if (particle != null) {
            particle.Play();
            while(particle.isPlaying) {
                yield return new WaitForSeconds(0.001f);
            }
        }
        
        DestroyImmediate(this.gameObject);
    }
}
