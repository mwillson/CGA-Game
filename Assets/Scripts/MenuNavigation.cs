using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuNavigation : MonoBehaviour {

	//whether the axis is being held down
	public bool axisInUse = false, isActive, isMarquee;
	//useful variables for gui manipulation
	public Text currentchoice, previouschoice;

	//this represents what choice we are on
	int choiceindex;
	//ChoiceMarqueeScript marqueescript;

	// Use this for initialization
	void Start () {
		isActive = false;
		choiceindex = 0;
		currentchoice = transform.GetChild (choiceindex).gameObject.GetComponentInChildren<Text>();
		//isMarquee = currentchoice.gameObject.GetComponent<ChoiceMarqueeScript> () != null;
		StartCoroutine (WaitToActivate ());

	}
	
	// Update is called once per frame
	void Update () {
			
		//if it has a marqueescript component, it is a selection of marquee style choices
			//isMarquee = currentchoice.gameObject.GetComponent<ChoiceMarqueeScript> () != null;
			//if (isMarquee) marqueescript = currentchoice.gameObject.GetComponent<ChoiceMarqueeScript> ();

			if (!axisInUse && isActive) {
				if (Input.GetAxisRaw ("Vertical") < 0 && choiceindex < (transform.childCount - 1)) {
					choiceindex++;
					//if(isMarquee) marqueescript.ResetMarquee ();
				} else if (Input.GetAxisRaw ("Vertical") > 0 && choiceindex > 0) {
					choiceindex--;
					//if(isMarquee) marqueescript.ResetMarquee ();
				}
				
			}
			if (Input.GetAxisRaw ("Vertical") != 0) {
				axisInUse = true;
			} else {
				axisInUse = false;
			}
			
			//assign the text objects based on the index i'm at
			//remember, each "choice" is a child of 'choices' panel gameobject
			previouschoice = currentchoice;
			previouschoice.color = Color.white;
			//thought: would it be more computationally efficient to set colors in order
			//(as is currently done) to achieve the effect
			//or to do a conditional for color setting?
			currentchoice = transform.GetChild(choiceindex).gameObject.GetComponentInChildren<Text>();
		currentchoice.color = new Color(0,.7f,1);
	}

	public bool IsActive(){
		return isActive;
	}

	public IEnumerator WaitToActivate(){
		isActive = false;

		if (isMarquee) {
			//go through each marquee script and disable it until a directional is pressed
			/*foreach (ChoiceMarqueeScript mscript in GetComponentsInChildren<ChoiceMarqueeScript>()) {
				mscript.enabled = false;
			}*/
			Debug.Log ("vertical: " + Input.GetAxisRaw ("Vertical"));
			while (!(Input.GetAxisRaw ("Vertical") == 1) && !(Input.GetAxisRaw ("Vertical") == -1)
				&& !(Input.GetAxisRaw ("Horizontal") == 1) && !(Input.GetAxisRaw ("Horizontal") == -1) && !Input.GetButtonDown("Jump_Confirm") 
				&& !Input.GetButtonDown("Speed")) {
				yield return null;
			}
			Debug.Log ("Activated Marquee!");
		}
		//go staright to being active if this is not a marquee style selection
		isActive = true;
		if (isMarquee) {
			/*foreach (ChoiceMarqueeScript mscript in GetComponentsInChildren<ChoiceMarqueeScript>()) {
				mscript.enabled = true;
			}*/
		}
		yield return null;
	}

	public int GetIndex(){
		return choiceindex;
	}
}
