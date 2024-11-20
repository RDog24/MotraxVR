using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Scene_Change : MonoBehaviour
{
    public void ChangeCurls()
    {
        SceneManager.LoadScene("Curls");
    }
    public void ChangeShoulders()
    {
        SceneManager.LoadScene("Shoulders");
    }
    public void ChangePunch()
    {
        SceneManager.LoadScene("Punch");
    }

    public void ChangeMain()
    {
        SceneManager.LoadScene("Main");
    }
}