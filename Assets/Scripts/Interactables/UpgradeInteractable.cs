using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInteractable : Interactable
{
    [Tooltip("The body part this upgrade affects")]
    public BodyParts partAffected;
    [Tooltip("The type of upgrade. 0 is none")]
    public int type;
    [Tooltip("Whether the pickup is destroyed on pickup")]
    public bool destroyOnPickup;

    [HideInInspector] public enum BodyParts {HAND, HEART, INTESTINES, EYE, EAR};

    public override void Interact()
    {
        switch(partAffected)
        {
            case BodyParts.EAR:
                pc.earType = type;
                break;
            case BodyParts.EYE:
                pc.eyeType = type;
                break;
            case BodyParts.HAND:
                pc.handType = type;
                break;
            case BodyParts.HEART:
                pc.heartType = type;
                break;
            case BodyParts.INTESTINES:
                pc.intestinesType = type;
                break;
        }

        if(destroyOnPickup)
        {
            Destroy(gameObject, 0.01f);
        }
    }
}
