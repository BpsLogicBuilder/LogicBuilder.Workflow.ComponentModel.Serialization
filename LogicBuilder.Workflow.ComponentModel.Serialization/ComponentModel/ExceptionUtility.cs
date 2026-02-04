using System;
using System.Threading;

namespace LogicBuilder.Workflow.ComponentModel
{
    internal static class ExceptionUtility
    {
        internal static bool IsCriticalException(Exception ex)
        {
            return ex is OutOfMemoryException
                or ThreadAbortException
                or StackOverflowException
                or ThreadInterruptedException;
        }
    }
}
