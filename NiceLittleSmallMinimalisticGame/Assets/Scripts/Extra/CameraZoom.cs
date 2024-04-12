using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    Camera cam;
    public List<Transform> targets;
    PlayerInput[] players;

    [SerializeField]
    int maxTargets;

    [Space]
    [Header("Movement and Zoom")]
    [SerializeField]
    float zoomSpeed;
    public Vector3 offset;   

    private void Start()
    {
        cam = this.GetComponent<Camera>();
    }

    private void Update()
    {
        if (targets.Count < maxTargets)
        {
            players = GameObject.FindObjectsOfType<PlayerInput>();
            foreach (PlayerInput player in players)
            {
                Transform playerPos = player.GetComponent<Transform>();

                if (!targets.Contains(playerPos))
                {
                    targets.Add(playerPos);
                }
            }
        }
        else if (targets.Count == 0)
        {
            Debug.Log("No one for the camera to follow");
            return;
        }
        else
        {
            Move();
            Zoom();
        }
    }

    void Move()
    { 
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = newPosition;
    }

    void Zoom()
    {
        if (GetGreatestDistance() <= 4)
        {
            float size = Mathf.MoveTowards(cam.orthographicSize, 4.5f, zoomSpeed * Time.deltaTime);
            cam.orthographicSize = size;
        }
        else if (GetGreatestDistance() <= 6)
        {
            float size = Mathf.MoveTowards(cam.orthographicSize, 5.625f, zoomSpeed * Time.deltaTime);
            cam.orthographicSize = size;
        }
        else if (GetGreatestDistance() <= 8)
        {
            float size = Mathf.MoveTowards(cam.orthographicSize, 7.5f, zoomSpeed * Time.deltaTime);
            cam.orthographicSize = size;
        }
        else if (GetGreatestDistance() <= 12)
        {
            float size = Mathf.MoveTowards(cam.orthographicSize, 11.25f, zoomSpeed * Time.deltaTime);
            cam.orthographicSize = size;
        }
    }
    // 45 -- 22.5 -- 11.25 -- 7.5 -- 5.625 -- 4.5 -- 3.75

    Vector2 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].transform.position;
        }
        else
        {
            var bounds = new Bounds(targets[0].transform.position, Vector2.zero);

            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].transform.position);
            }

            return bounds.center;
        }
    }

    float GetGreatestDistance()
    {
        if (targets.Count == 1)
        {
            return 0;
        }
        else 
        { 
            var bounds = new Bounds(targets[0].transform.position, Vector2.zero);

            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].transform.position);
            }

            return bounds.size.magnitude;
        }
    }
}
