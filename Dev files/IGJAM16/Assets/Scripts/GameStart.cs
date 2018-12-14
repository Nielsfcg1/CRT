using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GameStart : MonoBehaviour {

    public float countdownTimer;
    public Text uCountdownTimer;
    public Text uGetReady;

    public GameObject player;
    PlayerScript pl;
	
	void Update ()
    {
        countdownTimer -= Time.deltaTime;
        uCountdownTimer.text = Mathf.CeilToInt(countdownTimer).ToString();

        if(countdownTimer <= 0)
        {
            GetComponent<GameManager>().enabled = true;
            player.GetComponent<PlayerScript>().walk = true;
            player.GetComponent<PlayerScript>().GetComponentInChildren<Blur>().enabled = false;
            uCountdownTimer.enabled = false;
            uGetReady.enabled = false;
            Destroy(GetComponent<GameStart>());
        }

        
	}
}
