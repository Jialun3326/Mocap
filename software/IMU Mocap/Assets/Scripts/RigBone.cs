using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigBone {
    public GameObject gameBody;
    public HumanBodyBones gameBone;
    Animator animator;

    public RigBone(GameObject body, HumanBodyBones bone) {
        gameBody = body;
        gameBone = bone;
        animator = body.GetComponent<Animator>();

        if (animator == null) {
            Debug.LogError("No Animator");
            return;
        }

        Avatar avatar = animator.avatar;

        if (avatar == null || !avatar.isHuman || !avatar.isValid) {
            Debug.LogError("No Avatar");
            return;
        }
    }

    public void SetLocalRotation(Quaternion quaternion) {
        animator.GetBoneTransform(gameBone).localRotation = quaternion;
    }
}
