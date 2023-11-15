using System.Collections.Generic;

namespace Parts
{
    internal interface IDatabase
    {
        string GenerateHash(byte[] fileContent);

        bool ApprovalCheck(IEnumerable<string> requiredHashesFromServer);

        string GetModuleDataForServer();
    }
}