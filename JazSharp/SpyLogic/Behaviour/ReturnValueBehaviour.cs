namespace JazSharp.SpyLogic.Behaviour
{
    internal class ReturnValueBehaviour : SpyBehaviourBase
    {
        private readonly object _returnValue;

        internal ReturnValueBehaviour(object returnValue)
        {
            _returnValue = returnValue;
        }

        protected override void Execute(BehaviourArgs args)
        {
            args.Result = _returnValue;
        }
    }
}
