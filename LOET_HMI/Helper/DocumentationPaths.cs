using System;
using System.Collections.Generic;
using System.IO;

namespace LOET_HMI
{
    /// <summary>
    /// Zentrales Pfad-Handling für die Dokumentenanzeige (PDF/Excel) und Videos.
    ///
    /// Laufwerk-Fallback: zuerst D:, sonst C:. Fehlende Ordner werden beim Start
    /// automatisch angelegt (Documentation_Videos\ und Documentation_Documents\
    /// inkl. der Kategorie-Unterordner).
    /// </summary>
    public static class DocumentationPaths
    {
        public const string VideosCategory = "Videos";

        /// <summary>Dokument-Kategorien (Reihenfolge = Reihenfolge im Kategorie-Dropdown).</summary>
        public static readonly string[] DocumentSubfolders = new[]
        {
            "Circuit Diagram",
            "Manuals",
            "Partlists",
            "Pneumatic",
            "Spare Parts Documentation",
            "Technical Drawings"
        };

        public static string BaseDrive { get; private set; }
        public static string VideoRoot { get; private set; }
        public static string DocumentsRoot { get; private set; }

        /// <summary>Beim Programmstart aufrufen: Laufwerk wählen und Ordnerstruktur sicherstellen.</summary>
        public static void Initialize()
        {
            BaseDrive = Directory.Exists(@"D:\") ? @"D:\" : @"C:\";
            VideoRoot = Path.Combine(BaseDrive, "Documentation_Videos") + "\\";
            DocumentsRoot = Path.Combine(BaseDrive, "Documentation_Documents") + "\\";

            EnsureFolders();

            // Bestehender Video-Player liest weiterhin GlobalVar.videoPath.
            GlobalVar.videoPath = VideoRoot;
            GlobalVar.manualPath = DocumentsRoot;
            GlobalVar.videoExists = true;
            GlobalVar.manualExists = true;
        }

        /// <summary>Legt fehlende Ordner an (idempotent).</summary>
        public static void EnsureFolders()
        {
            Directory.CreateDirectory(VideoRoot);
            Directory.CreateDirectory(DocumentsRoot);
            foreach (string sub in DocumentSubfolders)
            {
                Directory.CreateDirectory(Path.Combine(DocumentsRoot, sub));
            }
        }

        /// <summary>Alle Kategorien in Dropdown-Reihenfolge ("Videos" zuerst, dann die Dokument-Kategorien).</summary>
        public static IEnumerable<string> Categories
        {
            get
            {
                yield return VideosCategory;
                foreach (string sub in DocumentSubfolders)
                {
                    yield return sub;
                }
            }
        }

        public static bool IsVideoCategory(string category)
        {
            return string.Equals(category, VideosCategory, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Ordnerpfad einer Kategorie (mit abschließendem Backslash).</summary>
        public static string GetCategoryPath(string category)
        {
            if (IsVideoCategory(category))
            {
                return VideoRoot;
            }
            return Path.Combine(DocumentsRoot, category) + "\\";
        }
    }
}
