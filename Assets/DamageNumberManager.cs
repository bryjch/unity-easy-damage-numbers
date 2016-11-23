using UnityEngine;
using System.Collections;

public class DamageNumberManager : MonoBehaviour {

	public static DamageNumberManager instance = null;

	public AnimationClip[] animations;

	// Use this for initialization
	void Start ()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
