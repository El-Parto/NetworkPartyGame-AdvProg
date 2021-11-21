using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ball = NetworkPartyGame.Physics.Ball;

public class BumperMechanic : MonoBehaviour
{
	private Ball ball;

	public void Start()
	{
		ball = FindObjectOfType<Ball>();
	}

	// Update is called once per frame
	void Update()
	{
		StartCoroutine(VisualiseKick());
	}


	private IEnumerator VisualiseKick()
	{
		gameObject.transform.localScale += new Vector3(6.5f, 0, 6.5f) * Time.deltaTime;
		yield return new WaitForSeconds(0.4f);
		Destroy(gameObject);

	}
	public void OnCollisionEnter(Collision _collision)
	{

	}
}
