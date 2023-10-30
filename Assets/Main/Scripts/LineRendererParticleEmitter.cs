using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class LineRendererParticleEmitter : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private ParticleSystem particleSystem;
    private ParticleSystem.EmitParams emitParams;
    private int lastPointIndex = 0;

    public float waitTimer;
    public float maxDistance = 20;
    public float distance;
    public float inverse;

    public void setUp(float Distance, float MaxDistance)
    {
      distance = Distance;
      maxDistance = MaxDistance;
      inverse = maxDistance - distance;
      waitTimer = inverse/100f;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(EmitRandomParticles());
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.P))
        {
          EmitParticlesAlongLine();
        }
    }

    IEnumerator EmitRandomParticles()
    {
        while (true)
        {
            EmitParticleAtRandomPoint();
            //yield return null; // Wait for the next frame
            yield return new WaitForSeconds(waitTimer);
        }
    }

    void EmitParticleAtRandomPoint()
    {
        // Get the current number of positions in the LineRenderer
        int positionCount = lineRenderer.positionCount;

        if (positionCount > 0)
        {
            // Choose a random index
            int randomIndex = Random.Range(0, positionCount);
            Vector3 randomPosition = lineRenderer.GetPosition(randomIndex);

            // Emit a particle at the random position
            emitParams.position = randomPosition;
            particleSystem.Emit(emitParams, 1);
        }
    }

    void EmitParticlesAlongLine()
    {
        // Get the current number of positions in the LineRenderer
        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);

        lastPointIndex = 0;
        // Emit a particle at each point along the line
        for (int i = lastPointIndex; i < linePositions.Length; i++)
        {
            emitParams.position = linePositions[i];
            particleSystem.Emit(emitParams, 1);
            lastPointIndex = i;
        }
    }
}
