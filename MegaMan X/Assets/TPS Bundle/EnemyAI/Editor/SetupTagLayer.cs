﻿using UnityEditor;
using UnityEngine;

public class SetupTagLayer : Editor
{
	[MenuItem("GameObject/Enemy AI/ Setup Tag and Layers", false, 31)]
	static void Init()
	{
		GameObject go = Selection.activeGameObject;
		go.tag = "Enemy";
		go.layer = LayerMask.NameToLayer("Enemy");
		GameObject hips = go.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).gameObject;
		if (!hips.GetComponent<Collider>())
			hips = hips.transform.GetChild(0).gameObject;
		hips.layer = LayerMask.NameToLayer("Enemy");
		go.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		foreach (Transform child in go.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand))
		{
			Transform gunMuzzle = child.Find("muzzle");
			if (gunMuzzle != null)
			{
				child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				foreach (Transform part in child)
				{
					part.gameObject.layer = child.gameObject.layer;
				}
			}
		}
	}
}

