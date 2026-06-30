# Auftrag: HMI-Projekt Analyse, Cleanup & Dokumentenanzeige

## Projektkontext
- WPF-Anwendung (.NET Framework 4.6.1, classic .csproj, NICHT SDK-Style)
- Solution: `LOET_HMI.sln`, Hauptprojekt `LOET_HMI`
- Steuerung über Beckhoff TwinCAT.Ads (`ADS_Com/ADSConnection.cs`, `ADS_Com/ADSService.cs`)
- Gewachsenes Projekt mit Altlasten, es existieren parallel eine alte Struktur und ein
  angefangener, nicht abgeschlossener Umbau in `_NEUE_ORDNERSTRUKTUR/`
- **Kein Total-Umbau / keine MVVM-Modernisierung in diesem Auftrag.** Nur gezielte
  Verbesserungen, siehe unten.
- Git-Repo ist initialisiert. Bitte für jeden abgeschlossenen logischen Schritt einen
  eigenen Commit mit aussagekräftiger Message erstellen, NICHT alles in einen Commit packen.

## Aufgabe 1: Analyse (vor jeder Änderung!)

1. Erstelle eine vollständige Referenzanalyse: welche Klassen, XAML-Views und
   UserControls werden tatsächlich noch irgendwo aufgerufen/instanziiert, und welche
   sind tote Hinterlassenschaften (keine Referenzen mehr im Projekt).
2. Besonderes Augenmerk auf folgende Verdachtskandidaten — bitte für jeden einzeln
   klären, ob ungenutzt:
   - `UserControls/Alt/`
   - `UserControls/Frank/`
   - `PLC_Com_Classes/Nicht verwendet (HOMS, REEB)/`
   - `_NEUE_ORDNERSTRUKTUR/` (klären: aktiv genutzt oder verworfener Versuch — falls
     unklar aus dem Code, im Ergebnisreport explizit als offene Frage auflisten statt
     zu raten)
   - `SystemPages/HORSCH Archiv/`
3. Liefere vor dem Löschen einen kurzen Report (z.B. `ANALYSE.md`) mit:
   - Liste der als "tot/ungenutzt" identifizierten Dateien/Ordner inkl. Begründung
   - Liste der unklaren Fälle, die Rückfragen brauchen
   - Geschätzte Code-Reduktion (Anzahl Dateien)

## Aufgabe 2: Aufräumen

1. Nach Freigabe des Analyse-Reports (auf Bestätigung warten, nicht blind löschen):
   - Tote/ungenutzte Dateien und Ordner entfernen
   - `.csproj` entsprechend bereinigen (keine verwaisten Item-Includes)
   - Sicherstellen, dass das Projekt nach dem Aufräumen weiterhin fehlerfrei baut
2. Commit mit klarer Beschreibung, was entfernt wurde und warum.

## Aufgabe 3: Dokumentenanzeige verbessern (Manuals / Circuit Diagrams)

### Ist-Zustand (Problem)
`SystemPages/PopUps/WindowPopUpManualReader.xaml.cs` zeigt aktuell **keine echten
PDFs** an, sondern lädt einen Ordner voller Bilddateien (`BitmapImage`) und blättert
seitenweise durch. Das ist fehleranfällig, kein echtes PDF-Rendering, kein Zoom.

### Soll-Zustand (Anforderungen)
- Anzeige von **echten PDF-Dateien** (nicht mehr Bildserien)
- **Zoom** (rein/raus, z.B. per Touch-Geste oder Buttons)
- **Blättern** (vor/zurück, wie bisher per Button)
- **Sprung zu Seite** (Eingabe einer Seitenzahl, direkter Sprung)
- **Read-only** — keine Druckfunktion nötig, keine Bearbeitungsfunktion
- Läuft eingebettet im HMI-Fenster (Touch-Bedienung), kein externes Öffnen von Foxit
  Reader o.ä. nötig (Foxit ist als Backup auf dem System vorhanden, aber nicht
  Teil dieser Lösung)
- Bitte technischen Ansatz selbst evaluieren und kurz begründen (z.B. PDF-Rendering-
  Library, die mit .NET Framework 4.6.1 / WPF kompatibel ist und sich gut für
  Embedded-Touch-Anzeige mit Zoom/Seitennavigation eignet). Vor Implementierung
  kurz die gewählte Library + Begründung + Lizenz nennen.
- Pfad-Handling: aktuell wird ein Verzeichnis mit Dateien als "Manual" übergeben
  (siehe `WindowPopUpManualReader(string filename)` Constructor, `GlobalVar.manualPath`).
  Anpassen, sodass eine einzelne PDF-Datei direkt geöffnet wird (Verzeichnis-Inhalt
  ist dann die PDF-Auswahl, nicht mehr Bildseiten).
- Bestehende Aufrufstellen (überall wo `WindowPopUpManualReader` instanziiert wird)
  entsprechend anpassen.

## Allgemeine Vorgaben

- Keine Änderungen an der ADS/Beckhoff-Kommunikationsschicht im Rahmen dieses Auftrags.
- Bestehenden Code-Stil im Projekt beibehalten (keine Formatierungs-Großumbauten).
- Nach jeder Aufgabe: kurzer Status in Textform, was gemacht wurde, was noch offen ist.
- Bei Unsicherheiten oder mehreren sinnvollen Lösungswegen: nachfragen statt raten.
