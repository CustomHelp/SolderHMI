using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LOET_HMI
{
    /// <summary>
    /// Einfacher, thread-sicherer Datei-Logger fuer das HMI.
    /// - Schreibt eine Zeile pro Eintrag nach D:\Logs\HMI\ (sonst C:\Logs\HMI\).
    /// - Ein File pro Tag: HMI_YYYY-MM-DD.log (automatischer Wechsel bei Tageswechsel).
    /// - Behaelt die neuesten 30 Log-Dateien, aeltere werden beim Datei-Wechsel geloescht.
    /// - Wirft NIEMALS eine Exception nach aussen: die Anlage darf nicht wegen Logging abstuerzen.
    ///   Ist kein Laufwerk beschreibbar, wird still weitergelaufen.
    /// Level: WARN = behandelte (gefangene) Exceptions, ERROR = unerwartete/bediener-relevante
    /// Fehler (mit StackTrace in der Folgezeile), INFO = reine Info-Meldungen.
    /// </summary>
    public static class AppLogger
    {
        private static readonly object _lock = new object();
        private const int MaxLogFiles = 30;

        private static string _logDir;
        private static string _currentFilePath;
        private static string _currentFileDate; // yyyy-MM-dd des aktuell offenen Log-Files

        /// <summary>Beim App-Start aufrufen: legt den Log-Ordner an, raeumt alte Dateien auf.</summary>
        public static void Init()
        {
            try
            {
                Log("AppLogger", "HMI gestartet - Logging aktiv.");
            }
            catch
            {
                // Logging darf den Start niemals verhindern.
            }
        }

        /// <summary>Gefangene Exception protokollieren (Level WARN, einzeilig).</summary>
        public static void Log(string context, Exception ex)
        {
            if (ex == null) { Write("WARN", context, "(Exception war null)", null); return; }
            Write("WARN", context, ex.Message + " (" + ex.GetType().Name + ")", null);
        }

        /// <summary>Reine Info-Meldung protokollieren (Level INFO).</summary>
        public static void Log(string context, string message)
        {
            Write("INFO", context, message, null);
        }

        /// <summary>Unerwarteten/bediener-relevanten Fehler protokollieren (Level ERROR, mit StackTrace).</summary>
        public static void LogError(string context, Exception ex)
        {
            if (ex == null) { Write("ERROR", context, "(Exception war null)", null); return; }
            Write("ERROR", context, ex.Message + " (" + ex.GetType().Name + ")", ex.StackTrace);
        }

        /// <summary>Unerwarteten/bediener-relevanten Fehler als Text protokollieren (Level ERROR).</summary>
        public static void LogError(string context, string message)
        {
            Write("ERROR", context, message, null);
        }

        private static void Write(string level, string context, string message, string stackTrace)
        {
            try
            {
                lock (_lock)
                {
                    if (!EnsureLogDir())
                        return; // kein beschreibbares Laufwerk -> still ignorieren

                    RollFileIfNeeded();

                    string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                  + " | " + level
                                  + " | " + (context ?? "")
                                  + " | " + (message ?? "");

                    // Bei ERROR optional den StackTrace in die Folgezeile (Tab-eingerueckt).
                    if (level == "ERROR" && !string.IsNullOrEmpty(stackTrace))
                        line += Environment.NewLine + "\t" + stackTrace.Replace(Environment.NewLine, Environment.NewLine + "\t");

                    File.AppendAllText(_currentFilePath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Der Logger darf niemals werfen.
            }
        }

        /// <summary>Ermittelt/erstellt den Log-Ordner (D: sonst C:). false, wenn nichts beschreibbar ist.</summary>
        private static bool EnsureLogDir()
        {
            if (_logDir != null && Directory.Exists(_logDir))
                return true;

            // Gleiches Laufwerk-Fallback wie im uebrigen Projekt (siehe DocumentationPaths).
            string baseDrive = Directory.Exists(@"D:\") ? @"D:\" : @"C:\";
            string dir = Path.Combine(baseDrive, "Logs", "HMI");
            Directory.CreateDirectory(dir);
            _logDir = dir;
            return true;
        }

        /// <summary>Bei Tageswechsel (oder erstem Aufruf) neues File waehlen und alte Dateien aufraeumen.</summary>
        private static void RollFileIfNeeded()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (_currentFileDate == today && _currentFilePath != null)
                return;

            _currentFileDate = today;
            _currentFilePath = Path.Combine(_logDir, "HMI_" + today + ".log");
            CleanupOldFiles();
        }

        /// <summary>Behaelt die neuesten MaxLogFiles HMI_*.log-Dateien, loescht aeltere.</summary>
        private static void CleanupOldFiles()
        {
            try
            {
                var files = new DirectoryInfo(_logDir)
                    .GetFiles("HMI_*.log")
                    .OrderByDescending(f => f.Name) // Dateiname enthaelt das Datum -> chronologische Sortierung
                    .ToList();

                for (int i = MaxLogFiles; i < files.Count; i++)
                {
                    try { files[i].Delete(); }
                    catch { /* einzelne nicht loeschbare Datei ueberspringen */ }
                }
            }
            catch
            {
                // Aufraeumen ist optional und darf nie stoeren.
            }
        }
    }
}
