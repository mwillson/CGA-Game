using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float yMargin = 0f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.


	public Transform player;		// Reference to the player's transform.
	public Transform target;		//the camera's target object of interest

	void Awake() {
		// Setting up the player reference.
		target = GameObject.FindGameObjectWithTag("Player").transform;
		player = target;
		//track the player from the get-go, cause intro cutscene needs this camera position
		transform.position = new Vector3(target.position.x, target.position.y + 2f, transform.position.z);

	}

	void Start ()
	{
		transform.position = new Vector3(target.position.x,target.position.y + 2f, transform.position.z);

	}


	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
	}

	public void SetYMargin(float newMargin){
		yMargin = newMargin;
	}

	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - target.position.y + 2f) > yMargin;
	}


	void FixedUpdate ()
	{
		TrackPlayer();
	}
	
	
	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = target.position.x;
		float targetY = target.position.y;

		// If the player has moved beyond the x margin...
		if(CheckXMargin())
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(transform.position.x, target.position.x, xSmooth * Time.deltaTime);

		// If the player has moved beyond the y margin...
		if(CheckYMargin())
			// ... the target y coordinate should be a Lerp between the camera's current y position and a bit above the player's current y position.
			targetY = Mathf.Lerp(transform.position.y, target.position.y + 2f, ySmooth * Time.deltaTime);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		if (player.GetComponent<PlayerController> ().FacingRight () /*&& target == player*/) {
			transform.position = new Vector3 (targetX + .2f, target.position.y+2f/*/targetY*/, transform.position.z);
		} else {
			transform.position = new Vector3 (targetX - .2f, target.position.y+2f/*targetY*/, transform.position.z);
		}
	}

	public void ScreenShake(){
		StartCoroutine (Shake ());
	}

	public IEnumerator Shake(){
		Vector3 origpos = transform.position;
		//shake for .5 seconds
		float endTime = Time.time + .5f;
		//every .05 second, add random direction shift
		while (Time.time < endTime) {
			transform.position += new Vector3(Random.Range (-.2f, .2f), Random.Range (-.2f, .2f), 0f);
			yield return new WaitForSeconds (.05f);
		}
		//return camera to original pre-shake position
		transform.position = origpos;
	}

	public void MoveTo(Vector3 newPlace){
		transform.position = newPlace;
	}
}
