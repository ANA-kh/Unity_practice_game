using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsHealthUnder : EnemyConditional
    {
        public SharedInt HealthTreshold;

        public override TaskStatus OnUpdate()
        {
            return destructable.CurrentHealth < HealthTreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}