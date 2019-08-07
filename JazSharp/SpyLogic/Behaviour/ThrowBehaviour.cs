using System;

namespace JazSharp.SpyLogic.Behaviour
{
    internal class ThrowBehaviour : SpyBehaviourBase
    {
        private readonly Exception _exception;
        private readonly Type _exceptionType;

        internal ThrowBehaviour(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        internal ThrowBehaviour(Exception exception)
        {
            _exception = exception;
        }

        protected override void Execute(BehaviourArgs args)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            throw (Exception)Activator.CreateInstance(_exceptionType);
        }
    }
}
