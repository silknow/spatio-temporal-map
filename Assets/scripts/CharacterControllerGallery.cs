using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerGallery : MonoBehaviour {

    public float speed = 3.0f;

	// Use this for initialization
	void Start () {
	    //Cursor.lockState = CursorLockMode.Locked;	
	}
	
	// Update is called once per frame
	void Update () {
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        transform.Translate(0,0, translation);
        transform.Rotate(0, straffe*1.5f, 0);

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;
	}
}
