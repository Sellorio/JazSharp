using System.Linq.Expressions;
using System.Reflection;

namespace JazSharp.Reflection
{
    internal static class ExpressionHelper
    {
        internal static MethodInfo GetMethodFromExpression(LambdaExpression expression)
        {
            var unary = (UnaryExpression)expression.Body;
            var specialMethodCall = unary.Operand;
            var constant = (ConstantExpression)specialMethodCall.GetType().GetProperty("Object").GetValue(specialMethodCall);
            return (MethodInfo)constant.Value;
        }

        internal static PropertyInfo GetPropertyFromExpression(LambdaExpression expression)
        {
            var member = (MemberExpression)expression.Body;
            return (PropertyInfo)member.Member;
        }
    }
}
