using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager Instance;

    [Header("References")]
    public Transform cube;

    private GameObject bodyPrefab;
    private Transform player;

    [Header("Movement Settings")]
    public float followDelay = 0.05f;
    public int pointsPerBody = 3;
    public float bodyMoveSpeed = 3.1f;
    public float cubeAttractionForce = 100f;

    private List<Transform> bodyParts = new List<Transform>();
    private Dictionary<Transform, int> bodyIndexMap = new Dictionary<Transform, int>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private Vector3[] velocityArray = new Vector3[200]; // 餈質馱撟單�����摨�
    private Vector3 lastRecordedPos;
    //private float recordTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerManager.Instance != null)
        {
            var spawned = PlayerManager.Instance.spawnedPlayer;
            if (spawned != null)
            {
                player = spawned.transform;
                bodyPrefab = PlayerManager.Instance.GetSelectedBodyPrefab();
            }
        }

        if (player == null)
        {
            Debug.LogError("SnakeManager: Player 撠���芰�Ｙ��嚗�");
            return;
        }

        bodyParts.Add(player);
        bodyIndexMap.Add(player, 0);
        lastRecordedPos = cube.InverseTransformPoint(player.position);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver || player == null) return;

        ApplyGravity();
        RecordPositionHistory();
        MoveBodyParts();
    }


    private void ApplyGravity()
    {
        foreach (Transform part in bodyParts)
        {
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 gravityDir = (cube.position - rb.position).normalized;
                rb.AddForce(gravityDir * cubeAttractionForce, ForceMode.Acceleration);
            }
        }
    }

    private void RecordPositionHistory()
    {
        Vector3 localPos = cube.InverseTransformPoint(player.position);

        if (positionHistory.Count == 0 || Vector3.Distance(positionHistory[0], localPos) > 0.05f)
        {
            positionHistory.Insert(0, localPos);
        }

        int maxHistory = bodyParts.Count * pointsPerBody + 10;
        if (positionHistory.Count > maxHistory)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }



    private void MoveBodyParts()
    {
        for (int i = 1; i < bodyParts.Count; i++)
        {
            Transform body = bodyParts[i];
            Rigidbody rb = body.GetComponent<Rigidbody>();

            int targetIndex = i * pointsPerBody;
            if (targetIndex < positionHistory.Count)
            {
                Vector3 targetWorldPos = cube.TransformPoint(positionHistory[targetIndex]);

                Vector3 moveDir = targetWorldPos - body.position;
                float dist = moveDir.magnitude;
                if (dist < 0.01f) continue;

                float maxMove = bodyMoveSpeed * Time.fixedDeltaTime;
                Vector3 newPos = Vector3.MoveTowards(body.position, targetWorldPos, maxMove);

                rb.MovePosition(newPos);
                rb.rotation = Quaternion.identity; // ��臭誑��急��銝�閬����頧�嚗����蝣箔��蝘餃��甇�撣�
            }
        }
    }

    public void AddBodySegment()
    {
        if (bodyPrefab == null)
        {
            Debug.LogWarning("SnakeManager: 撠���芾身摰� bodyPrefab嚗���⊥��������頨恍��");
            return;
        }

        int requiredHistory = (bodyParts.Count + 1) * pointsPerBody;
        if (positionHistory.Count <= requiredHistory)
        {
            Debug.LogWarning("SnakeManager: 鞈����銝�頞喉��銝�������頨恍��");
            return;
        }

        Transform last = bodyParts[bodyParts.Count - 1];
        int spawnIndex = (bodyParts.Count - 1) * pointsPerBody;
        Vector3 spawnLocalPos = positionHistory[spawnIndex];
        Vector3 spawnPos = cube.TransformPoint(spawnLocalPos);

        GameObject newBody = Instantiate(bodyPrefab, spawnPos, last.rotation, transform);

        Collider[] newColliders = newBody.GetComponentsInChildren<Collider>();

        // ��芸蕭��交�啗澈擃������嗡��頨恍��銋�������蝣唳��嚗�銝�敹賜�� Player
        foreach (Transform existing in bodyParts)
        {
            if (existing == null || existing == newBody.transform) continue;

            // 頝喲�� Player嚗�index 0嚗�
            if (existing == bodyParts[0]) continue;

            Collider[] existingColliders = existing.GetComponentsInChildren<Collider>();

            foreach (var colA in existingColliders)
            {
                foreach (var colB in newColliders)
                {
                    Physics.IgnoreCollision(colA, colB);
                }
            }
        }

        int newIndex = bodyParts.Count;
        bodyParts.Add(newBody.transform);
        bodyIndexMap.Add(newBody.transform, newIndex);
    }


    public void ResetSnake()
    {
        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            Transform body = bodyParts[i];
            bodyIndexMap.Remove(body);
            Destroy(body.gameObject);
            bodyParts.RemoveAt(i);
        }

        if (player != null)
        {
            player.position = cube.position + new Vector3(0, 1, 0);
            player.rotation = Quaternion.identity;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        positionHistory.Clear();
        //recordTimer = 0f;
        lastRecordedPos = cube.InverseTransformPoint(player.position);
    }

    public List<Transform> GetBodyParts() => bodyParts;

    public int GetBodyIndex(Transform part)
    {
        if (bodyIndexMap.TryGetValue(part, out int index))
            return index;
        return -1;
    }

    private void OnDrawGizmos()
    {
        if (positionHistory == null || positionHistory.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < positionHistory.Count - 1; i++)
        {
            Vector3 p1 = cube.TransformPoint(positionHistory[i]);
            Vector3 p2 = cube.TransformPoint(positionHistory[i + 1]);
            Gizmos.DrawLine(p1, p2);
        }
    }
}



