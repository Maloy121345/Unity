using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceGameController : MonoBehaviour
{
    [Header("Настройки кубиков")]
    [SerializeField] private Dice[] dice;

    [Header("Сила броска")]
    [SerializeField] private float minForce = 9f;
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float minTorque = 350f;
    [SerializeField] private float maxTorque = 750f;
    [SerializeField] private float sideSpread = 2.5f;

    [Header("UI и Ввод")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private InputActionAsset actions;

    private InputAction throwAction;
    private InputAction rebindAction;
    private bool waitingForSettle;
    private bool rebinding;
    private int rebindArmIn;

    private void OnEnable()
    {
        if (actions == null) return;

        throwAction = actions.FindAction("Throw");
        rebindAction = actions.FindAction("Rebind");

        if (throwAction != null) throwAction.performed += _ => ThrowDice();
        if (rebindAction != null) rebindAction.performed += _ => StartRebind();

        actions.Enable();
    }

    private void OnDisable()
    {
        if (actions != null) actions.Disable();
        rebinding = false;
    }

    private void Update()
    {
        if (!rebinding) return;

        if (rebindArmIn > 0)
        {
            rebindArmIn--;
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        foreach (var key in keyboard.allKeys)
        {
            if (!key.wasPressedThisFrame) continue;

            throwAction.ApplyBindingOverride(0, new InputBinding { overridePath = "<Keyboard>/" + key.name });
            rebinding = false;

            if (resultText != null) resultText.text = "Новая кнопка броска: " + key.displayName;
            break;
        }
    }

    private void ThrowDice()
    {
        if (waitingForSettle || rebinding) return;

        waitingForSettle = true;
        if (resultText != null) resultText.text = "Бросок...";

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
        foreach (var d in dice)
        {
            sum += d.GetTopFaceValue();
        }

        if (resultText != null) resultText.text = $"Сумма: {sum}";

        waitingForSettle = false;
    }

    private void StartRebind()
    {
        if (throwAction == null || rebinding) return;

        rebinding = true;
        rebindArmIn = 2;

        if (resultText != null) resultText.text = "Нажми новую кнопку для броска...";
    }

    private float RandomSign()
    {
        return Random.value < 0.5f ? -1f : 1f;
    }
}