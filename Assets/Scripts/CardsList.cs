using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListView;
using InControl;

public class CardsList : ListViewController<CardItemData, Card>  {

	private InputDevice joystick;

	protected override void Setup(){
		base.Setup ();
		joystick = InputManager.ActiveDevice;
	}

	void Update(){
		base.ViewUpdate ();
		if (joystick.LeftBumper.WasPressed) {
			Debug.Log ("left");
			ScrollPrev ();
		}
		if (joystick.RightBumper.WasPressed){
			Debug.Log ("right");
			ScrollNext ();
		}
	}
}
