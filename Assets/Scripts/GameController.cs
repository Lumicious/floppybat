using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public List<Collider2D> PipesColliders { get; private set; } = new();

    [field: SerializeField]
    public float HorizontalSpeed { get; private set; } = 1;

    [field: SerializeField]
    float TimeBetweenSpawns { get; set; } = 2;

    [field: SerializeField]
    GameObject PipesPrefab { get; set; }

    [field: SerializeField]
    Bird Bat { get; set; }

    [field: SerializeField]
    Image[] ScoreDigits { get; set; }

    [field: SerializeField]
    Sprite[] Digits { get; set; }

    [field: SerializeField]
    Image FloppyBatImage { get; set; }

    [field: SerializeField]
    Image PlayButtonImage { get; set; }

    [field: SerializeField]
    Image GameOverImage { get; set; }

    [field: SerializeField]
    Image FadeImage { get; set; }

    Vector2 FloppyBatOriginalPos { get; set; }

    List<Pipes> Pipes { get; set; } = new();

    float Score { get; set; }

    void Awake()
    {
        var score = PlayerPrefs.GetInt("score", 0);
        var digits = $"{score}".ToCharArray().Select(x => int.Parse($"{x}")).Reverse().ToList();
        foreach (var scoreDigit in ScoreDigits)
            scoreDigit.sprite = Digits[0];
        for (var i = 0; i < digits.Count; i++)
            ScoreDigits[i].sprite = Digits[digits[i]];

        Screen.SetResolution(480, 853, false);
        StartCoroutine(FadeCoroutine(true));
        FloppyBatOriginalPos = FloppyBatImage.rectTransform.localPosition;
    }

    void Update()
    {
        FloppyBatImage.rectTransform.localPosition = FloppyBatOriginalPos + new Vector2(0, Mathf.Sin(Time.time) * 50 * .66667f);
    }

    public void StartGame() => StartCoroutine(StartGameCoroutine());
    public void StopGame() => StartCoroutine(StopGameCoroutine());

    public void DestroyPipes(Pipes pipes)
    {
        Pipes.Remove(pipes);
        var colliders = pipes.GetComponentsInChildren<Collider2D>();
        PipesColliders.RemoveAll(x => colliders.Contains(x));
        Destroy(pipes.gameObject);
    }

    IEnumerator FadeCoroutine(bool fadeIn)
    {
        FadeImage.color = new Color(0, 0, 0, fadeIn ? 1 : 0);

        float t = 0f;
        while (t < .5f)
        {
            t += Time.deltaTime;

            FadeImage.color = new Color(0, 0, 0, (fadeIn ? 1 : 0) + (fadeIn ? -t / .5f : t / .5f));

            yield return null;
        }
    }

    IEnumerator StartGameCoroutine()
    {
        PlayButtonImage.GetComponent<Button>().interactable = false;
        StartCoroutine(TrackScoreCoroutine());
        StartCoroutine(SpawnPipesCoroutine());

        Bat.Flop();

        Bat.AnimationEnabled = true;
        Bat.ControlsEnabled = true;

        float t = 0f;
        while (t < .5f)
        {
            t += Time.deltaTime;
            FloppyBatImage.color = new Color(1, 1, 1, 1 - t / .5f);
            PlayButtonImage.color = new Color(1, 1, 1, 1 - t / .5f);

            yield return null;
        }

        FloppyBatImage.enabled = false;
        PlayButtonImage.enabled = false;
    }

    IEnumerator TrackScoreCoroutine()
    {
        while (HorizontalSpeed > 0) {
            Score += Time.deltaTime * 2;

            var score = (int)Score;
            var digits = $"{score}".ToCharArray().Select(x => int.Parse($"{x}")).Reverse().ToList();
            foreach (var scoreDigit in ScoreDigits)
                scoreDigit.sprite = Digits[0];
            for (var i = 0; i < digits.Count; i++)
                ScoreDigits[i].sprite = Digits[digits[i]];

            yield return null;
        }
    }

    IEnumerator SpawnPipesCoroutine(float t = 0)
    {
        while (t < TimeBetweenSpawns)
        {
            yield return null;
            t += Time.deltaTime;
        }

        var pipes = Instantiate(PipesPrefab, new Vector3(7, UnityEngine.Random.value * 7.6f - 3.8f, 0), Quaternion.identity);
        pipes.GetComponent<Pipes>().GameController = this;
        Pipes.Add(pipes.GetComponent<Pipes>());
        PipesColliders.AddRange(pipes.GetComponentsInChildren<Collider2D>());

        StartCoroutine(SpawnPipesCoroutine(t - TimeBetweenSpawns));
    }

    IEnumerator StopGameCoroutine()
    {
        var highScore = PlayerPrefs.GetInt("score", 0);
        if (Score > highScore)
        {
            PlayerPrefs.SetInt("score", (int)Score);
            PlayerPrefs.Save();
        }

        HorizontalSpeed = 0;

        Bat.AnimationEnabled = false;
        Bat.ControlsEnabled = false;

        GameOverImage.gameObject.SetActive(true);

        float t = 0f;
        while (t < .5f)
        {
            t += Time.deltaTime;

            GameOverImage.color = new Color(1, 1, 1, t / .5f);

            yield return null;
        }

        t -= .5f;

        while (t < 2)
        {
            yield return null;
            t += Time.deltaTime;
        }

        yield return FadeCoroutine(false);

        SceneManager.LoadScene("SampleScene");
    }
}
