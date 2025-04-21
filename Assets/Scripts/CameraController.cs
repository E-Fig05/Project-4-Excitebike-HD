using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Vector3 offset = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 angleOffset = new Vector3(25, 0, 0);

    private void Update()
    {
        transform.position = playerRB.worldCenterOfMass;
        cam.transform.position = new Vector3 (player.transform.position.x + offset.x, player.transform.position.y + offset.y,offset.z);
        cam.transform.localEulerAngles = angleOffset;
    }
}
