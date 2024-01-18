using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{

    public Animator anim;
    public PlayerStateMachine stateMachine;

    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public Transform positionToSpawn;
    public float meshDestroyDelay = 3f;
    public int maxClones = 4;



    [Header("Shader Related")]
    public Material mat;
    public bool isTrailActive;
    [SerializeField]
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
        int count = 3;
        //while (timeActive > 0)
        while (count > 0)
        {
            //timeActive -= meshRefreshRate;
            Debug.Log(count);

            if (skinnedRenders == null)
                skinnedRenders = GetComponentsInChildren<SkinnedMeshRenderer>();



            for (int i = 0; i < skinnedRenders.Length; i++)
            {
                //Debug.Log($"skinned mesh renderers:: {skinnedRenders[i]}");

                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedRenders[i].BakeMesh(mesh);

                mf.mesh = mesh;
                //mr.material = skinnedRenders[i].material;
                mr.material = mat;
                Destroy(gObj, meshDestroyDelay);
            }



            yield return new WaitForSeconds(meshRefreshRate);
            count--;
        }

        isTrailActive = false;
    }
}
