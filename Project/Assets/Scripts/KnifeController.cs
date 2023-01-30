using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SlowZoneArea
{
    [Range(0, 360)] public float MinAngle;
    [Range(0, 360)] public float MaxAngle;
}
public enum States { RotatingSlow, RotatingFast, Stopped, StopRotating }

public class KnifeController : MonoBehaviour
{
    [Header("General Parameters")]
    [SerializeField] [Range(1, 15)] float jumpForce = 5;
    [SerializeField] [Range(1, 15)] float forwardForce = 1;
    [SerializeField] States state;
    
    [Header("Rotation Parameters")]
    [SerializeField] [Range(0, 360)] float actualRotationAngle;
    [SerializeField] [Range(0, 100)] float slowZoneRotationVelocity;
    [SerializeField] [Range(0, 800)] float fastZoneRotationVelocity;
    [SerializeField] SlowZoneArea slowZoneArea;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject groundParticle;
    public GameController gameController;

    //Privates
    float gravityModifier;
    bool flip;
    bool flipped;

    #region Getters & Setters

    public States State { get { return state; } set { state = value; } }
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
    public float ForwardForce { get { return forwardForce; } set { forwardForce = value; } }
    public float ActualRotationAngle {
        get {
            var actual = Mathf.Atan2(transform.forward.z, transform.forward.y) * Mathf.Rad2Deg;
            if (actual < 0) actual += 360;
            return actual;
        }
    }

    #endregion

    private void Update() {
        //Atualiza o angulo da arma e a gravidade
        actualRotationAngle = ActualRotationAngle;
        Gravity();

        //Caso o jogo esteja inciado o player pode se mover e fazer ações
        if (gameController.GameStarted) { 
            //Input para executar um pulo
            if (Input.GetMouseButtonDown(0)) {
                ThrowKnife(new Vector3(0, jumpForce, forwardForce));
            }

            //Verificador de estado para decidir se a arma faz um flip ou não
            if (state != States.Stopped && state != States.StopRotating) {
                if (!flip)
                    RotateObject();
                else
                    Flip();
            }

            //Limitador altitude para reinicar o jogo
            if (transform.position.y < -50 || transform.position.y > 50)
                gameController.SetupGame();
        }
    }

    #region Movement

    //Controlador de gravidade
    void Gravity()
    {
        rb.AddForce(new Vector3(0, -gravityModifier, 0));
        if (gravityModifier > 2)
            gravityModifier = 2;

        if (state != States.Stopped)
        {
            gravityModifier += Time.deltaTime / 6f;
            rb.useGravity = true;
        }
        else
        {
            gravityModifier = 0;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
           
    }

    //Executa um impulso na arma
    public void ThrowKnife(Vector3 position)
    {
        Flip();
        rb.velocity = Vector3.zero;
        rb.AddForce(position, ForceMode.Impulse);
    }

    #endregion

    #region Actions

    //Executa a rotação da arma caso esteja cortando algo
    public void CuttingObject(GameObject cutObject)
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        cutObject.GetComponent<CuttableObject>().Slice();
        if (actualRotationAngle > 120)
            state = States.StopRotating;
    }

    //Para a rotação caso toque no chão
    public void GroundTouch(Vector3 groundPos)
    {
        state = States.Stopped;
        rb.velocity = Vector3.zero;
        Instantiate(groundParticle, groundPos, Quaternion.Euler(-90, 0, 0));
    }

    #endregion

    #region Rotations

    void RotateObject()
    {
        //Verificador de Angulação
        if (actualRotationAngle > slowZoneArea.MinAngle && actualRotationAngle < slowZoneArea.MaxAngle) {
            //Verificador da zona de Lentidão
            SlowRotate();
        }
        else {
            //Verificador da zona de Velocidade
            Flip();
        }
    }
    //Executa o flip
    void Flip() {
        flip = true;
        FastRotate();

        if (!flipped) {
            flipped = actualRotationAngle > slowZoneArea.MinAngle && actualRotationAngle < slowZoneArea.MaxAngle ? false : true;
            return;
        }

        if (actualRotationAngle > slowZoneArea.MinAngle && actualRotationAngle < slowZoneArea.MaxAngle) {
            flipped = false;
            flip = false;
        }
    }

    //Estados de Rotação
    void FastRotate() {
        transform.Rotate(fastZoneRotationVelocity * Time.deltaTime, 0, 0);
        state = States.RotatingSlow;
    }
    void SlowRotate() {
        transform.Rotate(slowZoneRotationVelocity * Time.deltaTime, 0, 0);
        state = States.RotatingFast;
    }

    #endregion

    #region Cable Collisions

    //Verificador de Colisão do cado das armas
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            ThrowKnife(new Vector3(0, JumpForce, 0));
        }
        else if (collision.transform.tag == "Finish")
        {
            ThrowKnife(new Vector3(0, JumpForce, -ForwardForce));
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == "Cuttable") {
            ThrowKnife(new Vector3(0, JumpForce, -ForwardForce));
        }
    }

    #endregion
}