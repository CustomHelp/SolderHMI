# Analyse-Report: HMI-Projekt Cleanup (Aufgabe 1)

**Stand:** 2026-06-30
**Scope:** Referenzanalyse vor dem Aufräumen. **Es wurde nichts gelöscht oder geändert.**
Dieser Report ist die Entscheidungsgrundlage für Aufgabe 2 (Aufräumen) und wartet auf
Freigabe.

---

## 1. Methodik

- Vollständiges Einlesen von `LOET_HMI/LOET_HMI.csproj` (1217 Zeilen, classic .csproj —
  d. h. **nur explizit gelistete Dateien werden kompiliert**, kein Wildcard-Include).
- Projektweite Referenzsuche pro Verdachts-Klasse / -Control / -View über alle `.cs`-
  und `.xaml`-Dateien (Wortgrenzen-Suche, um Teilstring-Fehltreffer wie `UcValue` in
  `UcValueInOut` oder `StPLCSetting` in `StPLCSettingWithDB` auszuschließen).
- Abgleich „auf der Festplatte vorhanden" vs. „im `.csproj` enthalten" (Orphan-Erkennung).
- Prüfung des Navigationsmechanismus: Seiten (`Page`) werden in `UcNavWorksp` über
  `NavCluster.Display` und `Frame.Navigate(...)` geladen. Eine Seite/ein Control gilt nur
  dann als **genutzt**, wenn sie irgendwo per `new XYZ()` instanziiert **oder** als XAML-Tag
  (`<LOET_HMI:XYZ …>`) eingebunden wird. Kein Hinweis auf reflexions-/string-basierte
  Instanziierung gefunden.

**Wichtige Einschränkung:** Endgültige Sicherheit liefert erst ein erfolgreicher Build nach
dem Entfernen (Teil von Aufgabe 2). Die untenstehende Einstufung basiert auf statischer
Referenzanalyse, die für die genannten Verdachtskandidaten vollständig durchgeführt wurde.

---

## 2. Als TOT / UNGENUTZT identifiziert

### 2.1 `UserControls/Alt/` — komplett ungenutzt ✅ (ganzer Ordner)
Keine der 5 Controls wird außerhalb des eigenen Ordners referenziert (nur Einträge im
`.csproj`). `Cylinder` taucht außerhalb nur als String-Literal in Log-Meldungen
(`StCylinder.cs`, "Cylinder " + …) auf — **keine** Typ-Referenz.

| Datei | Im .csproj | Begründung |
|---|---|---|
| `Cylinder.xaml` + `.xaml.cs` | ja (Page+Compile) | keine externe Referenz |
| `TextBoxNewRecipe.cs` + `TextBoxNewRecipe.xaml` | ja | keine externe Referenz |
| `UcSettingDINT.xaml` + `.xaml.cs` | ja | keine externe Referenz |
| `UcSettingDINT_RENA.xaml` + `.xaml.cs` | ja | keine externe Referenz |
| `UcSettingString.xaml` + `.xaml.cs` | ja | keine externe Referenz |

→ **10 Dateien**, ganzer Ordner entfernbar.

### 2.2 `PLC_Com_Classes/Nicht verwendet (HOMS, REEB)/` — komplett ungenutzt ✅ (ganzer Ordner)
Der Ordnername ist zutreffend. Detail-Befund zum scheinbaren Widerspruch:
`IStPLCSettingWithDB` (definiert in `StPLCSettingWithDB.cs`) taucht in den **kompilierten**
Controls `UcParamDataGrid.xaml.cs` und `UcInOutsDatagrid.xaml.cs` auf — **aber
ausschließlich innerhalb von `/* … */`-Kommentarblöcken** (UcParamDataGrid Z. 101–110,
UcInOutsDatagrid Z. 28–37 & 83 ff.) sowie in einer `//`-Kommentarzeile in `DataProjekt.cs`.
→ Kein aktiver kompilierter Code referenziert diese Klassen.

