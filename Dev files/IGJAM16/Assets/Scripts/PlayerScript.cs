using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    public GameObject target;
    public float walkingSpeed;
    public bool walk;

	void Update ()
    {
        //Moves the player forward
        if(walk)
        {
            float speed = walkingSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }

        //Rotates the player towards the target
        transform.LookAt(target.transform.position);
    }
}
