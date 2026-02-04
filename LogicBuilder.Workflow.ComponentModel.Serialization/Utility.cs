using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("LogicBuilder.Workflow.ComponentModel.Serialization.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010059b59302e7303accd5cc84fd482cae54dea8d8b8de7faaef37abbac4b08e3d91283087f48ae04c4fdd117752a3fcafcda61cd2099e2d5432b9bce70e5fe083b15e43cd652617b06dc1422d347ffe7b2aeb7b466e567c6988f26dccbf9723b4b57b1aeaa0a2dbd00478d7135da9bb04a6138d5f29e54ac7e9ac9ae3b7956cf6c2")]
namespace LogicBuilder.Workflow
{
    static class Utility
    {
        //[SuppressMessage("Reliability", "Reliability113", Justification = "These are the core methods that should be used for all other Guid(string) calls.")]
        internal static Guid CreateGuid(string guidString)
        {
            bool success = false;
            Guid result = Guid.Empty;

            try
            {
                result = new Guid(guidString);
                success = true;
            }
            finally
            {
                if (!success)
                {
                    System.Diagnostics.Debug.Assert(false, "Creation of the Guid failed.");
                }
            }

            return result;
        }

        //[SuppressMessage("Reliability", "Reliability113", Justification = "These are the core methods that should be used for all other Guid(string) calls.")]
        internal static bool TryCreateGuid(string guidString, out Guid result)
        {
            bool success = false;
            result = Guid.Empty;

            try
            {
                result = new Guid(guidString);
                success = true;
            }
            catch (ArgumentException)
            {
                // ---- this
            }
            catch (FormatException)
            {
                // ---- this
            }
            catch (OverflowException)
            {
                // ---- this
            }

            return success;
        }
    }
}
