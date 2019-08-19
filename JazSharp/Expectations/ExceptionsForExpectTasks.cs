using JazSharp.Expectations;
using System;
using System.Threading.Tasks;

// namespace intentionally altered so the consumer does not need to add another using
namespace JazSharp
{
    /// <summary>
    /// Extension methods to async methods with expectations on them to be tested
    /// without wrapping the Expect in an await.
    /// </summary>
    public static class ExceptionsForExpectTasks
    {
        /// <summary>
        /// An extension method to allow the test to call <see cref="ToThrow{TException}(Task{CallExpect})"/>
        /// on a <see cref="Task{TResult}"/> (<see cref="CallExpect"/>).
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="callExpect"></param>
        /// <returns></returns>
        public static async Task<TException> ToThrow<TException>(this Task<CallExpect> callExpect)
            where TException : Exception
        {
            return (await callExpect).ToThrow<TException>();
        }
    }
}
