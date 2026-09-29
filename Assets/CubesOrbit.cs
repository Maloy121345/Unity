using System.Collections.Generic;
using UnityEngine;

public class CubesOrbit : MonoBehaviour
{
    [Header("Откуда брать кубики")]
    [SerializeField] private GameObject cubePrefab;

    [Header("Параметры вращения")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private float height = 0f;

    [Header("Сколько кубиков и как их расставить")]
    [SerializeField] private int cubeCount = 8;
    [SerializeField] private bool uniformDistribution = true;
    [SerializeField] private float angleStepWhenChain = 15f;

    private readonly List<GameObject> spawnedCubes = new List<GameObject>();

    private void Awake()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("CubesOrbit: не назначен префаб кубика.");
            return;
        }
        if (cubeCount <= 0)
        {
            Debug.LogWarning("CubesOrbit: cubeCount <= 0.");
            return;
        }

        for (int i = 0; i < cubeCount; i++)
        {
            var cube = Instantiate(cubePrefab, transform);
            spawnedCubes.Add(cube);
        }

        LayoutCubes();
    }

    private void Update()
    {
        var step = rotationSpeed * Time.deltaTime;
        
        if (clockwise)
            step = -step;

        transform.Rotate(Vector3.up, step, Space.Self);

        LayoutCubes();
    }

    private void LayoutCubes()
    {
        var n = spawnedCubes.Count;
        
        for (var i = 0; i < n; i++)
        {
            var baseDeg = uniformDistribution ? (360f / n * i) : (angleStepWhenChain * i);
            var rad = baseDeg * Mathf.Deg2Rad;
            var x = radius * Mathf.Cos(rad);
            var z = radius * Mathf.Sin(rad);

            spawnedCubes[i].transform.localPosition = new Vector3(x, height, z);
        }
    }
}