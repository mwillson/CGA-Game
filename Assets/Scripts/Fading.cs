using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = .05f;

	private int drawDepth = -1000;
	public float alpha = .75f;
	private int fadeDir = -1;	//the direction to fade: in = -1, out = 1

	//public string exception;
	public Color col;

	void OnGUI () {
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01(alpha);
		//SceneManager.GetActiveScene().GetRootGameObjects().
		col = GetComponent<GUITexture>().color;
		if (SceneManager.GetActiveScene ().name == "title") {
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.depth = drawDepth;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
		} else {
			GetComponent<GUITexture> ().color = new Color (col.r, col.g, col.b, alpha);
		}
		//GameObject.Find (exception).GetComponent<Mask> ().useGUILayout = false; 
	}

	public void Update(){
		//Debug.Log ("Color: " + GameObject.Find (exception).GetComponent<Image> ().color);

	}

	public float BeginFade (int direction) {
		//GetComponent<GUITexture> ().color = Color.Lerp (GetComponent<GUITexture>().color, Color.black, 2.0f);
		fadeDir = direction;
		return fadeSpeed;
	}

	void Awake() {
		//fade in
		GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		Debug.Log("Beginning Fade");
		BeginFade (-1);
	}
}
