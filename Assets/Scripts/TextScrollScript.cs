using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

using M8.Animator;

public class TextScrollScript : MonoBehaviour {

	public List<string> sections;
	public string completeSection, cutscene;
	public float waitTime;
	public bool promptActive, promptEnd;
	public int currentSection;
	public GameObject promptprefab;
	private GameObject prompt;
	private GameObject instantiator;
	private bool isOneShot;

	// Use this for initialization
	public void StartScrolling () {
		waitTime = .08f;
		currentSection = 0;
		completeSection = sections [0];
		StartCoroutine (AnimateText (completeSection));
	}
	
	// Update is called once per frame
	void Update () {
		//speed up text
		if (Input.GetButtonDown ("Speed")) {
			waitTime = .02f;
			//if we are prompting for next section, go to it
			if (promptActive) {
				GoToNext();
			}
		}
		if (Input.GetButtonUp ("Speed")) {
			waitTime = .08f;
		}

		//if waiting to end a OneShot, and confirm or cancel is pressed, end it.
		/*if (promptEnd) {
			if (Input.GetButtonDown ("Jump_Confirm") || Input.GetButtonDown ("Cancel") || Input.GetButtonDown("Speed")) {
				instantiator.GetComponent<DialogueController> ().DestroyMessageObject ();
				//re-enable player control
				//but not if on pause or stat screen
				if(FindObjectOfType<PauseController>().paused != true)
					FindObjectOfType<PlayerStateController>().EnablePlayerControl();
				//if there is a cutscene to play, do it
				if (cutscene != null && cutscene != "") {
					EventManager.SceneEvent += new EventManager.CutsceneAction(Cutscene.PlayTake);
					GameObject.FindObjectOfType<EventManager>().ProcessSceneEvent(cutscene);
				}
			}
		}*/
	}

	public IEnumerator AnimateText(string completeString) {
		int i = 0;
		GetComponent<Text>().text = "";
		//while there is text left to show, keep showing it
		while( i < completeString.Length ){
			GetComponent<Text>().text += completeString[i++];
			yield return new WaitForSeconds(waitTime);
		}
		//once we've finished showing the text of this section,
		//if it's not the last section, prompt for the next one
		if (completeString != sections [sections.Count - 1]) {
			//only instantiate prompt on first one
			if (currentSection == 0) {
				prompt = (GameObject)Instantiate (promptprefab, transform.parent.position, transform.rotation);
				prompt.transform.SetParent (transform.parent);
				prompt.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-40, 20);
			}
			promptActive = true;
			prompt.SetActive (true);
		} 
		//otherwise, it IS the last section and we're done showing text
		else {
			//if this is not a single message with no choices (aka a "OneShot"), instantiate the choices 
			//if (!IsOneShot ()) {
			//	instantiator.GetComponent<DialogueController> ().InstantiateChoices ();
			//} else {
				promptEnd = true;
			//}
		}

	}

	//instantiating gameObject is a oneShot if it doesnt have a DialogueController component attached to it
	public bool IsOneShot() {
		return isOneShot;
	}

	public void SetInstantiator(GameObject go, bool ios) {
		instantiator = go;
		isOneShot = ios;
	}

	public void GoToNext(){
		//move to next section of text
		currentSection += 1;
		completeSection = sections [currentSection];
		//hide and disable prompt
		promptActive = false;
		prompt.SetActive(false);
		//start animating it
		StartCoroutine (AnimateText (completeSection));
	}

	//split up a large string into equal sized sections, except for the last one, obviously
	public void SplitSections(string completeText){
		int chunkSize = 75;
		int stringLength = completeText.Length;
		//check for base case of whether string is smaller than a chunk needs to be
		if (stringLength > chunkSize) {
			//if it isn't, then start breaking it up
			for (int i = 0; i < stringLength; i += chunkSize) {
				//if it's the last chunk, truncate it to the remaining size of the string
				if (i + chunkSize > stringLength)
					chunkSize = stringLength - i;
				//remove extra characters to end on whitespace for the chunk
				Debug.Log ("strlngth:" + stringLength);
				Debug.Log ("chuncksize:" + chunkSize);
				Debug.Log ("where in str:" + i);
				Debug.Log ("string:" + completeText.Substring (i, chunkSize));
				while (!completeText.Substring (i, chunkSize).EndsWith (" ")) {
					chunkSize -= 1;
				}
				sections.Add (completeText.Substring (i, chunkSize));

			}
		} else {
			//just add whole string,it's short enough
			sections.Add (completeText);
		}
	}
}
