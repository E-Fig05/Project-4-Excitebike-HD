using UnityEngine;
using UnityEngine.UI;

public class PlacementUIController : MonoBehaviour
{
    [SerializeField] private Sprite First;
    [SerializeField] private Sprite Second;
    [SerializeField] private Sprite Third;
    [SerializeField] private Sprite Fourth;
    [SerializeField] private Image image;
    public int placement;

    // Update is called once per frame
    void Update()
    {
        if(placement == 1)
        {
            image.sprite = First;
        }
        else if (placement == 2)
        {
            image.sprite = Second;
        }
        else if (placement == 3)
        {
            image.sprite = Third;
        }
        else if (placement == 4)
        {
            image.sprite = Fourth;
        }
    }
}
