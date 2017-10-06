using System;

namespace DocumentManagement.Utils
{
    public static class Helper
    {
        public static string GetExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            var valuesArray = fileName.Split(new[] { "." }, StringSplitOptions.None);
            return valuesArray[valuesArray.Length - 1];
        }
    }
}
