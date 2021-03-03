using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;

namespace NppPowerTools.Utils
{
    public class PDFFile : IDocument
    {
        public Delegate ComposeAction { get; set; }

        public void Compose(IContainer container)
        {
            ComposeAction?.DynamicInvoke(new object[] { new object[] { container } });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Save(string fileName)
        {
            this.GeneratePdf(fileName);
        }
    }
}
