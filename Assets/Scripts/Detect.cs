using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    [HideInInspector] public Observe observeScript;
    private Animator animator;
    private bool pause = false;
    private bool isIdle = true;
    private bool hasPaused = false;

    private Observe[] observeScripts;

    void Start()
    {
        animator = GetComponent<Animator>();
        observeScripts = FindObjectsOfType<Observe>();
    }

    void Update()
    {
        if (!pause)
        {
            CheckAnimationProgress();
            if (Input.anyKeyDown)
            {
                Detect_Input();
            }
        }
    }

    void CheckAnimationProgress()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Base Layer

        if (!stateInfo.IsName("Idle"))
        {
            foreach (Observe observeScript in observeScripts)
            {
                //observeScript.CheckJointAngles();
            }

            //if (stateInfo.IsName("Air Squat"))
            //{
            //    float progress = stateInfo.normalizedTime % 1;
            //    if (progress > 0.41f && progress < 0.42f && !hasPaused)
            //    {
            //        animator.speed = 0f;
            //        pause = true;
            //        hasPaused = true;
            //        StartCoroutine(PauseAnimation(2f));
            //    }
            //    if (progress > 0.42f)
            //    {
            //        hasPaused = false;
            //    }
            //}
        }
    }

    public void pause_animation()
    {
        animator.speed = 0f;
    }

    public void resume_animation()
    {
        animator.speed = 1.0f;
    }

    void Detect_Input()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Check!!");
            SetExercise("Punch");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetExercise("Bicep");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetExercise("Kettle");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetAllExercises();
        }
    }

    public void Set_Punch()
    {
        SetExercise("Punch");
    }

    public void Set_Bicep()
    {
        SetExercise("Bicep");
    }

    public void Set_Kettle()
    {
        SetExercise("Kettle");
    }

    public void Reset_all()
    {
        ResetAllExercises();
    }

    void SetExercise(string exercise)
    {
        animator.SetBool("Punch", false);
        animator.SetBool("Bicep", false);
        animator.SetBool("Kettle", false);

        animator.SetBool(exercise, true);
        isIdle = false;
    }

    void ResetAllExercises()
    {
        isIdle = true;
        animator.SetBool("Punch", false);
        animator.SetBool("Bicep", false);
        animator.SetBool("Kettle", false);
    }

    private IEnumerator PauseAnimation(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
        animator.speed = 1f;
        pause = false;
    }
}
//        Debug.Log("Check!!");