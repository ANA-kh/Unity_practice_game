using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class GotoNextStage : EnemyAction
    {
        public SharedInt CurrentStage;

        public override TaskStatus OnUpdate()
        {
            if (CurrentStage != null)
                CurrentStage.Value++;
            return TaskStatus.Success;
        }
    }
}