using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CircuitCover : NetworkBehaviour
{
    [SerializeField] private Animation leftCoverAnimation;
    [SerializeField] private Animation rightCoverAnimation;

    [ClientRpc]
    public void CircuitCoverOpeningAnimation()
    {
        rightCoverAnimation.Play("RightOpeningAnimation");
        leftCoverAnimation.Play("LeftOpeningAnimation");
    }

    [ClientRpc]
    public void CircuitCoverClosingAnimation()
    {
        rightCoverAnimation.Play("RightClosingAnimation");
        leftCoverAnimation.Play("LeftClosingAnimation");
    }
}
