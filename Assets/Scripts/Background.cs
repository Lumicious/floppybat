using UnityEngine;

public class Background : MonoBehaviour
{
    [field: SerializeField]
    GameController GameController { get; set; }

    void Update()
    {
        transform.position += new Vector3(-Time.deltaTime * GameController.HorizontalSpeed / 2, 0, 0);
        while (transform.position.x < -10)
            transform.position += new Vector3(10, 0, 0);
    }
}
