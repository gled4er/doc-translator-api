namespace TranslatorApp.Model
{
    public class TranslatorRequest
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public string OriginalLanguage { get; set; }
        public string TranslationLanguage { get; set; }
    }
}
