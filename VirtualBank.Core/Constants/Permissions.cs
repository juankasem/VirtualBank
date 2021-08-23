using System.Collections.Generic;

namespace VirtualBank.Core.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermisiionList(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.VDelete"
            };
        }
    }
}