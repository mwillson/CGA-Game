using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

using UnityEngine.SceneManagement;
using M8.Animator;

public class Cutscene: MonoBehaviour {

		//the name of the cutscene object for this cutscene. this is needed
		//for when a level is reloaded after the first time, the cutscene "hasplayed" info has to be loaded up
		public string cutSceneObjectName;
		public string takename;
		public bool playOnStart, isRecurring, hasPlayed;
		
		public string conditionClass, conditionMethod;
		public string[] conditionTakes, conditionMethodParams, triggerObjects;

		public /*virtual*/ void Start() { }
		public /*virtual*/ void SetCutSceneObject(GameObject obj) {} 
		public /*virtual*/ void Execute() {
			//play the cutscene
			AnimatorTimeline.Play (takename);
			//mark it as having played (if it does not recur)
			if (!isRecurring) {
				hasPlayed = true;
				//after cutscene plays, add its object to this level's cutscene list, so it is serialized 
				Debug.Log ("playing non-recurring cutscene and adding: " + cutSceneObjectName);
				GameManager.instance.GetCurrentLevel ().cutscenes.Add (new KeyValuePair<string,bool> (cutSceneObjectName, hasPlayed));
			}
			Exit(); 
		}
		public /*virtual*/ void Exit() {}

	//called when something hits the trigger collider
	public void OnTriggerEnter2D(Collider2D other){
		//question: should Player be a special case or should it be a potential trigger object?
		if (other.gameObject.name == "Player" && !hasPlayed) {
			CheckConditionsThenPlay ();
		} else {
			//check other possible trigger Objects for this cutscene to activate
			Debug.Log("other collision obj: " + other.gameObject.name);
			foreach(string trigger in triggerObjects){
				if (other.gameObject.name == trigger && !hasPlayed)
					CheckConditionsThenPlay ();
			}
		}
	}

	public void CheckConditionsThenPlay(){
		//if conditionclass field is something not empty, then there is a condition, so resolve it to figure out what take to use
		//otherwise, takename should be pre-set, so it will just default
		if (conditionClass != null && conditionClass != "")
			ResolveTakeCondition ();
		Debug.Log ("starting cutscene: " + takename);
		Execute ();
		hasPlayed = true;
	}

	public static void PlayTake(string t){
		AnimatorTimeline.Play(t);
	}

	//currently handles only a bool with the player actor
	//TO DO: Generalize so takes can be associated with conditions, like with a map, so any number of takes can be selected from.
	public void ResolveTakeCondition(){
		bool retVal = false;
		MethodInfo method;
		Type actorType;
		switch (conditionClass) {
		case "Player"://do something
			actorType = Type.GetType ("Player");
			method = actorType.GetMethod (conditionMethod);
			retVal = (bool)(method.Invoke (GameManager.instance.GetPlayer (), conditionMethodParams));
			break;
		default:
			break;
		}

		//this is terrible
		if (retVal == true) {
			takename = conditionTakes [0];
		} else {
			takename = conditionTakes [1];
		}
	}

}

/*public class IntroCutscene : Cutscene {

		public void SetCutSceneObject(GameObject obj){
			base.cutSceneObject = obj;
		}

		public override void Start() {
			base.cutSceneObject = gameObject;
			//QuickCutsceneController controller = base.cutSceneObject.GetComponent<QuickCutsceneController> ();

			//let cutscene controller know about player stuff to disable since it must be assigned at runtime, not beforehand in editor
			//MonoBehaviour[] disabledScripts = controller.disableWhileInCutscene;
			//disabledScripts [disabledScripts.Length - 1] = GameObject.Find ("Player").GetComponent<PlayerController> ();
			//controller.cutsceneAnimators [0] = GameObject.Find ("Player").GetComponent<Animator> ();
			//controller.cutsceneAnimatorVariables [0] = "Intro";
			//controller.cutsceneAnimatorVariableTargets [0] = true;

			base.cutSceneObject.transform.FindChild ("CameraPoint_2").position = GameObject.Find("Main Camera").transform.position;
			base.cutSceneObject.transform.FindChild ("CameraPoint_1").position = GameObject.Find("Main Camera").transform.position + new Vector3(0f, 0f, 2f); 
			//GameObject.Find ("Player").GetComponent<Animator> ().SetBool ("Intro", true);
			//disable player movement and attack controls?
			//grab player reference and move to certain point
			
			//if this is a regular level, use the regular level intro audio clip
			if (SceneManager.GetActiveScene ().name != "title" && SceneManager.GetActiveScene ().name != "menu1") {
				Debug.Log ("WE ARE PLAYING INTRO MUSIC WTF");
				//base.cutSceneObject.GetComponent<QuickCutsceneController> ().cutsceneAudio = new AudioClip[]{GameObject.Find ("introaudio").GetComponent<AudioSource> ().clip};
			}
			//base.cutSceneObject.GetComponent<QuickCutsceneController>().ActivateCutscene();
		}

		public override void Execute() {
			//bring up a dialog window where the player talks
			Debug.Log ("starting cutscene: " + cutSceneObject.name);
			
			//cutSceneObject.GetComponent<QuickCutsceneController>().ActivateCutscene();
			Exit();
		}

		public override void Exit() {
			//re-enable controls, other clean-up.
		}
}*/
