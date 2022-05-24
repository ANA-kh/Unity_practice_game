using BehaviorDesigner.Runtime.Tasks;
using Core.Character;
using Core.Util;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    public class DestroyBoss : EnemyAction
    {
        public float BleedTime = 2.0f;
        public ParticleSystem BleedEffect;
        public ParticleSystem ExplodeEffect;

        private bool isDestroyed;

        public override void OnStart()
        {
            EffectManager.Instance.PlayOneShot(BleedEffect,transform.position);
            DOVirtual.DelayedCall(BleedTime, () =>
            {
                EffectManager.Instance.PlayOneShot(ExplodeEffect, transform.position);
                CameraController.Instance.ShakeCamera(0.7f);
                isDestroyed = true;
                Object.Destroy(gameObject);
            }, false);
        }

        public override TaskStatus OnUpdate()
        {
            return isDestroyed ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}