using UnityEngine;
using System.Collections;

public class FloatingTextStatic : MonoBehaviour {

	public string text = "player";
	public int prefabIndex = 0;
	public bool parentToTransform = true;

	void Start ()
	{

		FloatingText instance = FloatingTextController.instance.CreateFloatingTextStatic(text, transform, prefabIndex).GetComponent<FloatingText>();

		if (parentToTransform)
			instance.SetParent(transform);
	}
}
