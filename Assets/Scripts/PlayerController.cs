using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 2.2f; // 移動速度
    public VariableJoystick variableJoystick; // 虛擬搖桿
    public Rigidbody rb; // 玩家剛體
    public Transform cube; // 中心立方體
    public float attractionForce = 10f; // 吸引力大小
    public AudioClip hitBodySound;

    public GameObject bombEffectPrefab;
    public GameObject lifeEffectPrefab;
    public GameObject diamondEffectPrefab;



    private void Start()
    {
        if (cube == null && SnakeManager.Instance != null)
        {
            cube = SnakeManager.Instance.cube;
            Debug.Log("PlayerController: 抓到 cube: " + cube.name);
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            Debug.Log("PlayerController: 抓到 Rigidbody");
        }

        if (variableJoystick == null)
        {
            variableJoystick = FindAnyObjectByType<VariableJoystick>();
            if (variableJoystick != null)
            {
                Debug.Log("PlayerController: 成功找到場景中的 VariableJoystick");
            }
            else
            {
                Debug.LogWarning("PlayerController: 場景中找不到 VariableJoystick");
            }
        }
    }

    private void FixedUpdate()
    {
        // 如果 GameManager 不存在（例如在 Menu 或 Store 場景），則略過遊戲邏輯
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (cube == null) return;

        // 吸引力使玩家靠近 cube
        Vector3 gravityDirection = (cube.position - rb.position).normalized;
        rb.AddForce(gravityDirection * attractionForce, ForceMode.Acceleration);

        // 玩家移動控制
        Vector3 moveDirLocal = new Vector3(variableJoystick.Horizontal, 0, variableJoystick.Vertical);
        Vector3 moveDirection = GetWorldMoveDirection(moveDirLocal);
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        // 面向移動方向
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }

    private Vector3 GetWorldMoveDirection(Vector3 inputDir)
    {
        return new Vector3(inputDir.x, 0, inputDir.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (SnakeManager.Instance == null || SnakeManager.Instance.GetBodyParts() == null)
        {
            Debug.LogWarning("PlayerController: SnakeManager 或其身體部位為 null");
            return;
        }

        GameObject otherObj = collision.gameObject;

        // 嘗試播放音效
        ItemSoundPlayer soundPlayer = otherObj.GetComponent<ItemSoundPlayer>();
        if (soundPlayer != null && soundPlayer.sound != null)
        {
            SoundManager.Instance.PlaySound(soundPlayer.sound, 0.8f); // volume 可調
        }


        if (otherObj.transform == cube) return;

        // 吃到食物
        if (otherObj.CompareTag("Food"))
        {
            if (diamondEffectPrefab != null)
            {
                Instantiate(diamondEffectPrefab, otherObj.transform.position, Quaternion.identity);
            }
            if (!otherObj.TryGetComponent<AlreadyEatenMarker>(out _))
            {
                otherObj.AddComponent<AlreadyEatenMarker>();
                GameManager.Instance.FoodEaten(otherObj);
                Debug.Log("吃到食物");
            }
            return;
        }

        // 碰到炸彈
        if (otherObj.CompareTag("Bomb"))
        {
            Debug.Log("碰到炸彈");

            // ✅ 加入炸彈粒子特效
            if (bombEffectPrefab != null)
            {
                Instantiate(bombEffectPrefab, otherObj.transform.position, Quaternion.identity);
            }
            GameManager.Instance.PlayerHitBody(-1);
            Destroy(otherObj); // 立即摧毀，不會卡住
            return;
        }

        // 碰到愛心
        if (otherObj.CompareTag("Life"))
        {
            Debug.Log("碰到愛心");

            // ✅ 加入愛心粒子特效
            if (lifeEffectPrefab != null)
            {
                Instantiate(lifeEffectPrefab, otherObj.transform.position, Quaternion.identity);
            }
            GameManager.Instance.AddLife(1);
            Destroy(otherObj); // 立即摧毀
            return;
        }

        // 撞到蛇身體
        int bodyIndex = SnakeManager.Instance.GetBodyIndex(collision.transform);
        if (bodyIndex > 3)
        {
            Debug.Log($"撞到 Snake Body，第 {bodyIndex} 節");

            // 如果還沒 Game Over，才播放音效與扣血
            if (!GameManager.Instance.isGameOver)
            {
                if (SoundManager.Instance != null && hitBodySound != null)
                {
                    SoundManager.Instance.PlaySound(hitBodySound, 0.8f);
                }

                GameManager.Instance.PlayerHitBody(bodyIndex);
            }
        }


    }


    public class AlreadyEatenMarker : MonoBehaviour
    {
        // 空的標記類別，只用來判斷食物是否已經被吃過
    }
}

