using UnityEngine;
using UnityEngine.UI;

public class TempMeter : MonoBehaviour
{
    [SerializeField] private RectMask2D mask;
    public float temp;
    private float maskSize = 215;

    // Update is called once per frame
    void Update()
    {
        mask.padding = new Vector4(0, 0, 0, ((1 - temp) * maskSize) + 28);
    }
}