| Datei | Im .csproj | Begründung |
|---|---|---|
| `StFluidValve.cs` | ja (Compile) | nur Eigendefinition, nie instanziiert |
| `StGrCylinderHMI.cs` | ja (Compile) | nur Eigendefinition, nie instanziiert |
| `ST_MessageWindow.cs` | ja (Compile) | nur Eigendefinition (+ Namens-Kommentar) |
| `StPLCSetting.cs` | **nein** | nicht kompiliert; nur intern genutzt |
| `StPLCSettingWithDB.cs` | **nein** | nicht kompiliert; nur in auskommentiertem Code referenziert |

→ **5 Dateien**, ganzer Ordner entfernbar. (3 davon erfordern Entfernen der `Compile`-
Einträge im `.csproj`, Zeilen 353/354/357; die 2 nicht-kompilierten sind reine Datei-Leichen.)

### 2.3 `SystemPages/HORSCH Archiv/` — komplett ungenutzt ✅ (ganzer Ordner)
Alle 4 Log-Seiten werden nirgends per `new …()` instanziiert und in kein `NavCluster`
eingebunden (Klassenname kommt nur in der eigenen Datei vor).

| View | Dateien | Begründung |
|---|---|---|
| `PageSettManualLog` | `.xaml` + `.xaml.cs` | nie instanziiert |
| `PageSettMessArchiv` | `.xaml` + `.xaml.cs` | nie instanziiert |
| `PageSettParamLog` | `.xaml` + `.xaml.cs` | nie instanziiert |
| `PageSettUserLog` | `.xaml` + `.xaml.cs` | nie instanziiert |

→ **8 Dateien**, ganzer Ordner entfernbar.

### 2.4 `UserControls/Frank/` — TEILWEISE ungenutzt ⚠️ (NICHT ganzer Ordner!)
**Wichtig:** Dieser Ordner darf **nicht** komplett gelöscht werden — 3 von 5 Controls sind
aktiv im Einsatz.

| Control | Status | Verwendet in (Auszug) |
|---|---|---|
| `UcLamp` | **GENUTZT** | PgKROSY, PgOrder, UcStation (KROSY), UcRFID_IND, UcRFID_WT, UcWTVariant, LOET_RFID.cs, StlyesLOET.xaml |
| `UcLampText` | **GENUTZT** | UcBeckhoffAxis, UcHU5000 |
| `UcValue` | **GENUTZT** | UcBeckhoffAxis, PgDVS |
| `UcBarValue` | **tot** | nur Eigendefinition |
| `UcStationButton` | **tot** | nur Eigendefinition |

→ Entfernbar: `UcBarValue.xaml`+`.xaml.cs`, `UcStationButton.xaml`+`.xaml.cs` = **4 Dateien**.

### 2.5 `_NEUE_ORDNERSTRUKTUR/` — leeres Gerüst, kein Code
Auf der Festplatte **vollständig leer**. Im `.csproj` nur als **46 leere `<Folder Include>`-
Einträge** (Zeilen 1107–1152) vorhanden — keine einzige `Compile`- oder `Page`-Datei.
Es handelt sich um ein **angelegtes, aber nie gefülltes Ordnergerüst** (verworfener
Umbauversuch). Siehe auch Abschnitt 3 (Rückfrage).

→ **0 Code-Dateien**, aber 46 verwaiste Folder-Includes + leere Verzeichnisse.

---

## 3. Zusätzliche Funde (nicht in der Auftragsliste, aber klar tote Hinterlassenschaften)

Bei der Analyse sind weitere eindeutig ungenutzte Views aufgefallen. **Diese stehen nicht in
der ursprünglichen Verdachtsliste — bitte Freigabe, ob sie mit aufgeräumt werden sollen.**

### 3.1 `Displays/999_Project/alt/` — alte RFID-Seiten, ungenutzt
Ordnername „alt". Alle 4 Seiten werden nirgends instanziiert; funktional ersetzt durch
`PgRFID_WT` / `PgRFID_IND` in `Displays/999_Project/10_Station/`.

| View | Dateien |
|---|---|
| `PgSt1_RFID_WT` | `.xaml` + `.xaml.cs` |
| `PgSt2_RFID_WT` | `.xaml` + `.xaml.cs` |
| `PgSt1_RFID_IND` | `.xaml` + `.xaml.cs` |
| `PgSt2_RFID_IND` | `.xaml` + `.xaml.cs` |

→ **8 Dateien** (Hinweis: diese Seiten verwenden zwar `UcLamp`, werden selbst aber nie geladen).

