using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject player;

    public float followSpeed = 5f;

    public float offsetX = -10f;
    public float offsetY = 5f;
    public float offsetZ = 0f;

    Vector3 _cameraPosition;

    private void LateUpdate()
    {
        _cameraPosition.x = player.transform.position.x + offsetX;
        _cameraPosition.y = player.transform.position.y + offsetY;
        _cameraPosition.z = player.transform.position.z + offsetZ;

        transform.position = Vector3.Lerp(transform.position, _cameraPosition, followSpeed * Time.deltaTime);
    }
}
