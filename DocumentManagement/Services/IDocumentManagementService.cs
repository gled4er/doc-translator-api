using System.Threading.Tasks;
using DocumentManagement.Model;
namespace DocumentManagement.Services
{
    public interface IDocumentManagementService
    {
        /// <summary>
        /// Translates a document and provides links to the original as well as the translated document
        /// </summary>
        /// <param name="storageContainerName">Name of the storage container</param>
        /// <param name="storageFileName">Name of the storage file (original document for translation)</param>
        /// <param name="originalLanguage">The language of the original file</param>
        /// <param name="translationLanguage">The language for translating the document</param>
        /// <returns></returns>
        DocumentLinks TranslateFile(string storageContainerName, string storageFileName, string originalLanguage, string translationLanguage);
    }
}
