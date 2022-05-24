using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Core.Character;
using Core.Combat;
using UnityEngine;

namespace Core.AI
{
    public class Shoot : EnemyAction
    {
        public Weapon weapon;
        public bool ShakeCamera;

        public override TaskStatus OnUpdate()
        {
            // foreach (var weapon in Weapons)
            // {
                var projectile = Object.Instantiate(weapon.projectilePrefab,weapon.weaponTransform.position ,
                    Quaternion.identity);
                projectile.Shooter = gameObject;
                
                var force = new Vector2(weapon.horizontalForce * transform.localScale.x,weapon.verticalForce);
                projectile.SetForce(force);

                if (ShakeCamera)
                    CameraController.Instance.ShakeCamera(0.5f);
            //}

            return TaskStatus.Success;
        }
    }
}