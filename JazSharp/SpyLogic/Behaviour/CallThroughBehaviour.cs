namespace JazSharp.SpyLogic.Behaviour
{
    internal class CallThroughBehaviour : SpyBehaviourBase
    {
        protected override void Execute(BehaviourArgs args)
        {
            args.Result = args.Spy.Method.Invoke(args.Instance, args.Parameters);
        }
    }
}
