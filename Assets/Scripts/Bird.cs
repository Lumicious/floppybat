using System.Collections;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public bool ControlsEnabled { get; set; }

    public bool AnimationEnabled { get; set; } = true;

    [field: SerializeField]
    public float JumpPower { get; set; } = 1;

    [field: SerializeField]
    public float Gravity { get; set; } = 1;

    [field: SerializeField]
    public float RotationSpeed { get; set; } = 1;

    [field: SerializeField]
    public float FlopSpeed { get; set; } = 1;

    [field: SerializeField]
    Sprite[] BatSprites { get; set; }

    [field: SerializeField]
    SpriteRenderer SpriteRenderer { get; set; }

    [field: SerializeField]
    GameController GameController { get; set; }

    [field: SerializeField]
    CircleCollider2D Collider { get; set; }

    int SpriteIndex { get; set; }

    float FallSpeed { get; set; }

    float Rotation => (FallSpeed / JumpPower) * 45;

    void Awake()
    {
        StartCoroutine(FlopCoroutine());
    }

    public void Flop() => FallSpeed = JumpPower;

    void Update()
    {
        if (!ControlsEnabled)
            return;

        if (Input.GetMouseButtonDown(0))
            Flop();
        else
        {
            FallSpeed -= Gravity * Time.deltaTime;
            if (FallSpeed < -JumpPower)
                FallSpeed = -JumpPower;
        }

        transform.position += new Vector3(0, FallSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(transform.rotation.eulerAngles.z >= 180 ? transform.rotation.eulerAngles.z - 360 : transform.rotation.eulerAngles.z, Rotation, Time.deltaTime * RotationSpeed)));

        if (Mathf.Abs(transform.position.y) > 7)
        {
            GameController.StopGame();
            return;
        }

        foreach (var collider in GameController.PipesColliders)
            if ((collider.ClosestPoint((Vector2)transform.position + collider.offset) - ((Vector2)transform.position + Collider.offset)).magnitude < Collider.radius)
            {
                GameController.StopGame();
                break;
            }
    }

    IEnumerator FlopCoroutine()
    {
        float t = 0f;

        while (true)
        {
            while (!AnimationEnabled)
                yield return null;

            if (t >= 1 / FlopSpeed)
            {
                SpriteIndex = (SpriteIndex + 1) % BatSprites.Length;
                SpriteRenderer.sprite = BatSprites[SpriteIndex];

                t -= 1 / FlopSpeed;
            }

            t += Time.deltaTime;

            yield return null;
        }
    }
}
