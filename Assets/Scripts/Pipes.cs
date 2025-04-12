using UnityEngine;

public class Pipes : MonoBehaviour
{
    public GameController GameController { get; set; }

    private void Update()
    {
        transform.position += new Vector3(-GameController.HorizontalSpeed * Time.deltaTime, 0);
        if (transform.position.x < -10)
            GameController.DestroyPipes(this);
    }
}
