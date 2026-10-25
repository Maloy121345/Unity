using TMPro;
using UnityEngine;

public class DiceUIView : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text thresholdsText;
    [SerializeField] private TMP_Text diceCountText;
    [SerializeField] private TMP_Text outcomeText;

    [SerializeField] private TMP_InputField diceCountInput;
    [SerializeField] private TMP_InputField winInput;
    [SerializeField] private TMP_InputField drawInput;
    [SerializeField] private TMP_InputField loseInput;

    public void Initialize(DiceGameController controller)
    {
        SetDiceCount(controller.DiceCount);
        SetThresholds(controller.WinThreshold, controller.DrawThreshold, controller.LoseThreshold);
        SetResult("Нажми кнопку «Перебросить»");
        SetOutcome("");
    }

    public void SetResult(string value)
    {
        resultText.text = value;
    }

    public void SetDiceCount(int count)
    {
        diceCountText.text = "Число кубиков: " + count;
    }

    public void SetThresholds(int win, int draw, int lose)
    {
        thresholdsText.text = $"Победа от {win} | Ничья от {draw} | Поражение от {lose}";
    }

    public void SetOutcome(string outcome)
    {
        outcomeText.text = outcome;
    }

    public int GetDiceCountOrDefault(int fallback)
    {
        if (int.TryParse(diceCountInput.text, out int value) && value >= 1 && value <= 10)
            return value;
        return fallback;
    }

        public bool TryGetThresholds(out int win, out int draw, out int lose)
    {
        win = 0;
        draw = 0;
        lose = 0;

        return int.TryParse(winInput.text, out win)
               && int.TryParse(drawInput.text, out draw)
               && int.TryParse(loseInput.text, out lose);
    }
}