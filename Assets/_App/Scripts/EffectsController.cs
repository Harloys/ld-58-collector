using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public float BallsScaleMult = 1f;
    public int ClawsCount = 1;
    public float SpeedMult = 0f;

    public static EffectsController Instance;

    public float effectDuration = 60f;

    public UIEffectHelper UIHelper;

    private class ActiveEffect
    {
        public System.Action Apply;
        public System.Action Remove;
        public float AddedValue;
    }

    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    private List<System.Action> effects = new List<System.Action>();
    private List<System.Action> unusedEffects = new List<System.Action>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        effects.Add(EffectIncreaseBallScale);
        effects.Add(EffectIncreaseClaws);
        effects.Add(EffectIncreaseSpeed);

        unusedEffects.AddRange(effects);
    }

    public void AddRandomEffect()
    {
        if (unusedEffects.Count == 0)
            unusedEffects.AddRange(effects);

        int index = Random.Range(0, unusedEffects.Count);
        System.Action selectedEffect = unusedEffects[index];
        unusedEffects.RemoveAt(index);

        ActiveEffect activeEffect = ApplyEffect(selectedEffect);

        ShowUIForEffect(selectedEffect);

        StartCoroutine(RemoveEffectAfterTime(activeEffect));

    }

    private ActiveEffect ApplyEffect(System.Action effect)
    {
        ActiveEffect active = new ActiveEffect();
        active.Apply = effect;

        if (effect == EffectIncreaseBallScale)
        {
            float amount = 0.35f;
            BallsScaleMult += amount;
            active.AddedValue = amount;
        }
        else if (effect == EffectIncreaseClaws)
        {
            if (ClawsCount < 4)
            {
                ClawsCount += 1;
                active.AddedValue = 1;
            }
            else
                active.AddedValue = 0;
        }
        else if (effect == EffectIncreaseSpeed)
        {
            float amount = 0.025f;
            SpeedMult += amount;
            active.AddedValue = amount;
        }

        activeEffects.Add(active);
        return active;
    }

    private IEnumerator RemoveEffectAfterTime(ActiveEffect effect)
    {
        yield return new WaitForSeconds(effectDuration);

        if (effect.Apply == EffectIncreaseBallScale)
        {
            BallsScaleMult -= effect.AddedValue;
            BallsScaleMult = Mathf.Max(BallsScaleMult, 1f);
        }
        else if (effect.Apply == EffectIncreaseClaws)
        {
            ClawsCount -= (int)effect.AddedValue;
            ClawsCount = Mathf.Max(ClawsCount, 1);
        }
        else if (effect.Apply == EffectIncreaseSpeed)
        {
            SpeedMult -= effect.AddedValue;
        }

        activeEffects.Remove(effect);
    }

    private void EffectIncreaseBallScale() { }
    private void EffectIncreaseClaws() { }
    private void EffectIncreaseSpeed() { }

    private void ShowUIForEffect(System.Action effect)
    {
        if (UIHelper == null) return;

        if (effect == EffectIncreaseBallScale) UIHelper.ShowBalls();
        else if (effect == EffectIncreaseClaws) UIHelper.ShowClaws();
        else if (effect == EffectIncreaseSpeed) UIHelper.ShowSpeed();
    }
}
