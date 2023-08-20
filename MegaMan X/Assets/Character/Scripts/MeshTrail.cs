using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{

    public Animator anim;
    public PlayerStateMachine stateMachine;


    [Header("Mesh Related")]
    public float meshRefreshRate;
    public float activeTime;
    public bool isTrailActive;

    private SkinnedMeshRenderer[] skinnedRenders;


    // Update is called once per frame
    void Update()
    {
        if (stateMachine.InputReader.isDashing && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActiveTrail(activeTime));
        }
    }

    IEnumerator ActiveTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedRenders == null)
                skinnedRenders = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i=0; i < skinnedRenders.Length; i++)
            {
                GameObject gObj = new GameObject();

                MeshRenderer mr =  gObj.AddComponent<MeshRenderer>();
                MeshFilter mf =  gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedRenders[i].BakeMesh(mesh);
                
                mf.mesh = mesh;
            }



            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }
}
