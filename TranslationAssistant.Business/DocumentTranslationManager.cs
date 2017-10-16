// // ----------------------------------------------------------------------
// // <copyright file="DocumentTranslationManager.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>DocumentTranslationManager.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;

    using TranslationAssistant.TranslationServices.Core;

    using Comment = DocumentFormat.OpenXml.Spreadsheet.Comment;

    #endregion

    /// <summary>
    ///     The document translation manager.
    /// </summary>
    public class DocumentTranslationManager
    {
        #region Public Properties

        #endregion
        #region Public Methods and Operators

        /// <summary>
        ///     Do the translation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDir">The is dir.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target langauge.</param>
        public static void DoTranslation(string path, bool isDir, string sourceLanguage, string targetLanguage)
        {
            GetAllDocumentsToProcess(path, targetLanguage)
                .ForEach(t => DoTranslationInternal(t, sourceLanguage, targetLanguage));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The do translation internal.
        /// </summary>
        /// <param name="fullNameForDocumentToProcess">The full name for document to process.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target langauge.</param>
        private static void DoTranslationInternal(
            string fullNameForDocumentToProcess,
            string sourceLanguage,
            string targetLanguage)
        {
            try
            {
                if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".docx"))
                {
                    ProcessWordDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
                else if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".xlsx"))
                {
                    ProcessExcelDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
                else if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".pptx"))
                {
                    ProcessPowerPointDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
                else if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".txt") || fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".text"))
                {
                    ProcessTextDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
                else if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".html") || fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".htm"))
                {
                    ProcessHTMLDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
                else if (fullNameForDocumentToProcess.ToLowerInvariant().EndsWith(".srt"))
                {
                    ProcessSRTDocument(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
                }
            }
            catch (AggregateException ae)
            {
                var errorMessage = String.Empty;
                foreach (var ex in ae.InnerExceptions)
                {
                    errorMessage = errorMessage + " " + ex.Message;
                    LoggingManager.LogError(string.Format("{0}:{1}", fullNameForDocumentToProcess, ex.Message + ex.StackTrace));
                }

                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                LoggingManager.LogError(
                    string.Format("{0}:{1}", fullNameForDocumentToProcess, ex.Message + ex.StackTrace));
                throw;
            }
        }

        /// <summary>
        ///     Gets all documents to process.
        /// </summary>
        /// <param name="documentPath">The document path.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>All documents to process.</returns>
        private static List<string> GetAllDocumentsToProcess(string documentPath, string targetLanguage)
        {
            var allFiles = new List<string>();
            File.Delete(GetOutputDocumentFullName(documentPath, targetLanguage));
            var outputDocumentName = GetOutputDocumentFullName(documentPath, targetLanguage);

            if (documentPath.ToLowerInvariant().EndsWith(".pdf"))
            {
                // TBD - add logic for dealing with pdf-s
            }
            else
            {
                File.Copy(documentPath, outputDocumentName);
                allFiles.Add(outputDocumentName);
            }

            return allFiles;
        }

        /// <summary>
        ///     The get output document full name.
        /// </summary>
        /// <param name="documentFullName">The document full name.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>
        ///     The System.String.
        /// </returns>
        private static string GetOutputDocumentFullName(string documentFullName, string targetLanguage)
        {
            var outputDocumentNameWithoutExtension = documentFullName.Substring(0, documentFullName.LastIndexOf(".", StringComparison.Ordinal))
                                                     + "." + TranslationServiceFacade.LanguageNameToLanguageCode(targetLanguage);
            if (documentFullName.ToLowerInvariant().EndsWith(".xls") || documentFullName.ToLowerInvariant().EndsWith(".xlsx"))
            {
                return outputDocumentNameWithoutExtension + ".xlsx";
            }

            if (documentFullName.ToLowerInvariant().EndsWith(".ppt") || documentFullName.ToLowerInvariant().EndsWith(".pptx"))
            {
                return outputDocumentNameWithoutExtension + ".pptx";
            }

            if (documentFullName.ToLowerInvariant().EndsWith(".txt"))
            {
                return outputDocumentNameWithoutExtension + ".txt";
            }

            if (documentFullName.ToLowerInvariant().EndsWith(".doc") || documentFullName.ToLowerInvariant().EndsWith(".docx") || documentFullName.ToLowerInvariant().EndsWith(".pdf"))
            {
                return outputDocumentNameWithoutExtension + ".docx";
            }

            return outputDocumentNameWithoutExtension + documentFullName.Substring(documentFullName.LastIndexOf(".", StringComparison.Ordinal));
        }


        private static void ProcessHTMLDocument(string fullNameForDocumentToProcess, string sourceLanguage, string targetLanguage)
        {
            HTMLTranslationManager.DoTranslation(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
        }

        private static void ProcessSRTDocument(string fullNameForDocumentToProcess, string sourceLanguage, string targetLanguage)
        {
            SRTTranslationManager.DoTranslation(fullNameForDocumentToProcess, sourceLanguage, targetLanguage);
        }


        /// <summary>
        /// Translates a plain text document in UTF8 encoding to the target language.
        /// </summary>
        /// <param name="fullNameForDocumentToProcess">SOurce document file name</param>
        /// <param name="sourceLanguage">From language</param>
        /// <param name="targetLanguage">To language</param>
        private static void ProcessTextDocument(string fullNameForDocumentToProcess, string sourceLanguage, string targetLanguage)
        {
            var document = File.ReadAllLines(fullNameForDocumentToProcess, Encoding.UTF8);
            List<string> lstTexts = new List<string>(document);
            var batches = SplitList(lstTexts, 99, 9000);
            File.Delete(fullNameForDocumentToProcess);

            foreach (var batch in batches)
            {
                string[] translated = TranslationServiceFacade.TranslateArray(batch.ToArray(), sourceLanguage, targetLanguage);
                File.AppendAllLines(fullNameForDocumentToProcess, translated, Encoding.UTF8);
            }

            return;
        }

        /// <summary>
        /// Create a CSV file with the alignment information as the third column. Original in 1st, translation in 2nd and alignment in 3rd column.
        /// Source document must be UTF-8 encoded text file. 
        /// </summary>
        /// <param name="fullNameForDocumentToProcess">Source document name</param>
        /// <param name="sourceLanguage">From language</param>
        /// <param name="targetLanguage">To language</param>
        public static void CreateAlignmentCSV(string fullNameForDocumentToProcess, string sourceLanguage, string targetLanguage)
        {
            var document = File.ReadAllLines(fullNameForDocumentToProcess, Encoding.UTF8);
            List<string> lstTexts = new List<string>(document);
            var batches = SplitList(lstTexts, 99, 9000);
            var textfile = File.CreateText(fullNameForDocumentToProcess + "." + TranslationServiceFacade.LanguageNameToLanguageCode(targetLanguage) + ".csv");
            textfile.WriteLine("\"{0}\",\"{1}\",\"{2}\"", TranslationServiceFacade.LanguageNameToLanguageCode(sourceLanguage).ToUpperInvariant(),
                                                          TranslationServiceFacade.LanguageNameToLanguageCode(targetLanguage).ToUpperInvariant(),
                                                          "Word Alignment");

            foreach (var batch in batches)
            {
                string[] alignments = null;
                string[] translated = TranslationServiceFacade.GetAlignments(batch.ToArray(), sourceLanguage, targetLanguage, ref alignments);

                for (int i=0; i<batch.Count(); i++)
                {
                    textfile.WriteLine("\"{0}\",\"{1}\",\"{2}\"", batch[i].Replace("\"", "\"\""), translated[i].Replace("\"", "\"\""), alignments[i]);
                }

            }
            textfile.Close();
            return;
        }



        private static void ProcessExcelDocument(
            string outputDocumentFullName,
            string sourceLanguage,
            string targetLanguage)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(outputDocumentFullName, true))
            {
                //document.WorkbookPart.SharedStringTablePart.PutXDocument();
                List<DocumentFormat.OpenXml.Spreadsheet.Text> lstTexts = new List<DocumentFormat.OpenXml.Spreadsheet.Text>();
                foreach (SharedStringItem si in document.WorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>())
                {
                    if (si != null && si.Text != null && !String.IsNullOrEmpty(si.Text.Text))
                    {
                        lstTexts.Add(si.Text);
                    }
                    else if (si != null)
                    {
                        lstTexts.AddRange(si.Elements<Run>().Where(item => (item != null && item.Text != null && !String.IsNullOrEmpty(item.Text.Text))).Select(item => item.Text));
                    }
                }

                var batch = lstTexts.Select(item => item.Text);
                IEnumerable<string> values = batch as string[] ?? batch.ToArray();

                var batches = SplitList(values, 99, 9000);
                string[] translated = new string[values.Count()];

                var exceptions = new ConcurrentQueue<Exception>();

                Parallel.For(
                    0,
                    batches.Count(),
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    l =>
                        {
                            try
                            {
                                var translationOutput = TranslationServiceFacade.TranslateArray(
                                    batches[l].ToArray(),
                                    sourceLanguage,
                                    targetLanguage);
                                int batchStartIndexInDocument = 0;
                                for (int i = 0; i < l; i++)
                                {
                                    batchStartIndexInDocument = batchStartIndexInDocument + batches[i].Count();
                                }

                                // Apply translated batch to document
                                for (int j = 0; j < translationOutput.Length; j++)
                                {
                                    int indexInDocument = j + batchStartIndexInDocument + 1;
                                    var newValue = translationOutput[j];
                                    translated[indexInDocument - 1] = newValue;
                                    lstTexts[indexInDocument-1].Text = newValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Enqueue(ex);
                            }
                        });

                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }

                // Refresh all the shared string references.
                foreach (var table in document.WorkbookPart.GetPartsOfType<WorksheetPart>().Select(part => part.TableDefinitionParts).SelectMany(tables => tables))
                {
                    foreach (TableColumn col in table.Table.TableColumns)
                    {
                        col.Name = translated[int.Parse(col.Id) - 1];
                    }

                    table.Table.Save();
                }

                // Update comments
                WorkbookPart workBookPart = document.WorkbookPart;
                List<DocumentFormat.OpenXml.Spreadsheet.Comment> lstComments = new List<DocumentFormat.OpenXml.Spreadsheet.Comment>();
                foreach (WorksheetCommentsPart commentsPart in workBookPart.WorksheetParts.SelectMany(sheet => sheet.GetPartsOfType<WorksheetCommentsPart>()))
                {
                    lstComments.AddRange(commentsPart.Comments.CommentList.Cast<Comment>());
                }

                var batchComments = lstComments.Select(item => item.InnerText);
                var batchesComments = SplitList(batchComments, 99, 9000);
                string[] translatedComments = new string[batchesComments.Count()];

                Parallel.For(
                    0,
                    batchesComments.Count(),
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    l =>
                        {
                            try
                            {
                                var translationOutput =
                                    TranslationServiceFacade.TranslateArray(
                                        batchesComments[l].ToArray(),
                                        sourceLanguage,
                                        targetLanguage);
                                int batchStartIndexInDocument = 0;
                                for (int i = 0; i < l; i++)
                                {
                                    batchStartIndexInDocument = batchStartIndexInDocument + batches[i].Count();
                                }

                                for (int j = 0; j < translationOutput.Length; j++)
                                {
                                    int indexInDocument = j + batchStartIndexInDocument + 1;
                                    var currentSharedStringItem = lstComments.Take(indexInDocument).Last();
                                    var newValue = translationOutput[j];
                                    if (translatedComments.Count() > indexInDocument - 1)
                                    {
                                        translatedComments[indexInDocument - 1] = newValue;
                                    }
                                    currentSharedStringItem.CommentText = new CommentText
                                                                              {
                                                                                  Text =
                                                                                      new DocumentFormat.
                                                                                      OpenXml.Spreadsheet.
                                                                                      Text
                                                                                          {
                                                                                              Text = newValue
                                                                                          }
                                                                              };
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Enqueue(ex);
                            }
                        });

                // Throw the exceptions here after the loop completes. 
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        private static void ProcessPowerPointDocument(string outputDocumentFullName,string sourceLanguage,string targetLanguage)
        {
            using (PresentationDocument doc = PresentationDocument.Open(outputDocumentFullName, true))
            {
                //doc.PresentationPart.PutXDocument();

                List<DocumentFormat.OpenXml.Drawing.Text> texts = new List<DocumentFormat.OpenXml.Drawing.Text>();
                List<DocumentFormat.OpenXml.Drawing.Text> notes = new List<DocumentFormat.OpenXml.Drawing.Text>();
                List<DocumentFormat.OpenXml.Presentation.Comment> lstComments = new List<DocumentFormat.OpenXml.Presentation.Comment>();
           
                var slideParts = doc.PresentationPart.SlideParts;
                if (slideParts != null)
                {
                    foreach (var slidePart in slideParts)
                    {
                        if (slidePart.Slide != null)
                        {
                            var slide = slidePart.Slide;
                            ExtractTextContent(texts, slide);

                            var commentsPart = slidePart.SlideCommentsPart;
                            if (commentsPart != null)
                            {
                                lstComments.AddRange(commentsPart.CommentList.Cast<DocumentFormat.OpenXml.Presentation.Comment>());
                            }

                            var notesPart = slidePart.NotesSlidePart;
                            if (notesPart != null)
                            {
                                ExtractTextContent(notes, notesPart.NotesSlide);
                            }
                        }
                    }

                    ReplaceTextsWithTranslation(texts, sourceLanguage, targetLanguage);
                    ReplaceTextsWithTranslation(notes, sourceLanguage, targetLanguage);

                    if (lstComments.Count() > 0)
                    {
                        // Extract Text for Translation
                        var batch = lstComments.Select(text => text.InnerText);

                        // Do Translation
                        var batchesComments = SplitList(batch, 99, 9000);

                        // Use ConcurrentQueue to enable safe enqueueing from multiple threads. 
                        var exceptions = new ConcurrentQueue<Exception>();

                        Parallel.For(
                            0,
                            batchesComments.Count(),
                            new ParallelOptions { MaxDegreeOfParallelism = 1 },
                            l =>
                            {
                                try
                                {
                                    var translationOutput =
                                        TranslationServiceFacade.TranslateArray(
                                            batchesComments[l].ToArray(),
                                            sourceLanguage,
                                            targetLanguage);
                                    int batchStartIndexInDocument = 0;
                                    for (int i = 0; i < l; i++)
                                    {
                                        batchStartIndexInDocument = batchStartIndexInDocument
                                                                    + batchesComments[i].Count();
                                    }

                                    // Apply translated batch to document
                                    for (int j = 0; j < translationOutput.Length; j++)
                                    {
                                        int indexInDocument = j + batchStartIndexInDocument + 1;
                                        var newValue = translationOutput[j];
                                        var commentPart = lstComments.Take(indexInDocument).Last();
                                        commentPart.Text = new DocumentFormat.OpenXml.Presentation.Text
                                        {
                                            Text = newValue
                                        };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    exceptions.Enqueue(ex);
                                }
                            });

                        // Throw the exceptions here after the loop completes. 
                        if (exceptions.Count > 0)
                        {
                            throw new AggregateException(exceptions);
                        }
                    }
                }

                //doc.PresentationPart.PutXDocument();
            }
        }

        private static void ReplaceTextsWithTranslation(List<DocumentFormat.OpenXml.Drawing.Text> texts, string sourceLanguage, string targetLanguage)
        {
            if (texts.Count() > 0)
            {
                // Extract Text for Translation
                var batch = texts.Select(text => text.Text);

                // Do Translation
                var batches = SplitList(batch, 99, 9000);

                // Use ConcurrentQueue to enable safe enqueueing from multiple threads. 
                var exceptions = new ConcurrentQueue<Exception>();

                Parallel.For(
                    0,
                    batches.Count(),
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    l =>
                    {
                        try
                        {
                            var translationOutput = TranslationServiceFacade.TranslateArray(batches[l].ToArray(), sourceLanguage, targetLanguage);
                            int batchStartIndexInDocument = 0;
                            for (int i = 0; i < l; i++)
                            {
                                batchStartIndexInDocument = batchStartIndexInDocument
                                                            + batches[i].Count();
                            }

                            // Apply translated batch to document
                            for (int j = 0; j < translationOutput.Length; j++)
                            {
                                int indexInDocument = j + batchStartIndexInDocument + 1;
                                var newValue = translationOutput[j];
                                texts.Take(indexInDocument).Last().Text = newValue;
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    });

                // Throw the exceptions here after the loop completes. 
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        private static void ExtractTextContent(List<DocumentFormat.OpenXml.Drawing.Text> textList, DocumentFormat.OpenXml.OpenXmlElement element)
        {
            foreach (DocumentFormat.OpenXml.Drawing.Paragraph para in element.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
            {
                textList.AddRange(para.Elements<DocumentFormat.OpenXml.Drawing.Run>().Where(item => (item != null && item.Text != null && !String.IsNullOrEmpty(item.Text.Text))).Select(item => item.Text));
            }
        }

        private static void ProcessWordDocument(
            string outputDocumentFullName,
            string sourceLanguage,
            string targetLanguage)
        {

            List<DocumentFormat.OpenXml.Wordprocessing.Text> texts = new List<DocumentFormat.OpenXml.Wordprocessing.Text>();
            using (WordprocessingDocument doc = WordprocessingDocument.Open(outputDocumentFullName, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                texts.AddRange(body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>().Where(text => !String.IsNullOrEmpty(text.Text) && text.Text.Length > 1));
                var exceptions = new ConcurrentQueue<Exception>();

                // Extract Text for Translation
                var batch = texts.Select(text => text.Text);

                // Do Translation
                var batches = SplitList(batch, 99, 9000);
                Parallel.For(
                    0,
                    batches.Count(),
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    l =>
                        {
                            try
                            {
                                var translationOutput = TranslationServiceFacade.TranslateArray(
                                    batches[l].ToArray(),
                                    sourceLanguage,
                                    targetLanguage);
                                int batchStartIndexInDocument = 0;
                                for (int i = 0; i < l; i++)
                                {
                                    batchStartIndexInDocument = batchStartIndexInDocument + batches[i].Count();
                                }

                                // Apply translated batch to document
                                for (int j = 0; j < translationOutput.Length; j++)
                                {
                                    int indexInDocument = j + batchStartIndexInDocument + 1;
                                    var newValue = translationOutput[j];
                                    texts.Take(indexInDocument).Last().Text = newValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Enqueue(ex);
                            }
                        });

                // Throw the exceptions here after the loop completes. 
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }

                //doc.MainDocumentPart.PutXDocument();
            }
        }

        /// <summary>
        /// Splits the list.
        /// </summary>
        /// <param name="values">
        ///  The values to be split.
        /// </param>
        /// <param name="groupSize">
        ///  The group size.
        /// </param>
        /// <param name="maxSize">
        ///  The max size.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///  The System.Collections.Generic.List`1[T -&gt; System.Collections.Generic.List`1[T -&gt; T]].
        /// </returns>
        private static List<List<T>> SplitList<T>(IEnumerable<T> values, int groupSize, int maxSize)
        {
            List<List<T>> result = new List<List<T>>();
            List<T> valueList = values.ToList();
            int startIndex = 0;
            int count = valueList.Count;

            while (startIndex < count)
            {
                int elementCount = (startIndex + groupSize > count) ? count - startIndex : groupSize;
                while (true)
                {
                    var aggregatedSize =
                        valueList.GetRange(startIndex, elementCount)
                            .Aggregate(
                                new StringBuilder(),
                                (s, i) => s.Length < maxSize ? s.Append(i) : s,
                                s => s.ToString())
                            .Length;
                    if (aggregatedSize >= maxSize)
                    {
                        elementCount = elementCount - 1;
                    }
                    else
                    {
                        break;
                    }
                }

                result.Add(valueList.GetRange(startIndex, elementCount));
                startIndex += elementCount;
            }

            return result;
        }

        #endregion
    }
}