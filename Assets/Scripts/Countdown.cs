using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject AI1;
    [SerializeField] private GameObject AI2;
    [SerializeField] private GameObject AI3;
    [SerializeField] private Sprite[] countdownSprites;
    [SerializeField] private Image countdownUI;
    private float startTimer = 3;
    private bool started = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.GetComponent<PlayerController>().enabled = false;
        AI1.GetComponent<AIController>().enabled = false;
        AI2.GetComponent<AIController>().enabled = false;
        AI3.GetComponent<AIController>().enabled = false;
        countdownUI.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        startTimer -= Time.deltaTime;
        if (startTimer <= 0)
        {
            if (!started)
            {
                Player.GetComponent<PlayerController>().enabled = true;
                AI1.GetComponent<AIController>().enabled = true;
                AI2.GetComponent<AIController>().enabled = true;
                AI3.GetComponent<AIController>().enabled = true;
                started = true;
            }
        }
        if (startTimer > -1)
        {
            countdownUI.sprite = countdownSprites[Mathf.CeilToInt(startTimer)];
        }
        else
        {
            countdownUI.enabled = false;
        }
    }
}
