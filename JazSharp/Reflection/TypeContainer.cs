using System;

namespace JazSharp.Reflection
{
    internal interface ITypeContainer
    {
        Type ClassType { get; }
        Type[] MethodGenericArguments { get; }
    }

    internal class TypeContainer0<TClass> : ITypeContainer
    {
        public Type ClassType => typeof(TClass);
        public virtual Type[] MethodGenericArguments => new Type[0];
    }

    internal class TypeContainer1<TClass, T1> : TypeContainer0<TClass>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1) };
    }

    internal class TypeContainer2<TClass, T1, T2> : TypeContainer1<TClass, T1>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2) };
    }

    internal class TypeContainer3<TClass, T1, T2, T3> : TypeContainer2<TClass, T1, T2>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3) };
    }

    internal class TypeContainer4<TClass, T1, T2, T3, T4> : TypeContainer3<TClass, T1, T2, T3>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
    }

    internal class TypeContainer5<TClass, T1, T2, T3, T4, T5> : TypeContainer4<TClass, T1, T2, T3, T4>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
    }

    internal class TypeContainer6<TClass, T1, T2, T3, T4, T5, T6> : TypeContainer5<TClass, T1, T2, T3, T4, T5>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
    }

    internal class TypeContainer7<TClass, T1, T2, T3, T4, T5, T6, T7> : TypeContainer6<TClass, T1, T2, T3, T4, T5, T6>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
    }

    internal class TypeContainer8<TClass, T1, T2, T3, T4, T5, T6, T7, T8> : TypeContainer7<TClass, T1, T2, T3, T4, T5, T6, T7>
    {
        public override Type[] MethodGenericArguments => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
    }
}
