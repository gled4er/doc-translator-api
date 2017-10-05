namespace TestApp.DocumentManagement.Model
{
    /// <summary>
    /// Object holding links to original and translated documents
    /// </summary>
    public class DocumentLinks
    {
        /// <summary>
        /// SharePoint Link to the original document
        /// </summary>
        public string OriginalDocument { get; set; }

        /// <summary>
        /// SharePoint Link to the translated document
        /// </summary>
        public string TranslatedDocument { get; set; }
    }
}
