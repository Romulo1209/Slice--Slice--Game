using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Controlador de movimento da camera
    [SerializeField] [Range(0.01f, 1f)] float cameraFollowVelocity = 0.3f;

    [SerializeField] Transform knifePos;
    private Vector3 futurePos;
    private Vector3 offset;
    private Vector3 originalPos;

    public Transform KnifePos { set { knifePos = value; } }

    private void Awake() {
        originalPos = transform.position;
    }
    //Faz o setup da camera e gera um offset para ser seguido
    public void Setup() 
    {
        offset = Vector3.zero;
        offset = originalPos - knifePos.position;
        offset.y += 1;
        offset.z -= .5f;
    }
    private void LateUpdate()
    {
        FollowKnife();
    }
    //Segue a arma
    void FollowKnife()
    {
        if(knifePos != null) {
            futurePos = knifePos.position + offset;
            transform.position = Vector3.Lerp(transform.position, futurePos, cameraFollowVelocity);
        }
    }
}
