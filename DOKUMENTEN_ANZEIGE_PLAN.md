# Umsetzungsplan: Dokumenten-/Excel-/Video-Anzeige (Aufgabe 3+4)

**Stand:** 2026-06-30
**Status:** PLANUNG — wartet auf Freigabe. **Noch kein Branch, keine Implementierung.**
Umsetzung später auf neuem Branch `feature/dokumenten-anzeige` (von `main`).

---

## 1. Library-Evaluierung (Kern der Freigabe)

### 1.1 PDF-Rendering
Anforderungen: echtes PDF-Rendering, Zoom, Blättern, Sprung zu Seite, read-only,
eingebettet (Touch), .NET Framework 4.6.1 / WPF, möglichst kostenlos.

| Option | Eignung | Lizenz | Bewertung |
|---|---|---|---|
| **PdfiumViewer (+ PdfiumViewer.Native)** ⭐ | Google-PDFium-Engine, rendert Seiten als Bild → volle Kontrolle über Zoom/Touch in WPF | **Apache-2.0** (Wrapper) + **BSD-3** (PDFium) | **Empfohlen.** Bewährt auf .NET FW/WPF, rein lesend, kein Office nötig. Native DLL via NuGet (x86+x64, AnyCPU-tauglich). |
| Windows.Data.Pdf (OS-eigen) | In Win10/11 enthalten, rendert Seiten als Bitmap | **kostenlos (OS-Bestandteil)** | Gute, abhängigkeitsfreie Alternative (Zielsystem ist Win11). Nachteil: WinRT-Interop aus .NET FW 4.6.1 etwas fummelig. |
| MoonPdf / MuPDF | WPF-Viewer | **AGPL / kommerziell** | Verworfen (Lizenzrisiko). |
| Syncfusion / Spire.PDF / PDFTron / Foxit SDK | sehr komfortabel | kommerziell / Community mit Auflagen | Verworfen (Kosten/Auflagen). |
| PDFsharp/MigraDoc | — | MIT | Nur Erzeugung, **kein** Rendering → ungeeignet. |

**Empfehlung: PdfiumViewer.** Ansatz „render-to-image": PDFium rendert jede Seite in ein
Bild, das in einer WPF-`Image` in einem `ScrollViewer` mit `ScaleTransform` angezeigt wird
(Zoom per Buttons + Pinch via `ManipulationDelta`), plus Seiten-Navigation und Gehe-zu-Seite.
Kein `WindowsFormsHost` nötig (vermeidet Airspace-Probleme). Read-only von Natur aus.

> Alternative auf Wunsch: **Windows.Data.Pdf** (keine Drittlib, OS-nativ). Bitte wählen.

### 1.2 Excel-Lesen (.xls UND .xlsx)
Anforderungen: beide Formate, schlichte flache Tabellen → DataGrid, read-only, kostenlos.

| Option | .xls | .xlsx | Lizenz | Bewertung |
|---|---|---|---|---|
| **ExcelDataReader (+ .DataSet)** ⭐ | ✔ | ✔ | **MIT** | **Empfohlen** für genau diesen Fall: liest direkt in `DataSet`/`DataTable` → 1:1 an `DataGrid.ItemsSource`. Minimaler Code, rein lesend, schlank. |
| NPOI (vom Auftraggeber vorgeschlagen) | ✔ | ✔ | **Apache-2.0** | Gleichwertig, etwas mehr Code (Zell-Iteration); kann auch schreiben (hier nicht nötig). |
| ClosedXML | �’ nur xlsx | ✔ | MIT | **Ungeeignet** (kein .xls). |

**Empfehlung: ExcelDataReader** (leanster Weg „flache Tabelle → DataGrid"). **NPOI** (deine
Anregung) ist gleichwertig — falls bevorzugt, setze ich NPOI ein. Deine Wahl.
(Hinweis: für manche alten .xls-Codepages ggf. `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)`.)

### 1.3 Video
**Kein** neues Paket. Bestehender `WindowPopUpVideoPlayer` (WPF `MediaElement`) wird
wiederverwendet, nur die Pfadquelle auf die neue Fallback-Logik umgestellt.

### Paket-Installation
Da es ein classic `packages.config`-Projekt ist, installiere ich die Pakete per `nuget.exe`
(lade ich bei Bedarf herunter, da im VS2017-Pfad keine `nuget.exe` liegt) bzw. trage
`packages.config` + csproj-`<Reference>`/`<Resource>` manuell ein und stelle wieder her.
Betroffene Pakete: `PdfiumViewer`, `PdfiumViewer.Native.x86.v8-xfa`,
`PdfiumViewer.Native.x86_64.v8-xfa`, `ExcelDataReader`, `ExcelDataReader.DataSet`.

---

## 2. Architektur

### 2.1 Pfad-/Fallback-Logik (Anforderung 1) — neuer Helfer
Neuer Helfer (z. B. `Helper/DocumentationPaths.cs`), aufgerufen beim Start (ersetzt den
`#region Check Instruction Paths`-Block in `App.xaml.cs`):
- `baseDrive = Directory.Exists(@"D:\") ? @"D:\" : @"C:\"`
- `videoRoot = baseDrive + "Documentation_Videos\"`
- `docRoot   = baseDrive + "Documentation_Documents\"`
- Beim Start sicherstellen (anlegen, falls fehlend): `videoRoot`, `docRoot` und die 6
  Unterordner `Circuit Diagram`, `Manuals`, `Partlists`, `Pneumatic`,
  `Spare Parts Documentation`, `Technical Drawings` (via `Directory.CreateDirectory`).
