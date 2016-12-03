using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(FloatingTextController))]
public class AnimatorEditor : Editor
{
	public string controllerAssetPath = "Assets/";

	private FloatingTextController myScript;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		myScript = (FloatingTextController)target;

		if (GUILayout.Button("Update Animator"))
		{
			// Generate a new AOC based on the animation in FloatingTextController
			AnimatorOverrideController updatedAOC = CreateAnimatorOverrideController();
			
			// Look for an existing asset path on FloatingTextController (returns null if none found)
			string assetPath = AssetDatabase.GetAssetPath(myScript.animatorOverrideController);

			// Create or replace AOC depending on whether assetPath was found
			Object updatedAOC_obj = CreateOrReplaceAsset(updatedAOC, assetPath);

			myScript.animatorOverrideController = (AnimatorOverrideController) updatedAOC_obj;
		}
	}
	
	public Object CreateOrReplaceAsset(Object asset, string path)
	{
		Object existingAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
	
		if (existingAsset == null)
		{
			// Create a new object in the main asset directory my default
			AssetDatabase.CreateAsset(asset, controllerAssetPath + "FTOverrideController.overrideController");
			existingAsset = asset;
			Debug.LogError("Animator Override Controller not found. Creating new one in " + controllerAssetPath + " directory.");
		}
		else
		{
			// Replace the existing copy with <asset> data
			EditorUtility.CopySerialized(asset, existingAsset);
			Debug.Log("Overriding existing Animator Override Controller found at " + path + ".");
		}
		return existingAsset;
	}

	// Create a new AOC based on animations[], including default state transitions
	public AnimatorOverrideController CreateAnimatorOverrideController()
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
		for (int i = 0; i < myScript.animations.Length; i++)
		{
			AnimationClip clip = myScript.animations[i];

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

		overrideController.name = "FloatingTextAnimatorController";
		//animator = overrideController;

		return overrideController;
	}
}