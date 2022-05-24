using BehaviorDesigner.Runtime.Tasks;
using Core.Combat;
using UnityEngine;

namespace Core.AI
{
    public class SpawnMaggot : EnemyAction
    {
        public GameObject MaggotPrefab;
        public Transform MaggotTransform;
        public GameObject HazardCollider;

        private Destructable maggot;

        public override void OnStart()
        {
            base.OnStart();
            maggot = Object.Instantiate(MaggotPrefab, MaggotTransform).GetComponent<Destructable>();
            maggot.transform.localPosition = Vector3.zero;
            destructable.Invincible = true;
            HazardCollider.SetActive(false);
        }

        public override TaskStatus OnUpdate()
        {
            if (maggot.CurrentHealth > 0) return TaskStatus.Running;

            destructable.Invincible = false;
            HazardCollider.SetActive(true);
            return TaskStatus.Success;
        }
    }
}