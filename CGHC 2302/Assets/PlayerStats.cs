using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public string presetName;

    //move
    public float maxSpeed;
    public float accel;
    public float decel;
    public float turnSpeed;

    //jump
    public float jumpHeight;
    public float timeToApex;

    public float upGravityMulti;
    public float downGravityMulti;
    public float jumpntGravityMulti;

    public float airAccel;
    public float airDecel;
    public float airTurnSpeed;

    //extra

    public bool variableJH;
    //public bool variableJHgrav;
    public int maxAirJumps;

    //
    public float coyoteTime;
    public float jumpBuffer;
    public float terminalVelocity;

}
