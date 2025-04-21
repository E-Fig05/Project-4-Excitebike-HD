using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct Racer
{
    public GameObject RacerObject;
    public float Progress;
}

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject StartLine;
    [SerializeField] private GameObject FinishLine;
    [SerializeField] private Image You;
    [SerializeField] private Image Win;
    [SerializeField] private Image Lose;
    [SerializeField] private Image Restart;

    [SerializeField] private AudioSource WinMusic;

    private bool playedWin = false;
    private int placement = 4;
    private float startToFinish;
    [SerializeField] private Racer[] racers;
    public PlacementUIController placementController;
    public TempMeter tempMeter;

    private void Start()
    {
        startToFinish = FinishLine.transform.position.x - StartLine.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        PlayerController player = racers[0].RacerObject.GetComponent<PlayerController>();

        if (player != null )
        {
            tempMeter.temp = player.Temp/player.MaxTemp;
        }

        GetRaceProgress();

        if (!player.isFinished)
        {
            placement = 1;

            for (int i = 1; i < racers.Length; i++)
            {
                if (racers[i].RacerObject.transform.position.x >= racers[0].RacerObject.transform.position.x)
                {
                    placement += 1;
                }
            }
            You.enabled = false;
            Win.enabled = false;
            Lose.enabled = false;
            Restart.enabled = false;
            placementController.placement = placement;
        }
        else
        {
            if (placement == 1)
            {
                if(playedWin == false)
                {
                    WinMusic.volume = 0.5f;
                    WinMusic.Play();
                    playedWin = true;
                }
                You.enabled = true;
                Win.enabled = true;
                Lose.enabled = false;
                Restart.enabled = true;
            }
            else
            {
                You.enabled = true;
                Lose.enabled = true;
                Win.enabled = false;
                Restart.enabled = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        for (int i = 1; i < racers.Length; i++)
        {
            if (racers[i].Progress == 1)
            {
                if (racers[i].RacerObject.GetComponent<AIController>() != null)
                {
                    racers[i].RacerObject.GetComponent<AIController>().isFinished = true;
                }
            }
        }

        //print(placement);

        if (racers[0].Progress == 1)
        {
            player.isFinished = true;
        }

        if (player.isFinished)
        {
            
        }
    }

    private void GetRaceProgress()
    {
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i].Progress = Mathf.InverseLerp(StartLine.transform.position.x, FinishLine.transform.position.x, racers[i].RacerObject.transform.position.x);
        }
    }
}
