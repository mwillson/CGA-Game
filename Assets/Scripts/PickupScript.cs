using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//
//pickup types: 
// "Voltage"
// "Health"
//

public class PickupScript : MonoBehaviour {

	public string pickupType;
	public int amt;
	public Player player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		player = GameManager.instance.GetPlayer ();
		if (other.gameObject.name == "Player") {
			if (pickupType == "Voltage") {
				player.addVoltage (amt);
				GameObject vgauge = GameObject.Find ("Voltage Gauge");
				vgauge.transform.FindChild("Bars").GetComponent<Image>().fillAmount = (float)player.GetVoltage() / 40.0f;
			} else if (pickupType == "Health") {
				GameManager.instance.GetPlayer ().AddHealth (amt);
			}
			Destroy (gameObject);
		}
	}
}
