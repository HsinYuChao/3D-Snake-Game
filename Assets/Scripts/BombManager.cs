using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [Header("References")]
    public GameObject bombPrefab;
    public Transform cube;
    public int maxTries = 30;
    public float minDistanceFromSnake = 1.5f;
    public float surfaceOffset = 0.1f;

    private List<Transform> snakeBodies => SnakeManager.Instance?.GetBodyParts();
    private GameObject currentBomb;

    private void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return null;
        SpawnBomb();
    }

    public void ClearBomb()
    {
        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }
    }

    public void SpawnBomb()
    {
        if (GameManager.Instance.isGameOver)
            return;

        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }

        if (bombPrefab == null || cube == null)
        {
            Debug.LogError("BombManager：未設定 bombPrefab 或 cube");
            return;
        }

        Vector3 cubeSize = Vector3.one;
        float halfX = cubeSize.x * 0.5f;
        float halfY = cubeSize.y * 0.5f;
        float halfZ = cubeSize.z * 0.5f;

        for (int i = 0; i < maxTries; i++)
        {
            int face = Random.Range(0, 6);
            Vector3 localPoint = Vector3.zero;

            switch (face)
            {
                case 0:
                    localPoint = new Vector3(halfX + surfaceOffset, Random.Range(-halfY, halfY), Random.Range(-halfZ, halfZ));
                    break;
                case 1:
                    localPoint = new Vector3(-halfX - surfaceOffset, Random.Range(-halfY, halfY), Random.Range(-halfZ, halfZ));
                    break;
                case 2:
                    localPoint = new Vector3(Random.Range(-halfX, halfX), halfY + surfaceOffset, Random.Range(-halfZ, halfZ));
                    break;
                case 3:
                    localPoint = new Vector3(Random.Range(-halfX, halfX), -halfY - surfaceOffset, Random.Range(-halfZ, halfZ));
                    break;
                case 4:
                    localPoint = new Vector3(Random.Range(-halfX, halfX), Random.Range(-halfY, halfY), halfZ + surfaceOffset);
                    break;
                case 5:
                    localPoint = new Vector3(Random.Range(-halfX, halfX), Random.Range(-halfY, halfY), -halfZ - surfaceOffset);
                    break;
            }

            Vector3 worldPoint = cube.TransformPoint(localPoint);

            if (!IsPositionOccupied(worldPoint))
            {
                currentBomb = Instantiate(bombPrefab, worldPoint, Quaternion.identity);
                currentBomb.transform.localScale = Vector3.one * 0.8f;

                var followRotation = currentBomb.AddComponent<FollowRotation>();
                followRotation.target = cube;

                Debug.Log("BombManager：生成炸彈在 cube 表面 " + worldPoint);
                return;
            }
        }

        Debug.LogWarning("BombManager：找不到合適位置生成炸彈");
    }

    private bool IsPositionOccupied(Vector3 point)
    {
        if (snakeBodies == null || snakeBodies.Count == 0)
            return false;

        float minDistSqr = minDistanceFromSnake * minDistanceFromSnake;

        foreach (Transform bodyPart in snakeBodies)
        {
            if ((bodyPart.position - point).sqrMagnitude < minDistSqr)
                return true;
        }

        return false;
    }

    // 讓炸彈跟隨 cube 旋轉
    private class FollowRotation : MonoBehaviour
    {
        public Transform target;

        private Vector3 localOffset;

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError("FollowRotation：target 未設定");
                return;
            }

            localOffset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            transform.position = target.position + target.rotation * localOffset;
            transform.up = Vector3.up;
        }
    }
}
