using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBlade : MonoBehaviour
{
    //Script Verificador de colisão com a lamina das armas

    [SerializeField] KnifeController knifeController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground") {
            if (knifeController.ActualRotationAngle > 105 && knifeController.ActualRotationAngle < 240) {
                knifeController.GroundTouch(collision.contacts[0].point);
            }
            else {
                knifeController.ThrowKnife(new Vector3(0, knifeController.JumpForce, 0));
            }
        }
        else if(collision.transform.tag == "Finish")
        {
            if(knifeController.ActualRotationAngle < 180) {
                knifeController.GroundTouch(collision.contacts[0].point);
                knifeController.gameController.FinishLevel((int)collision.gameObject.GetComponent<FinishMultiply>().Multiplier);
            }
            else {
                knifeController.ThrowKnife(new Vector3(0, knifeController.JumpForce, 0));
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Ground") {
            knifeController.State = States.RotatingFast;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Cuttable") {
            knifeController.CuttingObject(collision.gameObject);
        }
    }
}
