using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("References")]
    public GameObject lifePrefab;
    public Transform cube; // 旋轉的 cube
    public int maxTries = 30;
    public float minDistanceFromSnake = 1.5f;
    public float surfaceOffset = 0.1f;
    public int maxLifeCount = 1; // 同時存在的生命物件數量上限

    private List<Transform> snakeBodies => SnakeManager.Instance?.GetBodyParts();
    private List<GameObject> lifeObjects = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return null;
        SpawnLife();
    }

    public void ClearLives()
    {
        foreach (var lifeObj in lifeObjects)
        {
            if (lifeObj != null)
                Destroy(lifeObj);
        }
        lifeObjects.Clear();
    }

    public void SpawnLife()
    {
        if (GameManager.Instance.isGameOver || lifeObjects.Count >= maxLifeCount)
            return;

        if (lifePrefab == null || cube == null)
        {
            Debug.LogError("LifeManager：未設定 lifePrefab 或 cube");
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
                GameObject lifeObj = Instantiate(lifePrefab, worldPoint, Quaternion.identity);
                lifeObj.transform.localScale = Vector3.one * 0.8f;

                var followRotation = lifeObj.AddComponent<FollowRotation>();
                followRotation.target = cube;

                lifeObjects.Add(lifeObj);

                Debug.Log("LifeManager：生成生命物件在 cube 表面 " + worldPoint);
                return;
            }
        }

        Debug.LogWarning("LifeManager：找不到合適位置生成生命物件");
    }

    public void RemoveLife(GameObject lifeObj)
    {
        if (lifeObjects.Contains(lifeObj))
        {
            lifeObjects.Remove(lifeObj);
            Destroy(lifeObj);
        }
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

    // 讓生命物件跟隨 cube 旋轉
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
