using System.Collections.Generic;

namespace CosmosContainerMigrationTool.Implementations.Normalization
{
    public static class SpecialCharacters
    {
        public static Dictionary<string, object> ReplacementChars = new Dictionary<string, object>
        {
            {"à", "a"},
            {"á", "a"},
            {"â", "a"},
            {"ä", "a"},
            {"é", "e"},
            {"è", "e"},
            {"ê", "e"},
            {"ë", "e"},
            {"ï", "i"},
            {"î", "i"},
            {"í", "i"},
            {"ì", "i"},
            {"ö", "o"},
            {"ô", "o"},
            {"ó", "o"},
            {"ò", "o"},
            {"ù", "u"},
            {"ú", "u"},
            {"û", "u"},
            {"ü", "u"},

            {"À", "a"},
            {"Á", "a"},
            {"Â", "a"},
            {"Ä", "a"},
            {"É", "e"},
            {"È", "e"},
            {"Ê", "e"},
            {"Ë", "e"},
            {"Ï", "i"},
            {"Î", "i"},
            {"Í", "i"},
            {"Ì", "i"},
            {"Ö", "o"},
            {"Ô", "o"},
            {"Ó", "o"},
            {"Ò", "o"},
            {"Ù", "u"},
            {"Ú", "u"},
            {"Û", "u"},
            {"Ü", "u"},

            {"-", null},
            {"*", null},
            {" ", null},
            {"/", null},
            {".", null},
            {"\\", null},
            {"+", null},
            {"#", null},
            {":", null},
            {"_", null},
            {"ß", "SS"}
        };

        public static Dictionary<string, object> ReplacementCharsReduced = new Dictionary<string, object>
        {
            {"-", null},
            {" ", null},
            {".", null},
            {"_", null}
        };
    }

    public enum SetOfCharacters
    {
        FULL,
        REDUCED
    }
}
