using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceGameController : MonoBehaviour
{
    [Header("Префаб и спавн")]
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private Transform diceParent;
    [SerializeField] private int defaultDiceCount = 2;
    [SerializeField] private float spawnRadius = 3f;

    [Header("Бросок")]
    [SerializeField] private float minForce = 9f;
    [SerializeField] private float maxForce = 12f;
    [SerializeField] private float minTorque = 1f;
    [SerializeField] private float maxTorque = 3f;
    [SerializeField] private float sideSpread = 2f;

    [Header("UI")]
    [SerializeField] private DiceUIView ui;

    private readonly List<Dice> dice = new List<Dice>();
    private int diceCount;
    private int winThreshold;
    private int drawThreshold;
    private int loseThreshold;
    private bool waitingForSettle;

    public int DiceCount => diceCount;
    public int WinThreshold => winThreshold;
    public int DrawThreshold => drawThreshold;
    public int LoseThreshold => loseThreshold;

    private void Awake()
    {
        diceCount = defaultDiceCount;
        SpawnDice(diceCount);
        RollNewThresholds();
        ui.Initialize(this);
    }

    public void OnRollButton()
    {
        if (waitingForSettle) return;

        diceCount = ui.GetDiceCountOrDefault(diceCount);

        if (!ui.TryGetThresholds(out winThreshold, out drawThreshold, out loseThreshold))
            RollNewThresholds();

        RespawnDice(diceCount);
        ui.SetDiceCount(diceCount);
        ui.SetThresholds(winThreshold, drawThreshold, loseThreshold);
        ThrowAll();
    }

    private void SpawnDice(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0.5f,
                Random.Range(-spawnRadius, spawnRadius));

            Dice d = Instantiate(dicePrefab, pos, Quaternion.identity, diceParent)
                .GetComponent<Dice>();
            dice.Add(d);
        }
    }

    private void RespawnDice(int count)
    {
        foreach (var d in dice)
            Destroy(d.gameObject);
        dice.Clear();
        SpawnDice(count);
    }

    private void RollNewThresholds()
    {
        int minSum = diceCount;
        int maxSum = diceCount * 6;
        loseThreshold = minSum;
        drawThreshold = Random.Range(minSum + 1, maxSum);
        winThreshold = Random.Range(drawThreshold + 1, maxSum + 1);
    }

    private void ThrowAll()
    {
        waitingForSettle = true;
        ui.SetResult("Бросок...");
        ui.SetOutcome("");

        foreach (var d in dice)
        {
            Vector3 force = Vector3.up * Random.Range(minForce, maxForce);
            force += new Vector3(
                Random.Range(-sideSpread, sideSpread),
                0f,
                Random.Range(-sideSpread, sideSpread));

            Vector3 torque = new Vector3(
                RandomSign() * Random.Range(minTorque, maxTorque),
                RandomSign() * Random.Range(minTorque, maxTorque),
                RandomSign() * Random.Range(minTorque, maxTorque));

            d.Throw(force, torque);
        }

        StartCoroutine(WaitAndScore());
    }

    private IEnumerator WaitAndScore()
    {
        yield return new WaitForSeconds(0.4f);

        while (true)
        {
            bool allSettled = true;
            foreach (var d in dice)
            {
                if (!d.IsSettled)
                {
                    allSettled = false;
                    break;
                }
            }
            if (allSettled) break;
            yield return null;
        }

        int sum = 0;
        var values = new List<int>();
        foreach (var d in dice)
        {
            int v = d.GetTopFaceValue();
            values.Add(v);
            sum += v;
        }

        ui.SetResult($"Выпало: {string.Join(" + ", values)} = {sum}");

        if (sum >= winThreshold) ui.SetOutcome("Итог: Победа!");
        else if (sum >= drawThreshold) ui.SetOutcome("Итог: Ничья");
        else ui.SetOutcome("Итог: Поражение");

        waitingForSettle = false;
    }

    private float RandomSign()
    {
        return Random.value < 0.5f ? -1f : 1f;
    }
}