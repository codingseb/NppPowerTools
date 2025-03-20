using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;

namespace NppPowerTools.Utils
{
    public class PDFFile : IDocument
    {
        public string FileName { get; set; }

        public Action<IDocumentContainer> FirstAction { get; set; }
        public Delegate ComposeAction { get; set; }

        public void Compose(IDocumentContainer container)
        {
            FirstAction?.Invoke(container);
            ComposeAction?.DynamicInvoke(new object[] { new object[] { container } });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public DocumentSettings GetSettings() => new();

        public void Save(string fileName)
        {
            FileName = fileName;
            this.GeneratePdf(fileName);
        }

        public void SaveOpen(string fileName)
        {
            FileName = fileName;
            this.GeneratePdf(fileName);
            Process.Start(fileName);
        }
    }
}
