using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumRigidBodyController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.angularVelocity = 9.8f * 10;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