### 3.2 `Displays/OverviewTemplateTest` — Test-/Vorlagenseite, ungenutzt
`OverviewTemplateTest.xaml` + `.xaml.cs` — nie instanziiert, Name deutet auf Testseite hin.
→ **2 Dateien**.

### 3.3 Leere Folder-Includes
- `UserControls\Display UCs\` (`.csproj` Z. 1106) — leerer Ordner.

---

## 4. Unklare Fälle / Rückfragen (vor dem Löschen klären)

1. **`_NEUE_ORDNERSTRUKTUR/` — entfernen oder behalten?**
   Aus dem Code eindeutig: leeres Gerüst ohne Inhalt (verworfener/pausierter Umbau). Da der
   Auftrag „keinen Total-Umbau" vorsieht, lautet die Empfehlung **entfernen** (Ordner + die 46
   `<Folder Include>`-Einträge). **Rückfrage:** Soll das Gerüst als geplante künftige Struktur
   erhalten bleiben, oder weg?

2. **`UserControls/Frank/` — nur Teil-Löschung bestätigen.**
   Nur `UcBarValue` + `UcStationButton` löschen, Ordner + restliche 3 Controls bleiben. Bitte
   bestätigen, dass diese Teil-Bereinigung gewünscht ist (statt Ordner anfassen).

3. **Zusätzliche Funde (Abschnitt 3) mit aufräumen?**
   `Displays/999_Project/alt/` (8 Dateien), `OverviewTemplateTest` (2 Dateien) und der leere
   Ordner `UserControls\Display UCs\` sind nicht Teil der ursprünglichen Verdachtsliste.
   **Rückfrage:** mit entfernen oder bewusst stehen lassen?

4. **Build-Verifikation.** Die statische Analyse ist eindeutig, ein erfolgreicher Build nach
   dem Entfernen ist aber erst in Aufgabe 2 möglich/vorgesehen. Insbesondere die 3
   kompilierten HOMS/REEB-Klassen werden vor dem endgültigen Entfernen über einen Rebuild
   abgesichert.

---

## 5. Geschätzte Code-Reduktion

### Sicher (genannte Verdachtskandidaten, eindeutig tot)
| Bereich | Dateien |
|---|---|
| `UserControls/Alt/` (ganz) | 10 |
| `PLC_Com_Classes/Nicht verwendet (HOMS, REEB)/` (ganz) | 5 |
| `SystemPages/HORSCH Archiv/` (ganz) | 8 |
| `UserControls/Frank/` (nur UcBarValue + UcStationButton) | 4 |
| **Zwischensumme** | **27 Dateien** |

### Zusätzlich (Abschnitt 3, nach Freigabe)
| Bereich | Dateien |
|---|---|
| `Displays/999_Project/alt/` | 8 |
| `Displays/OverviewTemplateTest` | 2 |
| **Zwischensumme** | **10 Dateien** |

### Sonstiges (kein Code)
- `_NEUE_ORDNERSTRUKTUR/`: 0 Code-Dateien, aber 46 leere `<Folder Include>` + leere Verzeichnisse.
- `UserControls\Display UCs\`: 1 leeres Verzeichnis (1 Folder-Include).

**Gesamt-Potenzial: ca. 37 Code-Dateien** (27 sicher + 10 nach Freigabe) zzgl. Bereinigung
von ~47 verwaisten Folder-Includes im `.csproj`.

---

## 6. Empfehlung für Aufgabe 2 (Commit-Aufteilung)

Sofern freigegeben, schlage ich getrennte, logische Commits vor:
1. `UserControls/Alt/` entfernen (+ csproj)
2. `PLC_Com_Classes/Nicht verwendet (HOMS, REEB)/` entfernen (+ csproj)
3. `SystemPages/HORSCH Archiv/` entfernen (+ csproj)
4. `UserControls/Frank/`: UcBarValue + UcStationButton entfernen (+ csproj)
5. `_NEUE_ORDNERSTRUKTUR/` + leere Folder-Includes entfernen (csproj)
6. (optional, nach Freigabe) Zusätzliche Funde aus Abschnitt 3 entfernen

Nach jedem Schritt bzw. spätestens am Ende: Rebuild zur Absicherung.
