using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<TutorialObject> steps;
    [SerializeField] private CtrTutorialUI ui;
    private int currentStep = 0;

    void Start()
    {
        StartTutorilal();
    }

    public void StartTutorilal()
    {
        currentStep = 0;
        ShowCurrentStep();
    }

    public void ShowCurrentStep()
    {
        if (currentStep >= steps.Count)
        {
            ui.Hide();
            Debug.Log("チュートリアル終了！");
            return;
        }

        var step = steps[currentStep];
        ui.ShowStep(step);

        if (step.textSpeed > 0)
            StartCoroutine(AutoNext(step.textSpeed));
 
    }

    private IEnumerator AutoNext(float delay)
    {



        yield return null;
    }
}
