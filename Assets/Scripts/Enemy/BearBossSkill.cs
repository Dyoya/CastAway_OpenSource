using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearBossSkill
{
    public string name;
    public float cooldown;
    public float currentCooldown;
    public int priority;

    public BearBossSkill(string name, float cooldown, int priority)
    {
        this.name = name;
        this.cooldown = cooldown;
        this.priority = priority;
        this.currentCooldown = 0;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if(currentCooldown > 0)
        {
            currentCooldown -= deltaTime;
        }
    }
    
    public bool IsReady()
    {
        return currentCooldown <= 0;
    }

    public void SetCooldown()
    {
        currentCooldown = cooldown;
    }
}
