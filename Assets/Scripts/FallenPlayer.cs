using UnityEngine;

public class FallenPlayer : MonoBehaviour
{
    public Sprite run0;
    public Sprite run1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int currentFrame = 0;

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % 30 == 0)
        {
            currentFrame = (currentFrame + 1) % 2;
        }

        if(currentFrame == 0)
        {
            spriteRenderer.sprite = run0;
        }
        else
        {
            spriteRenderer.sprite = run1;
        }
    }
}
