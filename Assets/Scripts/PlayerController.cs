using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    #region INPUT HANDLING
    Vector3 INPUT { get { return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")); } }
    Vector3 NORMALINP { get { return new Vector3(INPUT.x, 0, INPUT.z).normalized; } }
    bool ISRUNNING { get { return Input.GetButton("Fire3"); } }
    #endregion
    #region SPEED SETTINGS
    [SerializeField] private Vector2 speedSettings;
    Vector2 targetSpeed, curSpeed; // HOLDS TARGET DIRECTION * SPEED
    

    Vector2 smoothVel;
    [SerializeField] float smoothSpeed;

    Vector3 velocity;
    
    #endregion
    CharacterController CC { get { return GetComponent<CharacterController>(); } }

    void Move(){
        #region SPEED SETTINGS        
        // U MAY HAVE TO USE MATHS FOR LOCAL SPACE, BUT YOU COULD POSSIBLY USE TRANSFORMPOINT FUNCTION WITH TRANSFORMS TO GET FORWARD VECTOR BY INPUT
        float dotProd = Vector3.Dot(transform.forward, NORMALINP);

        float x = ((ISRUNNING) ? ((dotProd > 0) ?
            (speedSettings.y * Mathf.Abs(dotProd)) : speedSettings.y / 2) :
            speedSettings.x)
        ;
        x = Mathf.Clamp(x, speedSettings.x, speedSettings.y) * NORMALINP.x;

        float z = ((ISRUNNING) ? ((dotProd > 0) ? 
            (speedSettings.y * Mathf.Abs(dotProd)) : speedSettings.y / 2) :
            speedSettings.x)
        ;
        z = Mathf.Clamp(z, speedSettings.x, speedSettings.y) * NORMALINP.z;

        targetSpeed = new Vector2(x, z);
        #endregion
        curSpeed = Vector2.SmoothDamp(curSpeed, targetSpeed, ref smoothVel, smoothSpeed);

        

        
        velocity = new Vector3(curSpeed.x, 0, curSpeed.y);
        #region GRAVITY
        velocity += Physics.gravity;
        #endregion

        CC.Move(velocity * Time.deltaTime);
    }

    private void Update(){
        Move();
    }
}
