using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this class creates and maintains a healthbar
public class HealthBarScript : MonoBehaviour {

	public int healthPerBar, numberOfBars, maxHealth, y, x;
	public Texture2D bgTex;
	public int healthUpgrades, height;
	//sprites for building background of health bar
	public Sprite baseBG, upBG, topBG, basebars, upbars;

	// Use this for initialization
	void Start () {
		height = 62;

		for (int i = 0; i < healthUpgrades; i++) {
			height += 10;
		}
		//set the size of the background texture
		bgTex = new Texture2D (20, height, TextureFormat.RGBA32, false);
		//place pixels for initial health amount, its a 20x64 sprite

		y = 0;
		while (y < 62) {
			x = 0;
			while (x < 20) {
				bgTex.SetPixel (x,y,baseBG.texture.GetPixel(x,y));
				++x;
			}
			++y;
		}
		bgTex.Apply ();
		//place pixels for each upgrade earned
		for (int i = 0; i < healthUpgrades; i++) {
			//increase max height by 10px for each upgrade
			while (y < 62 + ((i+1)*10)) {
				x = 0;
				while (x < 20) {
					//use the upBG (the background addition) texture
					bgTex.SetPixel (x,y,upBG.texture.GetPixel(x,(y-(i*10))-62));
					++x;
				}
				++y;
			}
			bgTex.Apply ();
		}

		Sprite s = Sprite.Create (bgTex, new Rect (0f, 0f, bgTex.width, bgTex.height), new Vector2 (.5f, .5f));
		transform.FindChild ("BG").GetComponent<Image> ().sprite = s;



	}

	public void FillBars(){
		//still need to dynamically create bars sprite
		transform.FindChild("Bars").GetComponent<Image>().sprite = basebars;
	}

	// Update is called once per frame
	public void UpdateHealth (int newHealth) {
		//Debug.Log ("health is: " +newHealth);
		transform.FindChild ("Bars").GetComponent<Image> ().fillAmount = (float)newHealth / 48.0f;
		//Debug.Log ("Fill amount: " + transform.FindChild ("Bars").GetComponent<Image> ().fillAmount);
	}

	public void CreateBar(int maxHealth){

	}
}
