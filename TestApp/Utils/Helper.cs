using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Utils
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