- Kategorien-Mapping (Dropdown 1 → Ordner):
  - „Videos" → `videoRoot` (flach, `*.mov`)
  - die 6 Dokument-Kategorien → jeweiliger Unterordner unter `docRoot` (`*.pdf`, `*.xls`, `*.xlsx`)
- Alte Felder `GlobalVar.manualPath/videoPath/manualExists/videoExists` werden auf die neue
  Logik umgestellt bzw. ersetzt.

### 2.2 Neue Anzeige-Seite (Anforderung 2) — `PgDocumentation` (Page)
- Oben **Dropdown 1 (Kategorie)** mit den 7 Einträgen und **Dropdown 2 (Datei)** (gefüllt aus
  dem Kategorie-Ordner; Anzeige = Dateiname ohne Endung).
- Darunter ein **Content-Host** (`ContentControl`/`Grid`), der je Dateiendung den passenden
  Viewer einblendet:
  - `.pdf` → **PdfViewerControl** (inline)
  - `.xls`/`.xlsx` → **ExcelViewerControl** (inline)
  - `.mov` → Video (siehe 2.5)
- Auswahl in Dropdown 2 lädt die Datei automatisch.

### 2.3 PdfViewerControl (UserControl, Anforderung 3)
- PDFium render-to-image; `Image` in `ScrollViewer`; Zoom (Buttons +/− und Pinch),
  Blättern (vor/zurück), Gehe-zu-Seite (Texteingabe), Seitenanzeige „x / n". Read-only.
- Ablage: `UserControls/Documents/` (neuer fachlicher Unterordner, passend zur neuen Struktur).

### 2.4 ExcelViewerControl (UserControl, Anforderung 4)
- `DataGrid` (read-only, `IsReadOnly=True`, AutoGenerateColumns) gebunden an `DataTable`.
- Bei mehreren Tabellenblättern optional kleiner Blatt-Auswahl-Tab/Dropdown (klären, s. u.).
- Ablage: `UserControls/Documents/`.

### 2.5 Video (Anforderung 5)
Zwei Varianten — **Entscheidung nötig**:
- **(A, empfohlen, minimal):** Auswahl einer `.mov` in der neuen Seite öffnet den
  **bestehenden** `WindowPopUpVideoPlayer` (nur Pfadquelle = neue Logik). Erfüllt „keine
  Funktionsänderung" wörtlich.
- **(B):** Video inline in der Seite einbetten (MediaElement-Logik in ein UserControl
  refaktorieren). Konsistentere UX, aber mehr Änderung am Video-Teil.

### 2.6 Alt-Code
- `WindowPopUpManualReader` (Bildserien) wird durch PdfViewer + Seite **ersetzt** und entfernt.
- ComboBox-Logik in `MainWindow` (`manualCB`/`videoCB`, `SetManuals`/`SetVideos`,
  `ManualCB_SelectionChanged`, `VideoCB_SelectionChanged`) wird durch die neue Seite ersetzt
  bzw. auf diese umgeleitet (Details s. Rückfrage 4).

---

## 3. Offene Punkte / Rückfragen

1. **PDF-Library:** PdfiumViewer (empfohlen) **oder** Windows.Data.Pdf (abhängigkeitsfrei)?
2. **Excel-Library:** ExcelDataReader (empfohlen) **oder** NPOI (deine Anregung)?
3. **Video-Integration:** Variante A (Popup wiederverwenden, empfohlen) **oder** B (inline)?
4. **Einbindung/Navigation der neuen Seite:** Wo soll `PgDocumentation` erreichbar sein?
   - Vorschlag: als Display in der bestehenden Navigation, und die bisherigen
     Manual-/Video-Dropdowns in `MainWindow` entfernen/umleiten. Bitte Zielort im Menü nennen
     (oder „mach einen sinnvollen Vorschlag").
5. **Excel-Mehrblatt:** Falls eine Datei mehrere Tabellenblätter hat — Blattauswahl anzeigen
   oder nur das erste Blatt? (Vorschlag: erste Blatt + optionale Auswahl, falls >1.)
6. **Alt-Reader entfernen:** Bestätigung, dass `WindowPopUpManualReader` (+ zugehörige
   MainWindow-Logik) entfernt werden darf.
7. **AnyCPU/Native:** PdfiumViewer.Native bringt x86+x64 mit (AnyCPU-tauglich) — ok so.

---

## 4. Umsetzungsschritte (nach Freigabe, kleine Commits, Build nach jedem Schritt)

1. Branch `feature/dokumenten-anzeige` von `main`.
2. NuGet-Pakete hinzufügen (PdfiumViewer (+Native), ExcelDataReader (+DataSet)) + Build-Check.
3. `DocumentationPaths`-Helfer + `App.xaml.cs`-Init (Fallback + Auto-Anlage) + Build.
4. `ExcelViewerControl` (UserControl) + Build.
5. `PdfViewerControl` (UserControl) + Build.
6. `PgDocumentation` (Seite mit 2 Dropdowns + Typ-Dispatch) + Build.
7. Video auf neue Pfadlogik umstellen (Variante A/B) + Build.
8. Navigation/Anbindung; alten `WindowPopUpManualReader` + MainWindow-Combo-Logik entfernen + Build.
9. Abschluss-Rebuild.

**Ich beginne erst nach deiner Freigabe (insb. Rückfragen 1–4).**
