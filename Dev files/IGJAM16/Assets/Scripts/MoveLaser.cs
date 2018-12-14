using UnityEngine;
using System.Collections;

public class MoveLaser : MonoBehaviour {

    Vector3 target;
    GameManager gm;
    public float movementSpeed;
	
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().gameObject.transform.position;
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

	void Update ()
    {
        float speed = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, speed);

        //transform.LookAt(target);
    }

    void OnTriggerEnter(Collider colInfo)
    {
        if (colInfo.tag == "Player")
        {
            Debug.Log("Hits the player");

            if(!gm.correctMask)
            {
                gm.WrongMask();
            }

            Destroy(gameObject);
        }
    }
}
