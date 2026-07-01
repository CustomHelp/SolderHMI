# Error-Handling Audit (Branch: feature/bugfix-errorhandling)

Vollständige Liste der try/catch-Stellen mit leerem, nur-kommentiertem oder still
schluckendem catch-Block. Grundlage: `catch {}`, `catch {;}`, `catch { //… }` projektweit
(exakte Zeilen per Suche verifiziert), ergänzt um bestätigte Schluck-Stellen.

> **Status: Zur Freigabe.** Umsetzung erst nach deinem OK.

## Vorhandene Mechanismen (werden konsistent weiterverwendet, KEIN neues Framework)
- **`MessageBox.Show(...)`** — das Standard-Bediener-Feedback im Projekt (bereits in vielen catches genutzt).
- **`System.Diagnostics.Debug.WriteLine(ex.Message)`** — vorhandene Konvention für nicht-modale/zyklische Diagnose (z.B. `StCylinder`, `WindowInputNum`).
- **`DBLog.Handler`** (`DBLogger`) — **domänenspezifisch** (Parameter-/Manual-/User-Log in die DB), **kein** Exception-Logger → für Fehler-Handling NICHT geeignet.
- **`MessageArchivist`** — vollständig auskommentiert (toter Code), nicht nutzbar.

## Vorgeschlagener Mindeststandard (Legende)
- **[LOG]** → `catch (Exception ex) { System.Diagnostics.Debug.WriteLine("<Kontext>: " + ex.Message); }`
  Für zyklische / nicht bediener-relevante Stellen — v.a. **zyklische ADS-Wertkonvertierung** (MessageBox würde das HMI mit modalen Dialogen fluten).
- **[MSG]** → zusätzlich `MessageBox.Show(...)` mit klarer Meldung.
  Für **einmalige, bediener-relevante** Aktionen (DB laden/speichern, ADS-Einmaloperationen, Login, Benutzerverwaltung).
- **[KEEP]** → harmlos: minimaler catch **mit begründendem Kommentar** belassen
  (UI-Animation/kosmetisch, oder toter catch weil `as`-Cast nie wirft).

---

## A) ADS-/SPS-Kommunikation — besonders sorgfältig

### `ADS_Com/ADSConnection.cs`
| Zeile | Kat. | try-Zweck | ADS | Empfehlung |
|------:|:----:|-----------|:---:|------------|
| 118–121 | a (leer) | `ItemChangeEvent`: Handle in `ItemList` via FindIndex zuordnen | ja | **[LOG]** – Handle nicht gefunden protokollieren |
| 201–204 | a (leer) | `RemoveItem`: `DeleteDeviceNotification` beim Deregistrieren | ja | **[LOG]** – stilles Fehlschlagen des Abmeldens sichtbar machen |

### `PLC_Com_Classes/*` — zyklische ADS-Wertkonvertierung (`Convert.ChangeType` in ItemChanged)
Alle **[LOG]** (hochfrequent → **kein** MessageBox):
| Datei | Zeile(n) | Kat. |
|-------|----------|:----:|
| `StParamPLCDB.cs` | 842, 890, 915, 940 | a |
| `StSensor.cs` | 268, 287 | a |
| `StDeviceOnOff.cs` | 484–486 | a |
| `StBeckhoffAxis.cs` | 1081–1084 | a |
| `StButton.cs` | 341–344 | b (;) |
| `StCounter.cs` | 344–347 | b (;) |
| `StOrder.cs` | 163–166 | b (;) |
| `999_Project/StHU5000.cs` | 618–621 | a |
| `999_Project/StRFID_IND.cs` | 346–349 | a |
| `999_Project/StKROSY.cs` | 804, 819, 846 | b (;) |
| `999_Project/StRFID_WT.cs` | 1102, 1114, 1168 | b (;) |

---

## B) Datenbank / Helper / Ablauflogik

| Datei | Zeile | Kat. | try-Zweck | Empfehlung |
|-------|------:|:----:|-----------|------------|
| `Helper/DBOrderHandler.cs` | 50–53 | b (;) | Höchste Auftrags-Warteschlangennummer aus DB ermitteln | **[MSG]** (DB, auftragsrelevant) |
| `Helper/DBParamHandler.cs` | 654–657 | b (;) | Parameter-Verarbeitung | **[LOG]** |
| `Helper/DBParamHandler.cs` | 765–768 | a | Parameter-Verarbeitung | **[LOG]** |
| `Helper/GlobalFunc.cs` | 216–219 | b (;) | `DeregisterPgOrder` äußerer catch (innerer meldet bereits) | **[LOG]** |
| `Helper/GlobalFunc.cs` | 279, 290, 300, 311, 321, 333 | b (;) | Typ-Erkennung via `item as StParamPLCDB<T>` | **[KEEP]** – `as` wirft nicht → toter catch; Kommentar/Entfernen |
| `MainWindow.xaml.cs` | 811–814 | b (;) | Inneres catch bei `db_actual`-Initialisierung | **[LOG]** (äußeres meldet bereits per MessageBox) |
| `MainWindow.xaml.cs` | 1116–1119 | a | Inneres catch beim Default-User-Login | **[LOG]** (äußeres meldet bereits) |
| `MainWindow.xaml.cs` | 1524–1527 | b (;) | `BtnEvaluation1_Click` Navigations-Shortcut | **[KEEP]** – reine Navi-Bequemlichkeit; Kommentar |
| `MainWindow.xaml.cs` | 1538–1541 | b (;) | `BtnEvaluation2_Click` Navigations-Shortcut | **[KEEP]** – wie oben |
| `MainWindow.xaml.cs` | 1607–1610 | b (Komm.) | Datei-Dropdown: Ordner ggf. leer/nicht lesbar | **[KEEP]** – bereits dokumentiert harmlos |

