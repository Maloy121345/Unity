using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    private Rigidbody rb;
    private int settleCounter;

    private static readonly Vector3[] FaceNormals =
    {
        Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    private static readonly int[] FaceValues = { 1, 6, 2, 5, 3, 4 };

    public bool IsSettled { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 force, Vector3 torque)
    {
        IsSettled = false;
        settleCounter = 0;
        rb.WakeUp();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (IsSettled) return;

        if (rb.linearVelocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
        {
            settleCounter++;
            if (settleCounter >= 10) IsSettled = true;
        }
        else
        {
            settleCounter = 0;
        }
    }

    public int GetTopFaceValue()
    {
        var localUp = transform.InverseTransformDirection(Vector3.up);
        var bestIndex = 0;
        var bestDot = float.MinValue;

        for (var i = 0; i < FaceNormals.Length; i++)
        {
            var d = Vector3.Dot(FaceNormals[i], localUp);
            if (d > bestDot)
            {
                bestDot = d;
                bestIndex = i;
            }
        }

        return FaceValues[bestIndex];
    }
}