using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class FacePlayer : EnemyAction
    {
        private float _baseScaleX;

        public override void OnAwake()
        {
            base.OnAwake();
            _baseScaleX = transform.localScale.x;
        }

        public override TaskStatus OnUpdate()
        {
            var scale = transform.localScale;
            scale.x = transform.position.x > player.transform.position.x ? -_baseScaleX : _baseScaleX;
            transform.localScale = scale;

            return TaskStatus.Success;
        }
    }
}