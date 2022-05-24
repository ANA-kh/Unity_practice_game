
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.animState = 0;
        enemy.SwichPoint();
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            enemy.animState = 1;
            enemy.MoveToTarget();
        }

        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x) < 0.05f)
        {
            enemy.TransitionToState(enemy.patrolState);
        }

        if (enemy.attackList.Count > 0)
            enemy.TransitionToState(enemy.attackState);
    }
}


public class AttackState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.animState = 2;
        enemy.targetPoint = enemy.attackList[0];
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (enemy.attackList.Count == 0)
            enemy.TransitionToState(enemy.patrolState);
        else if (enemy.attackList.Count > 0)
        {
            enemy.attackList.Sort( Compare);
            enemy.targetPoint = enemy.attackList[0];
        }
        if(enemy.hasBomb)
            return;
        
        if (enemy.targetPoint.CompareTag("Player"))
            enemy.AttackAction();
        if (enemy.targetPoint.CompareTag("Bomb"))
            enemy.SkillAction();

        enemy.MoveToTarget();
    }

    public static int Compare(Transform target1, Transform target2)
    {
        return target1.position.x.CompareTo(target2.position.x);
    }

}
