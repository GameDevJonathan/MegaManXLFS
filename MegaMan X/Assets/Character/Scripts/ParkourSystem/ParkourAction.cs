using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    [SerializeField] string ObstacleTag;
    [SerializeField] float minHeight, maxHeight;
    [SerializeField] bool rotateToObstacle;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchEndTime;

    [field: SerializeField] public Quaternion TargetRotation { get; private set; }
    public Vector3 MatchPos { get; set; }

    public bool CheckIfPossible(ObstacleHitData hitdata, Transform player)
    {
        if (!string.IsNullOrEmpty(ObstacleTag) && hitdata.forwardHit.transform.tag != ObstacleTag)
            return false;
        Debug.Log("We passed the obstacle tag check");

        //height tag
        //float height = hitdata.heightHit.point.y - player.transform.position.y;
        float height = hitdata.heightHit.collider.bounds.size.y;
        Debug.Log("hitdata obstacle collider height: " + hitdata.heightHit.collider.bounds);
        Debug.Log("hitdata obstacle height: " + height);
        if(height < minHeight || height > maxHeight)
        {
            return false;
        }

        if (rotateToObstacle)
        {
            TargetRotation = Quaternion.LookRotation(-hitdata.forwardHit.normal);
        }

        if (enableTargetMatching)
        {
            MatchPos = hitdata.heightHit.point;
        }
        return true;
    }


    //exposed variable
    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchEndTime => matchEndTime;
}
