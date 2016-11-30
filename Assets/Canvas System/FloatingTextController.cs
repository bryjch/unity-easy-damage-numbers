﻿using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class FloatingTextController : MonoBehaviour {

	public static FloatingTextController instance = null;
	
	[Header("Text Settings")]
	public float defaultTextScale = 1.0f;
	public FloatingText.ScalingMode defaultScalingMode = FloatingText.ScalingMode.constantScale;    // This won't update if you change it in the inspector after runtime

	[Header("Instantiable Prefabs & Animations")]
	[Tooltip("These prefabs can be instantiated based on their index by calling: CreateFloatingText(...)")]
	public GameObject[] floatingTextPrefabs;
	[Tooltip("Animations are also selected when calling: CreateFloatingText(...)")]
	public AnimationClip[] animations;

	private AnimatorOverrideController animatorOverrideController;
	
	/************************************************************************************************/
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		animatorOverrideController = CreateAnimatorController();
	}

	/************************************************************************************************/
	public FloatingText CreateFloatingText(string textValue, Transform t, int prefabIndex, int animIndex)
	{
		FloatingText instance = SimplePool.Spawn(floatingTextPrefabs[prefabIndex], Vector3.zero, Quaternion.identity).GetComponent<FloatingText>();
		instance.transform.SetParent(transform);	//By default, it will be a parent of the controller gameobject
		
		if (animIndex < animations.Length)
			instance.Initialize(t, textValue, animIndex);
		else
		{
			instance.Initialize(t, textValue, 0);
			Debug.LogError("[!] Animation index invalid. Will try to play animation on index [0] instead.");
		}

		return instance;
	}

	// Method if you want to create a text object that shouldn't animate and despawn
	public FloatingText CreateFloatingTextStatic(string text, Transform t)
	{
		GameObject instance = SimplePool.Spawn(floatingTextPrefabs[0], Vector3.zero, Quaternion.identity);
		instance.GetComponent<FloatingText>().InitializeStatic(t, text);

		return instance.GetComponent<FloatingText>();
	}

	/************************************************************************************************/
	public AnimatorOverrideController CreateAnimatorController()
	{
		// The new controller that will be created based on Manager animations
		AnimatorController newController = new AnimatorController();
		newController.AddLayer("DefaultLayer");

		// Add a parameter that will determine the animation states
		AnimatorControllerParameter animatorParameter = new AnimatorControllerParameter();
		animatorParameter.type = AnimatorControllerParameterType.Int;
		animatorParameter.name = "TextAnimation";
		animatorParameter.defaultInt = 999;
		newController.AddParameter(animatorParameter);

		// Add state machine
		AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
		AnimatorStateMachine stateMachine = rootStateMachine.AddStateMachine("TextAnimationStateMachine");

		// Create a default state to prevent animation auto playing index 0
		AnimatorState waitingState = stateMachine.AddState("Waiting");

		//foreach (AnimationClip clip in DamageNumberManager.instance.animations)
		for (int i = 0; i < animations.Length; i++)
		{
			AnimationClip clip = FloatingTextController.instance.animations[i];

			// Add new state based on the AnimationClip
			AnimatorState state = stateMachine.AddState(clip.name);
			state.motion = clip;

			// Create transition from "Waiting" to the new state
			AnimatorStateTransition transition = waitingState.AddTransition(state, false);
			transition.AddCondition(AnimatorConditionMode.Equals, i, "TextAnimation");
		}

		// Override the existing Animator Controller
		AnimatorOverrideController overrideController = new AnimatorOverrideController();
		overrideController.runtimeAnimatorController = newController;

		return overrideController;
	}

	public AnimatorOverrideController GetAnimatorController()
	{
		return animatorOverrideController;
	}
}
