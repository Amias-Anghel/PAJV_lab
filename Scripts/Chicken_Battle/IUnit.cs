using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit 
{
    public void SetUnitStats(float attackPower, int maxDistance, float specialPower, bool massHeal);
    public void ModifyLife(float power);
    public void Move(Vector3 position);

    public Vector3 GetPosition();
}
