using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSequenceSO sequence;
    [SerializeField] private TutorialUI ui;

    private int index = 0;
    private float timer = 0f;

    void Start()
    {
        ShowStep(0);
    }

    void Update()
    {
        var step = sequence.steps[index];

        if (step.trigger == TutorialTriggerType.TimeElapsed)
        {
            timer += Time.deltaTime;
            if (timer >= step.time)
            {
                Next();
            }
        }
    }

    void ShowStep(int i)
    {
        timer = 0f;
        var step = sequence.steps[i];
        ui.Set(step.message, step.image, step.textimage);
        RegisterTrigger(step.trigger);
    }

    void RegisterTrigger(TutorialTriggerType t)
    {
        if (t == TutorialTriggerType.PressTrigger)
            InputObserver.OnTriggerPressed += Next;

        if (t == TutorialTriggerType.MoveToTarget)
            TargetObserver.OnReached += Next;

        if (t == TutorialTriggerType.HitTarget)
            ShootingObserver.OnHit += Next;

        if (t == TutorialTriggerType.CustomEvent)
            TutorialEvents.OnCustomEvent += Next;
    }

    void UnregisterAll()
    {
        InputObserver.OnTriggerPressed -= Next;
        TargetObserver.OnReached -= Next;
        ShootingObserver.OnHit -= Next;
        TutorialEvents.OnCustomEvent -= Next;
    }

    public void Next()
    {
        UnregisterAll();
        index++;

        if (index >= sequence.steps.Length)
        {
            ui.gameObject.SetActive(false);
            return;
        }

        ShowStep(index);
    }
}
