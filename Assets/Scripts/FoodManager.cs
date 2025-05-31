using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [Header("References")]
    public GameObject foodPrefab;
    public Transform cube; // 旋轉的 cube
    public int maxTries = 30;
    public float minDistanceFromSnake = 2.5f;
    public float surfaceOffset = 0.1f; // 食物稍微遠離 cube 表面的偏移量

    private List<Transform> snakeBodies => SnakeManager.Instance?.GetBodyParts();
    private GameObject currentFood;

    private void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return null;
        SpawnFood();
    }

    public void ClearFood()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }
    }
    public void SpawnFood()
    {
        if (GameManager.Instance.isGameOver)
            return;

        // 如果場上已有食物，先刪掉
        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }

        if (foodPrefab == null || cube == null)
        {
            Debug.LogError("FoodManager：未設定 foodPrefab 或 cube");
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
                currentFood = Instantiate(foodPrefab, worldPoint, Quaternion.identity);
                currentFood.transform.localScale = Vector3.one * 0.8f;

                var followRotation = currentFood.AddComponent<FollowRotation>();
                followRotation.target = cube;

                Debug.Log("FoodManager：生成食物在 cube 表面 " + worldPoint);
                return;
            }
        }

        Debug.LogWarning("FoodManager：找不到合適位置生成食物");
    }


    // 檢查食物位置是否被蛇體佔用
    private bool IsPositionOccupied(Vector3 point)
    {
        float radius = minDistanceFromSnake;

        // 只檢查 SnakeLayer
        int snakeLayerMask = LayerMask.GetMask("body");

        Collider[] hits = Physics.OverlapSphere(point, radius, snakeLayerMask);

        return hits.Length > 0;
    }


    // FollowRotation 脚本，讓食物跟隨 cube 旋轉
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

            // 計算相對於目標物體的偏移
            localOffset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 更新位置，使其隨著目標物體旋轉，但保持偏移量
            transform.position = target.position + target.rotation * localOffset;
            transform.up = Vector3.up; // 食物的「上」方向對齊世界向上


        }
    }
}





