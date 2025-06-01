using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

		public float speed =500;
		public float speedAc = 10;
		public Rigidbody rb;


		public GUIText countText;
		public GUIText winText;
		private int count;
		//accelerometer
		private Vector3 zeroAc;
		private Vector3 curAc;
		private float sensH = 10;
		private float sensV = 10;
		private float smooth = 0.5f;
		private float GetAxisH = 0;
		private float GetAxisV = 0;

		// Use this for initialization
		void Start () {
				rb = GetComponent<Rigidbody>();
				count = 0;
				setCountText ();
				winText.text = "";
				ResetAxes();
		}

		// Update is called once per frame
		void Update () {


		}

		// Update is called once per frame
		void FixedUpdate () {
						//get input by accelerometer
						curAc = Vector3.Lerp(curAc, Input.acceleration-zeroAc, Time.deltaTime/smooth);
						GetAxisV = Mathf.Clamp(curAc.y * sensV, -1, 1);
						GetAxisH = Mathf.Clamp(curAc.x * sensH, -1, 1);
						// now use GetAxisV and GetAxisH instead of Input.GetAxis vertical and horizontal
						// If the horizontal and vertical directions are swapped, swap curAc.y and curAc.x
						// in the above equations. If some axis is going in the wrong direction, invert the
						// signal (use -curAc.x or -curAc.y)

						Vector3 movement = new Vector3 (GetAxisH, 0.0f, GetAxisV);

						rb.AddForce(movement * speedAc);

		}

		void OnTriggerEnter(Collider other)
		{
				if (other.gameObject.tag == "Pickup") {
						other.gameObject.SetActive(false);
						count+=1;
						setCountText();
				}
		}

		void setCountText()
		{
				countText.text = "Count: " + count.ToString();
				if (count == 13) {
						winText.text = "YOU WIN!";
				}
		}


		//accelerometer
		void ResetAxes(){
				zeroAc = Input.acceleration;
				curAc = Vector3.zero;
		}

}
