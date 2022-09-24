using UnityEngine;
using System.Collections;
using System;
using CnControls;
public class Character_Movement : MonoBehaviour
{
    public static bool moving = true;

    Animator animator;

    public Camera TPSCamera;

    [System.Serializable]
    public class AnimationSettings
    {
        public string verticalVelocityFloat = "Forword";
        public string horizontalVelocityFloat = "Strafe";
    }
    [SerializeField]
    public AnimationSettings animations;


    [System.Serializable]
    public class OtherSettings
    {
        public float lookSpeed = 5.0f;
        public float lookDistance = 30.0f;
        public bool requireInputForTurn = true;
    }
    [SerializeField]
    public OtherSettings other;

    

    void Start()
    {
        animator = GetComponent<Animator>();
      
    }


    void Update()
    {
        


        if (Input.GetAxis("Horizontal")!=  0 || Input.GetAxis("Vertical") != 0 || CnInputManager.GetAxis("Horizontal") != 0 || CnInputManager.GetAxis("Vertical") != 0)
        {

            RotatePlayer();
        }

        if (moving)
        {
            animator.SetBool("setIdle", false);
            Animate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            Animate(CnInputManager.GetAxis("Vertical"), CnInputManager.GetAxis("Horizontal"));

        }
        else
        {
            animator.SetBool("setIdle", true);
        }





    }

    private void RotatePlayer()
    {
        Transform mainCamT = TPSCamera.transform;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
        transform.rotation = newRotation;
    }

    public void Animate(float forward,float strafe)
    {
        animator.SetFloat(animations.verticalVelocityFloat, forward);
        animator.SetFloat(animations.horizontalVelocityFloat, strafe);
 
    }



}
