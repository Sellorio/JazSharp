using System.Reflection;
using System.Runtime.ExceptionServices;

namespace JazSharp.SpyLogic.Behaviour
{
    internal class CallThroughBehaviour : SpyBehaviourBase
    {
        protected override void Execute(BehaviourArgs args)
        {
            try
            {
                args.Result = args.ExactMethod.Invoke(args.Instance, args.Parameters);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
