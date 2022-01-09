using System.Collections.Generic;

namespace CosmosContainerMigrationTool.Implementations.Normalization
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string inputString, Dictionary<string, object> setOfCharacters)
        {
            string resultString = inputString;
            Dictionary<string, object> specialCharacters = setOfCharacters;
            foreach (string item in specialCharacters.Keys)
            {
                _ = specialCharacters.TryGetValue(item, out object replacingBy);
                string rep = replacingBy?.ToString() ?? "";
                resultString = resultString.Replace(item.ToString(), rep);
            }
            return resultString;
        }
    }
}

