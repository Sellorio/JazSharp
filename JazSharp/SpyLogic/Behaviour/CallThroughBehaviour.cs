namespace JazSharp.SpyLogic.Behaviour
{
    internal class CallThroughBehaviour : SpyBehaviourBase
    {
        protected override void Execute(BehaviourArgs args)
        {
            args.Result = args.ExactMethod.Invoke(args.Instance, args.Parameters);
        }
    }
}
