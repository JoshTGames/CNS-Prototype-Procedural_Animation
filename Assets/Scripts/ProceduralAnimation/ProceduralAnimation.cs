using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Velocity))]
public class ProceduralAnimation : MonoBehaviour{
    // THIS IS A RE-FACTORED SCRIPT 
    // IDEAS: 
    /*
     * Add impact effect to torso when landing (WILL NEED AN IS GROUNDED FUNCTION)
    */

    Velocity velocity { get { return GetComponent<Velocity>(); } }
    #region GIZMOS
    private void OnDrawGizmosSelected(){
        if(legs.Length <= 0){ return; }
        if (!animationProfile){
            Debug.LogWarning($"Please assign an animationProfile on {transform.name}");
            return;
        }
        if (!prerequisiteSettings.COM){
            Debug.LogWarning($"Please fill the prerequisiteSettings on {transform.name}");
            return;
        }
        foreach (Leg curLeg in legs){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((curLeg.restingPos != Vector3.zero) ? prerequisiteSettings.COM.position + curLeg.restingPos : curLeg.legOBJ.position, animationProfile.raycastSettings.scanningRadius);

            if (Application.isPlaying){
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(curLeg.desiredPos, animationProfile.raycastSettings.scanningRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(curLeg.targetPos, animationProfile.raycastSettings.scanningRadius);
            }            
        }
        Debug.DrawLine(prerequisiteSettings.COM.position, prerequisiteSettings.COM.position + prerequisiteSettings.COM.forward);
    }
    #endregion
    #region SETTINGS
    [SerializeField] private AnimationProfile animationProfile;

    [Serializable] private class Prerequisites {
        public Collider parentCollider;
        [Tooltip("Typically the root bone")] public Transform COM;
    }
    [SerializeField] private Prerequisites prerequisiteSettings;

    [Serializable] private class Leg{
        public string name;
        public Transform legOBJ;
        #region HIDDEN
        [HideInInspector] public Vector3 sleepPos; // THIS IS THE ACTUAL READER    
        [HideInInspector] public Vector3 offsetPos; // USED TO CALCULATE THE POSITION OF THE DESIRED POS REALTIME

        [HideInInspector] public Vector3 desiredPos, targetPos; // DESIRED POSITION IS THE POSITION THE LEG WILL TRY AND GET TOWARDS, TARGET POS IS A LOCKED DESIRED POS
        [HideInInspector] public Quaternion targetRot; // WILL TAKE TORSO ROTATION
        [HideInInspector] public bool isPlanted, isIdle;
        #endregion

        [Header("If any below is left default, it'll be set automatically")]
        public Vector3 restingPos; // STARTING POS
        
        #region LEG FUNCTIONS
        public void Step(float lastStep, float stepDuration, AnimationCurve stepHeight, float stepHeightMultiplier){ // THIS IS CALLED EVERY FRAME TO MAKE SURE THE LEG IS PLANTED ON THE TARGET POSITION
            float stepCompletion = Mathf.Clamp01((Time.time - lastStep) / stepDuration);
            legOBJ.position = Vector3.Lerp(legOBJ.position, targetPos, stepCompletion) + ((!isPlanted && !isIdle) ? Vector3.up * stepHeight.Evaluate(stepCompletion) * stepHeightMultiplier : Vector3.zero);
            legOBJ.rotation = Quaternion.Lerp(legOBJ.rotation, targetRot, stepCompletion);
        }
        #endregion
    }
    [SerializeField] private Leg[] legs;

    [Tooltip("This is used to position vector closer to the target position")][SerializeField] private float desiredPositionOffset = 1;
    [Serializable] private enum Space{
        LocalSpace,
        WorldSpace
    }
    [SerializeField] private Space rotationSpace; // THIS WILL AFFECT HOW THE DIRECTIONAL VECTORS ROTATE
    [Tooltip("Greater the number, the smoother the transition")][SerializeField] private float velocitySmoothing;
    #region HIDDEN -- MOST THESE ARE DYNAMIC VALUES AND CHANGE IN REALTIME    
    float lastStep;
    float stepDuration = 1f;
    Vector3 previousPosition = Vector3.zero;
    float timeBTWStep = 1f;
    float actualDistBTWStep = .05f;
    float actualSpeedMultiplier = 1.5f;
    float stepHeightMultiplier = .25f;
    int legIndex = 0; // LEG TO MOVE

    float curIdleTime;
    float velSmoother, curVel;

    Vector3 curLean, curLeanVel;

    bool breatheIn;
    float targetBobble;
    Vector3 bobbleVel;
    #endregion
    #endregion




    #region FUNCTIONS
    void Calibrate(){
        #region APPLYING DEFAULTS
        foreach (Leg curLeg in legs){
            curLeg.sleepPos = (curLeg.restingPos != Vector3.zero) ? prerequisiteSettings.COM.position + curLeg.restingPos : curLeg.legOBJ.position;
            curLeg.offsetPos = curLeg.sleepPos - prerequisiteSettings.COM.position;
        }

        for (int i = 0; i < legs.Length; i++){ // CALIBRATION
            CalculateStep(i, Vector3.zero);
        }
        #endregion
    }

    Vector3 GetAvgLegHeight(){
        Vector3 totalVector = Vector3.zero;
        foreach (Leg curLeg in legs){
            totalVector += curLeg.legOBJ.position;
        }
        totalVector.x = 0;
        totalVector.z = 0;
        return totalVector / legs.Length;
    }
    void BobbleTorso(float avgLegHeight, float curIdleTime){
        Vector3 newPos = Vector3.zero;
        Vector3 pos = prerequisiteSettings.COM.position;
        if (animationProfile.idleSettings.idleBounce) {


            #region BREATHE HANDLING
            targetBobble = avgLegHeight + animationProfile.bobbleSettings.torsoYOffset; // UPDATES THE DEFAULT PART OF THE CALCULATION
            if (!breatheIn) { targetBobble += animationProfile.idleSettings.torsoOffset; } // MEANS THAT THE DEFAULT POSITION IS ALWAYS BEING CALCULATED

            float dif = Mathf.Abs(prerequisiteSettings.COM.position.y - targetBobble);
            if (dif <= 0.01f && breatheIn){                
                breatheIn = false;
            }
            else if (dif <= 0.01f && !breatheIn){                
                breatheIn = true;
            }            
            #endregion



            #region ACTUAL POSITION TRANSITIONING
            if (curIdleTime > 0 && !legs[legIndex].isIdle){
                newPos = Vector3.SmoothDamp(pos, new Vector3(pos.x, avgLegHeight + animationProfile.bobbleSettings.torsoYOffset, pos.z), ref bobbleVel, 0);
            }
            else if (animationProfile.idleSettings.idleBounce){
                newPos = Vector3.SmoothDamp(pos, new Vector3(pos.x, targetBobble, pos.z), ref bobbleVel, (breatheIn) ? animationProfile.idleSettings.breatheInOutSpeed.x : animationProfile.idleSettings.breatheInOutSpeed.y);
            }
            #endregion
        }
        else { newPos = new Vector3(pos.x, avgLegHeight + animationProfile.bobbleSettings.torsoYOffset, pos.z); }

        
        


        prerequisiteSettings.COM.position = newPos;
    }

    void TiltTorso(Vector3 velocity){
        curLean = Vector3.SmoothDamp(curLean, velocity, ref curLeanVel, animationProfile.leanSettings.leanSmoothSpeed);
        prerequisiteSettings.COM.rotation = Quaternion.FromToRotation(transform.up, Vector3.up * animationProfile.leanSettings.leanIntensity + curLean) * transform.rotation;
    }
    void ClampFoot(Leg leg){
        float min = (prerequisiteSettings.parentCollider.bounds.center.y - prerequisiteSettings.parentCollider.bounds.extents.y) - .1f;
        float max = (prerequisiteSettings.parentCollider.bounds.center.y - prerequisiteSettings.parentCollider.bounds.extents.y) + (stepHeightMultiplier * 2);

        leg.legOBJ.position = new Vector3(leg.legOBJ.position.x, Mathf.Clamp(leg.legOBJ.position.y, min, max), leg.legOBJ.position.z);
    }
    void CalculateMoveSettings(Vector3 velocity){        
        actualDistBTWStep = Mathf.Clamp(animationProfile.moveSettings.distancePerStep.y * velocity.magnitude, animationProfile.moveSettings.distancePerStep.y, animationProfile.moveSettings.distancePerStep.x);
        actualSpeedMultiplier = Mathf.Clamp(animationProfile.moveSettings.speedMultiplier.y * curVel, animationProfile.moveSettings.speedMultiplier.y, animationProfile.moveSettings.speedMultiplier.x);

    }
    void CalculateStepHeight(Vector3 velocity){
        stepHeightMultiplier = Mathf.Clamp(animationProfile.heightSettings.stepHeightMultiplier.y * velocity.magnitude, animationProfile.heightSettings.stepHeightMultiplier.x, animationProfile.heightSettings.stepHeightMultiplier.y);
    }
    void CalculateStep(int index, Vector3 direction, float speed = 1){ // THIS DOESNT MOVE THE LEG BUT SIMPLY CALCULATES FOR THE NEXT STEP
        legs[index].desiredPos = (prerequisiteSettings.COM.TransformPoint(((rotationSpace == Space.LocalSpace) ? direction * speed : Vector3.zero) + legs[index].offsetPos * desiredPositionOffset) + ((rotationSpace == Space.WorldSpace) ? direction * speed : Vector3.zero)); // ROTATES THE OFFSETS BASED ON COM ROTATION

        RaycastHit hit;
        if (Physics.SphereCast(legs[index].desiredPos + new Vector3(0, stepHeightMultiplier, 0), animationProfile.raycastSettings.scanningRadius, Vector3.down, out hit, animationProfile.raycastSettings.rayDistance, animationProfile.raycastSettings.layersToCollide, QueryTriggerInteraction.Ignore)) { 
            legs[index].targetPos = hit.point + new Vector3(0, animationProfile.raycastSettings.scanningRadius);
            legs[index].targetRot = Quaternion.LookRotation(prerequisiteSettings.COM.forward, Vector3.up);
        }
        else { legs[index].targetPos = prerequisiteSettings.COM.TransformPoint(legs[index].offsetPos * desiredPositionOffset); } // IF NO GROUND IS FOUND
        lastStep = Time.time;
    }
    void StepHandler(Vector3 velocity){
        timeBTWStep = actualDistBTWStep / velocity.magnitude;
        curVel = Mathf.SmoothDamp(curVel, velocity.magnitude, ref velSmoother, velocitySmoothing);

        if(Time.time > lastStep + (timeBTWStep / legs.Length)){
            legs[legIndex].isIdle = false;
            curIdleTime = animationProfile.idleSettings.timeTillIdle;

            legs[legIndex].isPlanted = true;
            legIndex = (legIndex + 1) % legs.Length; // Smart way of making the value return to 0 after exceeding the leg length

            stepDuration = Mathf.Min(.5f, timeBTWStep / 2f);

            CalculateStep(legIndex, velocity.normalized, curVel * actualSpeedMultiplier);
            legs[legIndex].isPlanted = false;
        }
    }
    void ResetLegs(Vector3 velocity){
        timeBTWStep = actualDistBTWStep / velocity.magnitude;
        curVel = Mathf.SmoothDamp(curVel, velocity.magnitude, ref velSmoother, velocitySmoothing);

        if (Time.time > lastStep && !legs[legIndex].isIdle){
            legs[legIndex].isIdle = true;
            curIdleTime = animationProfile.idleSettings.timeTillIdle / 5;

            legs[legIndex].isPlanted = true;
            legIndex = (legIndex + 1) % legs.Length; // Smart way of making the value return to 0 after exceeding the leg length

            stepDuration = Mathf.Min(.5f, timeBTWStep / 2f);

            CalculateStep(legIndex, velocity.normalized, curVel * actualSpeedMultiplier);
            legs[legIndex].isPlanted = false;

        }
        else if (Time.time > lastStep && AngleDIFFeetAndCom(legIndex) >= animationProfile.idleSettings.idleRotationToTrigStep){
            curIdleTime = animationProfile.idleSettings.timeTillIdle / 5;

            legs[legIndex].isPlanted = true;
            legIndex = (legIndex + 1) % legs.Length; // Smart way of making the value return to 0 after exceeding the leg length

            stepDuration = Mathf.Min(.5f, timeBTWStep / 2f);

            CalculateStep(legIndex, velocity.normalized, curVel * actualSpeedMultiplier);
            legs[legIndex].isPlanted = false;
        }
    } // USED IN IDLING
    float AngleDIFFeetAndCom(int index){        
        return Vector3.Angle(legs[index].legOBJ.forward, prerequisiteSettings.COM.forward);
    }
    #endregion







    bool hasInitiated; // SAFETY CHECK
    private void Awake(){
        Calibrate();
        hasInitiated = true;
    }

    private void Update(){
        if (!hasInitiated) { return; }
        curIdleTime -= (animationProfile.idleSettings.hasIdle && curIdleTime >0) ? Time.deltaTime : 0;      


        foreach (Leg curLeg in legs){            
            curLeg.Step(lastStep, stepDuration, animationProfile.heightSettings.stepHeight, stepHeightMultiplier);
            ClampFoot(curLeg);
        }

        float avgLegHeight = GetAvgLegHeight().y;
        if (animationProfile.bobbleSettings.bobbleTorso) { BobbleTorso(avgLegHeight, curIdleTime); }
    }

    private void FixedUpdate(){
        #region CALCULATE VELOCITY
        Vector3 vel = velocity.value;
        vel.y = 0;
        #endregion
        

        StepHandler(vel.normalized);
        CalculateMoveSettings(vel/10);
        CalculateStepHeight(vel.normalized);

        if (curIdleTime <= 0 && animationProfile.idleSettings.hasIdle && vel.magnitude == 0){
            ResetLegs(vel.normalized);
        }

        if (animationProfile.leanSettings.doesLean){
            TiltTorso(vel/10);
        }
    }
}
