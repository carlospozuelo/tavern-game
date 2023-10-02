using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

        List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        /*
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains("_0_")) // All clothes
            {
                // anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, test));
            }
        }
        */

        aoc.ApplyOverrides(anims);
        animator.runtimeAnimatorController = aoc;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
