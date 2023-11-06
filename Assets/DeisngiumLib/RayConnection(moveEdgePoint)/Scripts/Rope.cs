using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Deisgnium.RayConnection
{

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    [Header("Instanced Mesh Details")]
    [SerializeField, Tooltip("The Mesh of chain link to render")] Mesh link;
    [SerializeField, Tooltip("The chain link material, must have gpu instancing enabled!")] Material linkMaterial;

    [Space]

    [Header("Demo Parameters")]
    [SerializeField] int step = 4;

    [Space]

    [Header("Verlet Parameters")]

    [SerializeField, Tooltip("The distance between each link in the chain")] float nodeDistance = 0.35f;
    [SerializeField, Tooltip("The scale of the chain")] float nodeScale = 0.35f;
    [SerializeField, Tooltip("The radius of the sphere collider used for each chain link")] float nodeColliderRadius = 0.2f;

    [SerializeField, Tooltip("Works best with a lower value")] float gravityStrength = 2;

    [SerializeField, Tooltip("The number of chain links. Decreases performance with high values and high iteration")] int totalNodes = 100;

    [SerializeField, Range(0, 1), Tooltip("Modifier to dampen velocity so the simulation can stabilize")] float velocityDampen = 0.95f;

    [SerializeField, Range(0, 0.99f), Tooltip("The stiffness of the simulation. Set to lower values for more elasticity")] float stiffness = 0.8f;

    [SerializeField, Tooltip("Setting this will test collisions for every n iterations. Possibly more performance but less stable collisions")] int iterateCollisionsEvery = 1;

    [SerializeField, Tooltip("Iterations for the simulation. More iterations is more expensive but more stable")] int iterations = 100;

    [SerializeField, Tooltip("How many colliders to test against for every node.")] int colliderBufferSize = 1;


    RaycastHit[] raycastHitBuffer;
    Collider[] colliderHitBuffer;
    Camera cam;

    // Need a better way of stepping through collisions for high Gravity
    // And high Velocity
    Vector3 gravity;

    [SerializeField]
    Vector3 startLock;
    Vector3 endLock;

    bool isStartLocked = false;
    bool isEndLocked = false;

    [Space]

    // For Debug Drawing the chain/rope
    [Header("Line Renderer")]
    [SerializeField, Tooltip("Width for the line renderer")] float ropeWidth = 0.1f;

    LineRenderer lineRenderer;
    Vector3[] linePositions;

    Vector3[] previousNodePositions;

    Vector3[] currentNodePositions;
    Quaternion[] currentNodeRotations;

    SphereCollider nodeCollider;
    GameObject nodeTester;
    Matrix4x4[] matrices;
    Vector4[] colors;

    [Space]
    [SerializeField] ObjResource objResource;
    public List<GameObject> attachObjs = new List<GameObject>();
    public List<int> attachIndexs = new List<int>();

    bool _isReady = false;
    private MaterialPropertyBlock block;

    int _displayIndex = 0;
    float _displaySpeed = 100f;
    [SerializeField] float _displayTimer;

    int previous;
    int previousprevious;
    public int numberToCheck = 0; // The number to check for being a multiple
    public int multipleOf = 1;     // The number to check if the first number is a multiple of

    public Color[] colours;

    public void Set(Vector3 stPos, Vector3 endPos)
    {
        float dist = Vector3.Distance(stPos, endPos);
        totalNodes = Mathf.FloorToInt(dist/nodeDistance);
        currentNodePositions = new Vector3[totalNodes];
        previousNodePositions = new Vector3[totalNodes];
        currentNodeRotations = new Quaternion[totalNodes];

        raycastHitBuffer = new RaycastHit[colliderBufferSize];
        colliderHitBuffer = new Collider[colliderBufferSize];
        gravity = new Vector3(0, -gravityStrength, 0);
        cam = Camera.main;
        lineRenderer = this.GetComponent<LineRenderer>();

        // using a single dynamically created GameObject to test collisions on every node
        nodeTester = new GameObject();
        nodeTester.name = "Node Tester";
        nodeTester.layer = 8;
        nodeCollider = nodeTester.AddComponent<SphereCollider>();
        nodeCollider.radius = nodeColliderRadius;

        matrices = new Matrix4x4[totalNodes];
        colors = new Vector4[totalNodes];
        attachObjs = new List<GameObject>();
        attachIndexs = new List<int>();

        startLock = stPos;
        endLock = endPos;
        isStartLocked = isEndLocked = true;
        Vector3 startPosition = stPos;

        block = new MaterialPropertyBlock();
        for (int i = 0; i < totalNodes; i++)
        {
            currentNodePositions[i] = startPosition;
            currentNodeRotations[i] = Quaternion.identity;

            previousNodePositions[i] = startPosition;

            matrices[i] = Matrix4x4.TRS(startPosition, Quaternion.identity, Vector3.one);

            startPosition.y -= nodeDistance;

            //colors[i] = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            colors[i] = colours[UnityEngine.Random.Range(0, 4)];

            // attach object
            if (i % step == 0 && (i >= 2 && i <= totalNodes - 3))
            {
                    if (IsMultiple(numberToCheck, multipleOf))
                    {
                        int oIndex = 0;
                        do
                        {
                            oIndex = UnityEngine.Random.Range(0, objResource.objlist.Count);
                        } while (oIndex == previous || oIndex == previousprevious);

                        previousprevious = previous;
                        previous = oIndex;
                        //int oIndex = Random.Range(0, objResource.objlist.Count);
                        GameObject obj = Instantiate(objResource.objlist[oIndex].obj, Vector3.zero, Quaternion.identity);
                        obj.transform.SetParent(this.transform, false);
                        obj.name = $"{objResource.objlist[oIndex].name}_{i}";
                        obj.SetActive(false);
                        attachObjs.Add(obj);
                        attachIndexs.Add(i);
                    }
                    numberToCheck += 1;
            }
        }
        block.SetVectorArray("_Color", colors);

        // for line renderer data
        linePositions = new Vector3[totalNodes];

        _isReady = true;

        StartCoroutine(stopSim());
    }

    IEnumerator stopSim()
    {
            yield return new WaitForSeconds(5);
            _isReady = false;
    }

        private bool IsMultiple(int number, int multiple)
    {
        return number % multiple == 0;
    }

        public void UpdateStartPt(Vector3 stPos)
    {
      startLock = stPos;
    }

    public void UpdateEndPt(Vector3 endPos)
    {
      endLock = endPos;
    }

    void Update()
    {
        if (!_isReady) return;

        // Draw rope with lineRenderer
        DrawRope();
        // Draw objects on rope
        DrawDecoration();

        // Instanced drawing here is really performant over using GameObjects
        if (totalNodes > 2)
          Graphics.DrawMeshInstanced(link, 0, linkMaterial, matrices, totalNodes, block);
    }

    private void FixedUpdate()
    {
        if (!_isReady) return;

        Simulate();

        for (int i = 0; i < iterations; i++)
        {
            ApplyConstraint();

            if(i % iterateCollisionsEvery == 0)
            {
                AdjustCollisions();
            }
        }

        SetAngles();
        TranslateMatrices();
    }

    private void Simulate()
    {
        var fixedDt = Time.fixedDeltaTime;
        for (int i = 0; i < totalNodes; i++)
        {
            Vector3 velocity = currentNodePositions[i] - previousNodePositions[i];
            velocity *= velocityDampen;

            previousNodePositions[i] = currentNodePositions[i];

            // calculate new position
            Vector3 newPos = currentNodePositions[i] + velocity;
            newPos += gravity * fixedDt;
            Vector3 direction = currentNodePositions[i] - newPos;

            currentNodePositions[i] = newPos;
        }
    }

    private void AdjustCollisions()
    {
        for (int i = 0; i < totalNodes; i++)
        {
            if(i % 2 == 0) continue;

            int result = -1;
            result = Physics.OverlapSphereNonAlloc(currentNodePositions[i], nodeColliderRadius + 0.01f, colliderHitBuffer, ~(1 << 8));

            for (int n = 0; n < result; n++)
            {
                // if (colliderHitBuffer[n].gameObject.layer != 8)
                {
                    Vector3 colliderPosition = colliderHitBuffer[n].transform.position;
                    Quaternion colliderRotation = colliderHitBuffer[n].gameObject.transform.rotation;

                    Vector3 dir;
                    float distance;

                    Physics.ComputePenetration(nodeCollider, currentNodePositions[i], Quaternion.identity, colliderHitBuffer[n], colliderPosition, colliderRotation, out dir, out distance);

                    currentNodePositions[i] += dir * distance;
                }
            }
        }
    }

    private void ApplyConstraint()
    {
        currentNodePositions[0] = startLock;
        if(isStartLocked)
        {
            currentNodePositions[totalNodes - 1] = endLock;
        }

        for (int i = 0; i < totalNodes - 1; i++)
        {
            var node1 = currentNodePositions[i];
            var node2 = currentNodePositions[i + 1];

            // Get the current distance between rope nodes
            float currentDistance = (node1 - node2).magnitude;
            float difference = Mathf.Abs(currentDistance - nodeDistance);
            Vector3 direction = Vector3.zero;

            // determine what direction we need to adjust our nodes
            if (currentDistance > nodeDistance)
            {
                direction = (node1 - node2).normalized;
            }
            else if (currentDistance < nodeDistance)
            {
                direction = (node2 - node1).normalized;
            }

            // calculate the movement vector
            Vector3 movement = direction * difference;

            // apply correction
            currentNodePositions[i] -= (movement * stiffness);
            currentNodePositions[i + 1] += (movement * stiffness);
        }
    }

    void SetAngles()
    {
        for (int i = 0; i < totalNodes - 1; i++)
        {
            var node1 = currentNodePositions[i];
            var node2 = currentNodePositions[i + 1];

            var dir = (node2 - node1).normalized;
            if(dir != Vector3.zero)
            {
                if( i > 0)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.right);
                    currentNodeRotations[i + 1] = desiredRotation;
                }
                else if( i < totalNodes - 1)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.right);
                    currentNodeRotations[i + 1] = desiredRotation;
                }
                else
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(dir, Vector3.right);
                    currentNodeRotations[i] = desiredRotation;
                }
            }

            if( i % 2 == 0 && i != 0)
            {
                currentNodeRotations[i + 1] *= Quaternion.Euler(0, 0, 90);
            }
        }
    }

    void TranslateMatrices()
    {
        for(int i = 0; i < totalNodes; i++)
        {
            matrices[i].SetTRS(currentNodePositions[i], currentNodeRotations[i], Vector3.one * nodeScale);
        }
    }

    void DrawRope()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        for (int n = 0; n < totalNodes; n++)
        {
            linePositions[n] = currentNodePositions[n];
        }

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);
    }

    void DrawDecoration()
    {
      if (_displayIndex <= attachObjs.Count) {
        _displayTimer = Time.deltaTime * _displaySpeed;
        if (_displayTimer > 1f)
        {
          _displayIndex++;
          _displayTimer = 0f;
        }
      }

      for (int i=0; i< attachObjs.Count; i++)
      {
        AttachObject(attachObjs[i], attachIndexs[i]);

        if (!attachObjs[i].activeSelf && i <= _displayIndex) {
          attachObjs[i].SetActive(true);
          attachObjs[i].transform.localScale = Vector3.zero;
          attachObjs[i].transform.DOScale(Vector3.one * Random.Range(0.6f, 1f), 0.5f)
                       .SetDelay(0.1f * i)
                       .SetEase(Ease.OutBounce);
        }
      }
    }

    void AttachObject(GameObject obj, int nodeIndex)
    {
        obj.transform.position = currentNodePositions[nodeIndex];
    }

}

}
