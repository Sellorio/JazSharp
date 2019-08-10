using Mono.Cecil;

namespace JazSharp.Reflection
{
    static class CecilExtensions
    {
        public static MethodReference ChangeHost(this MethodReference self, TypeReference typeReference)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, typeReference)
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
        }
    }
}
