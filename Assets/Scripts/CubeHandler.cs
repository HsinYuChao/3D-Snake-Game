using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHandler : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public VariableJoystick variableJoystick;
    public float edgeThreshold = 0.1f;

    private Transform player;
    private bool isRotating = false;
    public Vector3 rotationAxis = Vector3.zero;
    private float totalRotation = 0f;

    private float rotationCooldown = 0.2f;
    private float lastRotationTime = 0f;

    public enum FaceDirection
    {
        Top,
        Bottom,
        Front,
        Back,
        Left,
        Right
    }
    public FaceDirection currentTopFace = FaceDirection.Top;

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;

        // 嘗試抓 Player（如果還沒抓到）
        if (player == null)
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.spawnedPlayer != null)
            {
                player = PlayerManager.Instance.spawnedPlayer.transform;
            }
            else
            {
                return; // 還沒生出 player，先跳過
            }
        }

        if (Time.time - lastRotationTime < rotationCooldown)
            return;

        if (isRotating)
        {
            ContinueRotation();
            return;
        }

        if (IsNearEdge())
        {
            Vector3 playerDirection = player.position - transform.position;
            Vector3 rotationDirection = Vector3.zero;

            if (Mathf.Abs(playerDirection.x) > Mathf.Abs(playerDirection.z))
            {
                rotationDirection.z = playerDirection.x > 0 ? 1 : -1;
            }
            else
            {
                rotationDirection.x = playerDirection.z > 0 ? -1 : 1;
            }

            rotationAxis = rotationDirection.normalized;
            isRotating = true;
            totalRotation = 0f;
            lastRotationTime = Time.time;
        }
    }

    void ContinueRotation()
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        if (totalRotation + rotationThisFrame >= 90f)
        {
            rotationThisFrame = 90f - totalRotation;
            isRotating = false;
            currentTopFace = GetNextTopFace(currentTopFace, rotationAxis);
        }

        transform.Rotate(rotationAxis * rotationThisFrame, Space.World);
        totalRotation += rotationThisFrame;
    }

    bool IsNearEdge()
    {
        if (player == null) return false;

        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        Vector3 localPos = transform.InverseTransformPoint(player.position);

        return (Mathf.Abs(playerVelocity.x) > 0.1f || Mathf.Abs(playerVelocity.z) > 0.1f) &&
               (Mathf.Abs(localPos.x) > edgeThreshold || Mathf.Abs(localPos.z) > edgeThreshold);
    }

    public FaceDirection GetTopFace() => currentTopFace;

    public FaceDirection GetNextTopFace(FaceDirection current, Vector3 axis)
    {
        if (axis == Vector3.back)
        {
            switch (current)
            {
                case FaceDirection.Top: return FaceDirection.Left;
                case FaceDirection.Bottom: return FaceDirection.Right;
                case FaceDirection.Right: return FaceDirection.Top;
                case FaceDirection.Left: return FaceDirection.Bottom;
                case FaceDirection.Front: return FaceDirection.Back;
                case FaceDirection.Back: return FaceDirection.Front;
            }
        }
        else if (axis == Vector3.forward)
        {
            switch (current)
            {
                case FaceDirection.Top: return FaceDirection.Right;
                case FaceDirection.Bottom: return FaceDirection.Left;
                case FaceDirection.Right: return FaceDirection.Bottom;
                case FaceDirection.Left: return FaceDirection.Top;
                case FaceDirection.Front: return FaceDirection.Back;
                case FaceDirection.Back: return FaceDirection.Front;
            }
        }
        else if (axis == Vector3.left)
        {
            switch (current)
            {
                case FaceDirection.Top: return FaceDirection.Back;
                case FaceDirection.Bottom: return FaceDirection.Front;
                case FaceDirection.Right: return FaceDirection.Left;
                case FaceDirection.Left: return FaceDirection.Right;
                case FaceDirection.Front: return FaceDirection.Top;
                case FaceDirection.Back: return FaceDirection.Bottom;
            }
        }
        else if (axis == Vector3.right)
        {
            switch (current)
            {
                case FaceDirection.Top: return FaceDirection.Front;
                case FaceDirection.Bottom: return FaceDirection.Back;
                case FaceDirection.Right: return FaceDirection.Left;
                case FaceDirection.Left: return FaceDirection.Right;
                case FaceDirection.Front: return FaceDirection.Bottom;
                case FaceDirection.Back: return FaceDirection.Top;
            }
        }

        return current;
    }
}
