
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Animation Profile", menuName = "ScriptableObjects/Procedural Animation/Animation Profile")]
public class AnimationProfile : ScriptableObject{
    #region HEIGHT SETTINGS
    [Serializable] public class HeightSettings{
        [Tooltip("How high the leg will raise")] public Vector2 stepHeightMultiplier = new Vector2(.25f,.4f);
        [Tooltip("How the leg will move")] public AnimationCurve stepHeight;
    }
    public HeightSettings heightSettings;
    #endregion
    #region BOBBLE SETTINGS
    [Serializable] public class BobbleSettings{
        [Tooltip("If disabled, the body will stay still")] public bool bobbleTorso;
        [Tooltip("This height is added onto the average of the leg height")] public float torsoYOffset = 0;
    }
    public BobbleSettings bobbleSettings;
    #endregion
    #region RAYCAST SETTINGS
    [Serializable] public class RaycastSettings{
        [Tooltip("Length of the raycast being drawn")] public float rayDistance = .1f;
        [Tooltip("Scale this to the size of the foot to stop phasing")] public float scanningRadius;
        [Tooltip("Tick all but the characters layer")] public LayerMask layersToCollide;
    }
    public RaycastSettings raycastSettings;
    #endregion
    #region MOVE SETTINGS
    [Serializable] public class MoveSettings{
        public Vector2 distancePerStep;
        [Tooltip("This multiplies the distance away from character")] public Vector2 speedMultiplier;
    }
    public MoveSettings moveSettings;

    #endregion
    #region JUMP SETTINGS
    [Serializable] public class JumpSettings{
        [Tooltip("If enabled, the COM will take an impact when landing")] public bool hasImpact;
        [Tooltip("Min and Max bobble the COM will take when landing")] public Vector2 impactVector;
    }
    public JumpSettings jumpSettings;
    #endregion
    #region IDLE SETTINGS
    [Serializable] public class IdleSettings{
        [Tooltip("If enabled, the legs will return to sleeping position after x time")] public bool hasIdle;
        [Tooltip("Time it'll take until the script thinks the character is idling")] public float timeTillIdle;
        [Tooltip("The difference in angle between COM and feet to trigger a step")] public float idleRotationToTrigStep;

        [Tooltip("If enabled, the COM will bob up and down when idle")] public bool idleBounce;
        [Tooltip("X = breathe in, Y = breathe out")] public Vector2 breatheInOutSpeed;
        [Tooltip("Height the torso will raise when 'breathing in'")] public float torsoOffset;
    }
    public IdleSettings idleSettings;
    #endregion
    #region LEAN SETTINGS
    [Serializable] public class LeanSettings{
        [Tooltip("If enabled, the COM will lean towards velocity direction")] public bool doesLean;
        [Tooltip("Smaller the number, bigger the tilt")] public float leanIntensity;
        [Tooltip("Smalelr the number, more responsive it is")] public float leanSmoothSpeed;
    }
    public LeanSettings leanSettings;
    #endregion
}