---

## C) Benutzer / Login / Popups

| Datei | Zeile | Kat. | try-Zweck | Empfehlung |
|-------|------:|:----:|-----------|------------|
| `SystemPages/PopUps/User/Window_LogIn.xaml.cs` | 268 | a | User-Buttons in `Window_Loaded` deaktivieren (Schleife) | **[LOG]** |
| `SystemPages/PopUps/User/Window_LogIn.xaml.cs` | 293–296 | b (;) | Benutzer-Info/Level beim Laden ermitteln | **[LOG]** |
| `SystemPages/PopUps/User/Window_DeleteUser.xaml.cs` | 54 | a | Benutzer aus DB löschen | **[MSG]** (bediener-relevante DB-Aktion) |
| `SystemPages/PopUps/Order/WindowPopUpNewOrder.xaml.cs` | 70–73 | b (;) | Neuen Auftrag anlegen/vorbelegen | **[MSG]** |
| `Displays/900_Settings/PgUserManagement.xaml.cs` | 95–98 | b (;) | Benutzerverwaltung (DB) | **[MSG]** |
| `SystemPages/PopUps/WindowInputVirtualKeyboard.xaml.cs` | 755–756 | b (;) | Virtuelle Tastatur (UI) | **[KEEP]** – kosmetisch |
| `SystemPages/PopUps/WindowInputNum.xaml.cs` | 133, 144 | b (;) | Fallback-Init dynamischer Settings (V1/V2) | **[KEEP]** – dokumentierter Fallback |

---

## D) Anzeige / UserControls

| Datei | Zeile | Kat. | try-Zweck | Empfehlung |
|-------|------:|:----:|-----------|------------|
| `Displays/100_Machine/PgMessageArchiv.xaml.cs` | 170, 210, 375, 387, 399, 411, 423 | b (;) | Meldearchiv: DB-Abfrage/Sortierung/Filter | **[LOG]** |
| `Displays/200_Order/PgOrder.xaml.cs` | 355–358 | b (;) | Auftragsanzeige-Aktualisierung | **[LOG]** |
| `UserControls/Status/UcMessage.xaml.cs` | 357–360, 419–422 | b (;) | Meldungs-Registrierung / OP-Anzeige | **[LOG]** |
| `UserControls/Parameters/UcParamDataGrid.xaml.cs` | 295–296 | a | Ausgewählte Parametersatz-ID ermitteln | **[LOG]** |
| `UserControls/Parameters/UcParamDataGrid.xaml.cs` | 723 | b (;) | Parameter-Filter/Anzeige | **[LOG]** |
| `UserControls/Parameters/UcParamDataGrid.xaml.cs` | 898–901 | a (`ThreadAbortException`) | Hintergrund-Ladethread wird abgebrochen | **[KEEP]** – ThreadAbort ist erwartet; Kommentar |
| `UserControls/Parameters/UcInOutsDatagrid.xaml.cs` | 95, 110 | b (;) | IO-Datagrid Aktualisierung | **[LOG]** |
| `UserControls/Navigation/UcNavWorksp.xaml.cs` | 201–204 | b (;) | Auto-Auswahl 2. Nav-Reihe (kann leer sein) | **[KEEP]** – leere 2. Reihe ist zulässig; Kommentar |
| `UserControls/Devices/Cylinder/UcGrCylinder.xaml.cs` | 159, 173 | b (;) | Zylinder-Grafik/Animation | **[KEEP]** – UI-Animation, darf ausbleiben |
| `UserControls/Devices/Valves/UcGrValve.xaml.cs` | 150 | b (;) | Ventil-Grafik/Animation | **[KEEP]** – UI-Animation |

---

## E) Zu verifizieren
| Datei | Zeile | Kat. | Hinweis |
|-------|------:|:----:|---------|
| `Displays/100_Machine/PageHome.xaml.cs` | 730–731 | a | Alt-Namespace `HOPA_HMI` – vor Änderung prüfen, ob überhaupt im Build kompiliert. Falls ja: **[LOG]** (OP-Statusbild). |

---

## Zusammenfassung
- **Kategorie a (leer):** ~14 Stellen · **Kategorie b (nur `;`/Kommentar):** ~40 Stellen
- **[LOG]:** ~34 · **[MSG]:** ~5 · **[KEEP] (harmlos, begründet):** ~12
- Schwerpunkt & Sorgfalt bei **ADS-Kommunikation** (Abschnitt A): zyklische Stellen bekommen `Debug.WriteLine` (kein MessageBox-Sturm), Einmal-Operationen ggf. MessageBox.
- catch-Blöcke, die bereits per `MessageBox`/`throw`/sinnvollem Fallback reagieren, sind **nicht** in dieser Liste.

**Bitte freigeben** — danach setze ich es (in separaten, gebauten Commits) um.
