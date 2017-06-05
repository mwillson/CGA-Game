using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class DeckManipulator : MonoBehaviour {

	private InputDevice joystick;
	public List<GameObject> cardObs;
	public List<Card> cards;

	Card card;
	GameObject cardOb, loopCO;
	int newInd = 0, start = 0, end = 0, loopInd = 0;
	float dirMod = 1f;

	// Use this for initialization
	void Start () {
		joystick = InputManager.ActiveDevice;
		cards = new List<Card> ();
		foreach (Transform child in transform) {
			cards.Add (child.GetComponent<Card>());
		}
		cardObs = new List<GameObject> ();
		foreach (Transform child in transform) {
			cardObs.Add (child.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (joystick.LeftBumper.WasPressed)
			Rotate("left");
		if (joystick.RightBumper.WasPressed)
			Rotate("right");
	}

	void Rotate(string dir){

		//set up rotation parameters
		switch (dir) {
		case "left":
			start = 0;
			end = cards.Count - 1;
			loopCO = cardObs [cardObs.Count - 1];
			loopInd = 0;
			break;
		case "right":
			start = 1;
			end = cards.Count;
			loopCO = cardObs [0];
			loopInd = cards.Count - 1;
			break;
		}

		//do the rotatin for every card except 'looparound card'
		for(int i = start; i < end; i++) {
			card = cards [i];
			cardOb = cardObs [i];
			switch (dir) {
			case "left":
				Debug.Log ("rotating left");
				newInd = i + 1;
				dirMod = 1f;
				//somehow do this for this and the other thing for 'right' direction
				cards[i] = cards [newInd]; 
				cardObs[i] = cardObs[newInd];
				break;
			case "right":
				newInd = i - 1;
				dirMod = -1f;
				//decrement each card index by 1 
				Debug.Log ("rotating right");
				cards [newInd] = card;
				cardObs [newInd] = cardOb;
				break;
			}

			//move the cardobject 1 space up visually
			cardOb.transform.position += new Vector3 (.5f, .5f, .05f) * dirMod;
		}
		foreach (GameObject ob in cardObs)
			Debug.Log ("incrment step: " + ob.name);
		//loop the first or last card(depending on dir) to either the front or back of the list
		cards [loopInd] = loopCO.GetComponent<Card> ();
		cardObs [loopInd] = loopCO;
		//visually move it's object
		loopCO.transform.position += new Vector3 (.5f * (cards.Count - 1), .5f * (cards.Count - 1), .05f * (cards.Count - 1)) * -1f * dirMod;
		foreach (GameObject obj in cardObs)
			Debug.Log ("loop step: " + obj.name);
	}

	GameObject GetActiveCard(){
		return cardObs [0];
	}
}
