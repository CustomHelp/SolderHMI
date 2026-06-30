# Umstrukturierung UserControls — Mapping-Plan (Aufgabe 2.5, Schritt 1+2)

**Stand:** 2026-06-30
**Status:** PLANUNG. **Es wurde noch keine Datei verschoben.** Dieser Plan wartet auf
Freigabe der Mapping-Tabelle, bevor irgendetwas verschoben wird.

Bezieht sich auf den Stand **nach Aufgabe 2** (Cleanup), Branch `cleanup/aufgabe2-cleanup`.

> **Freigabe-Stand (2026-06-30):**
> 1. Namespace-Strategie: **Option A** (nur Ordner + .csproj-Pfade, Namespaces unverändert).
> 2. Devices mit Unterordnern (Cylinder/Valves/Sensors/Axis/OnOff/Passive): **ja**.
> 3. UcPressureReducingValve → **Valves**.
> 4. Mehrdeutige: UcStateBlock→Status, UcValueInOut→Status, UcAnimationBusy→Status,
>    UcGrSettingAnalogDINT→Parameters, UcCounter→Devices,
>    **UcOverviewDispl→Status** (generische Status-Übersicht: Ready/PowerOn/Error/SafetySTO,
>    keine LOET-Spezifik → Status; entschieden nach Code-Prüfung).
> 5. LOET-Controls als eigener `Project_LOET/`: **ja**.
> 6. UcImages: separater Commit am Ende: **ja**. Zusätzlich: relativer Bildpfad in
>    UcGrCylinder.xaml (`UcImages/Presswerkzeug6.PNG`) wird auf absoluten Pack-URI umgestellt.

---

## 0. ZENTRALE ENTSCHEIDUNG ZUERST: Namespace-Strategie

Die aktuellen C#-Namespaces der UserControls sind **stark uneinheitlich** — teils sogar
innerhalb desselben Ordners gemischt. Beispiele:
- `LOET_HMI` (flach): UcLamp, UcLampText, UcValue, UcButtonHandmenu, UcParamDataGrid, UcValveFunctions
- `LOET_HMI.UserControls`: UcCylinder, UcDeviceOnOff, UcSensBOOL, UcBeckhoffAxis, UcMessage, …
- `LOET_HMI.UserControls.Graphical_UCs`: alle RENA-Graphical-Controls
- `LOET_HMI.UserControls.HOPA_Devices`: **nur** UcCylinderMin (Rest des Ordners: `…UserControls`)
- `LOET_HMI.UserControls.Button`: UcButton (zusätzlich enthält die Datei eine zweite Klasse im Namespace `LOET_HMI`)
- `LOET_HMI.UserControls.LOET`: UcStation
- `LOET_HMI.UserControls.Display_UCs`: UcOverviewDispl

**Wichtig (classic .csproj):** Der Ordnerpfad ist in .NET **nicht** an den Namespace
gekoppelt. Dateien lassen sich verschieben, ohne den Namespace zu ändern.

### Option A — Nur Ordner + .csproj-Pfade ändern, Namespaces UNVERÄNDERT lassen ⭐ EMPFOHLEN
- Erreicht das Ziel (Auffindbarkeit über Ordner) vollständig.
- **Kein** Eingriff in `x:Class`, **keine** Änderung an `xmlns:`-Referenzen in den vielen
  Consumer-XAMLs (Displays/*, andere Controls). Die Namespace-Strings bleiben gleich →
  alle bestehenden `<LOET_HMI:UcLamp …>` / `clr-namespace:…`-Einbindungen bleiben gültig.
- Zu ändern: nur physische Dateiverschiebung + `<Compile>`/`<Page>`-Include-Pfade im
  `.csproj` (`<DependentUpon>` = Dateiname, bleibt gleich).
- **Risiko: gering.** Kleiner, gut reviewbarer Diff.

### Option B — Namespaces an die neue Ordnerstruktur angleichen
- Sauberer „from scratch", aber: berührt `x:Class` in jeder verschobenen `.xaml`, den
  `namespace` in jeder `.xaml.cs`, **und jede** Consumer-XAML (xmlns + Tag-Präfix) sowie
  C#-`using`-Direktiven. Bei der aktuellen Verbreitung (z. B. UcLamp in 8+ Dateien) ist das
  ein großer, fehleranfälliger Eingriff.
- **Risiko: hoch.** Großer Diff über viele Displays.

> **Rückfrage 1:** Option A oder B? Die folgende Tabelle ist für **Option A** ausgelegt
> (nur Pfade ändern). Bei Option B käme je Datei eine Namespace-/Consumer-Anpassung hinzu.
> Empfehlung: **Option A**. Optional könnte man die Namespaces in einem späteren, separaten
> Schritt vereinheitlichen.

---

## 1. Vorgeschlagene Zielstruktur

Die im Auftrag genannten 4 Kategorien decken nicht alle ~45 Controls ab. Vorschlag mit
Ergänzungen (zur Diskussion):

```
UserControls/
  Devices/        Geräte-/Prozess-Controls: Zylinder, Ventile, Achsen, Sensoren,
                  On/Off-Aktoren, passive Komponenten (ggf. mit Unterordnern, s. u.)
  Buttons/        Buttons
  Status/         Anzeige/Status: Lampen, Werte, Meldungen, Status-Blöcke
  Charts/         Diagramme
  Parameters/     Parameter-/Einstellungs-Datagrids (NEU – passt in keine der 4)
  Navigation/     Navigations-/Übersichts-Controls (NEU)
  Project_LOET/   Projekt-/kundenspezifische LOET-Controls (NEU – bewusst gruppiert lassen)
  Common/         Allgemeine Hilfs-Controls: Container, Counter, Zeit (NEU)
```

> **Rückfrage 2 (Devices-Granularität):** `Devices/` würde ~20 Controls umfassen. Vorschlag:
> Unterordner `Devices/Cylinder`, `Devices/Valves`, `Devices/Sensors`, `Devices/Axis`,
> `Devices/OnOff`, `Devices/Passive`. Alternativ flach. Tabelle unten zeigt die Unterordner-
> Variante; bei „flach" entfällt nur die letzte Pfadebene.

---

## 2. Mapping-Tabelle (alt → neu), pro Zielordner

Legende „Akt. NS" = aktueller Namespace (bei Option A unverändert).
Datei-Paare `.xaml` + `.xaml.cs` werden immer gemeinsam verschoben.

### Devices/Cylinder/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| HOPA Devices/UcCylinder.xaml(.cs) | Devices/Cylinder/ | LOET_HMI.UserControls | |
| HOPA Devices/UcCylinderMin.xaml(.cs) | Devices/Cylinder/ | LOET_HMI.UserControls.HOPA_Devices | abweichender NS im selben Altordner |
| RENA - Graphical UCs/UcGrCylinder.xaml(.cs) | Devices/Cylinder/ | …Graphical_UCs | |
| RENA - Graphical UCs/UcGrCylControl.xaml(.cs) | Devices/Cylinder/ | …Graphical_UCs | |
| RENA - Graphical UCs/UcPnCylControl.xaml(.cs) | Devices/Cylinder/ | …Graphical_UCs | |

### Devices/Valves/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| RENA - Graphical UCs/Uc4-3HydrValveAndAnSens.xaml(.cs) | Devices/Valves/ | …Graphical_UCs | Ventil + Analogsensor (Mischform) |
| RENA - Graphical UCs/DevicesOnOff/UcGrValve.xaml(.cs) | Devices/Valves/ | …Graphical_UCs | liegt aktuell unter „DevicesOnOff", ist aber ein Ventil |
| RENA - Graphical UCs/PassiveComponents/UcPressureReducingValve.xaml(.cs) | Devices/Valves/ | …Graphical_UCs | Druckminderer – Valves oder Passive? (s. Rückfrage 3) |
| RENA - Graphical UCs/UcValveFunctions.cs | Devices/Valves/ | LOET_HMI | reine Helper-Klasse (kein XAML) |

### Devices/Sensors/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| HOPA Devices/UcSensBOOL.xaml(.cs) | Devices/Sensors/ | LOET_HMI.UserControls | |
| RENA - Graphical UCs/UcGrSensorAnalog.xaml(.cs) | Devices/Sensors/ | …Graphical_UCs | |
| RENA - Graphical UCs/UcGrSensorDigital.xaml(.cs) | Devices/Sensors/ | …Graphical_UCs | |

### Devices/Axis/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| HOPA Devices/UcBeckhoffAxis.xaml(.cs) | Devices/Axis/ | LOET_HMI.UserControls | bindet UcValue + UcLampText ein (Option A: xmlns bleibt gültig) |

### Devices/OnOff/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| HOPA Devices/UcDeviceOnOff.xaml(.cs) | Devices/OnOff/ | LOET_HMI.UserControls | |
| RENA - Graphical UCs/DevicesOnOff/UcGrHeater.xaml(.cs) | Devices/OnOff/ | …Graphical_UCs | |
| RENA - Graphical UCs/DevicesOnOff/UcGrPumpe.xaml(.cs) | Devices/OnOff/ | …Graphical_UCs | |

### Devices/Passive/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| RENA - Graphical UCs/PassiveComponents/UcTank.xaml(.cs) | Devices/Passive/ | …Graphical_UCs | |
| RENA - Graphical UCs/PassiveComponents/UcDosingTank.xaml(.cs) | Devices/Passive/ | …Graphical_UCs | |
| RENA - Graphical UCs/PassiveComponents/UcEmergencyStop.xaml(.cs) | Devices/Passive/ | …Graphical_UCs | |
| RENA - Graphical UCs/PassiveComponents/UcSwitchSymbol.xaml(.cs) | Devices/Passive/ | …Graphical_UCs | |
| RENA - Graphical UCs/UcPipeTriangle.xaml(.cs) | Devices/Passive/ | …Graphical_UCs | Rohr-/Symbolelement |

### Buttons/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| Button/UcButton.xaml(.cs) | Buttons/ | LOET_HMI.UserControls.Button (+ 2. Klasse in LOET_HMI) | Datei enthält Zusatzklasse – bei Option B beachten |
| Button/UcButtonHandmenu.xaml(.cs) | Buttons/ | LOET_HMI | |

### Status/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| Frank/UcLamp.xaml(.cs) | Status/ | LOET_HMI | stark genutzt (8+ Consumer) |
| Frank/UcLampText.xaml(.cs) | Status/ | LOET_HMI | |
| Frank/UcValue.xaml(.cs) | Status/ | LOET_HMI | |
| UcMessage.xaml(.cs) | Status/ | LOET_HMI.UserControls | |
| RENA - Sonstiges/UcStateBlock.xaml(.cs) | Status/ | LOET_HMI.UserControls | Status-Block – Status oder Project? (Rückfrage 4) |
| UcValueInOut.xaml(.cs) | Status/ | LOET_HMI.UserControls | alternativ Parameters (Rückfrage 4) |
| UcAnimationBusy.xaml(.cs) | Status/ | LOET_HMI.UserControls | Busy-Indikator – Status oder Common? (Rückfrage 4) |

### Charts/
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| LiveCharts/UcLineChart.xaml(.cs) | Charts/ | LOET_HMI.UserControls | (Ordner praktisch unverändert) |

### Parameters/ (NEU)
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| UcParamDataGrid.xaml(.cs) | Parameters/ | LOET_HMI | |
| RENA - Sonstiges/UcInOutsDatagrid.xaml(.cs) | Parameters/ | LOET_HMI.UserControls | |
| RENA - Graphical UCs/UcGrSettingAnalogDINT.xaml(.cs) | Parameters/ | …Graphical_UCs | Einstell-Control – Parameters oder Devices? (Rückfrage 4) |

### Navigation/ (NEU)
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| UcNavWorksp.xaml(.cs) | Navigation/ | LOET_HMI.UserControls | |
| RENA - Sonstiges/UcOverviewDispl.xaml(.cs) | Navigation/ | LOET_HMI.UserControls.Display_UCs | Übersicht – Navigation oder Project? (Rückfrage 4) |

### Project_LOET/ (NEU — projektspezifisch, bewusst gruppiert)
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| LOET/UcHU5000.xaml(.cs) | Project_LOET/ | LOET_HMI.UserControls | |
| LOET/LOET_RFID.cs | Project_LOET/ | LOET_HMI.UserControls | Helper-Klasse |
| LOET/KROSY/UcStation.xaml(.cs) | Project_LOET/KROSY/ | LOET_HMI.UserControls.LOET | |
| LOET/RFID_WT/UcRFID_WT.xaml(.cs) | Project_LOET/RFID_WT/ | LOET_HMI.UserControls | |
| LOET/RFID_WT/UcWTVariant.xaml(.cs) | Project_LOET/RFID_WT/ | LOET_HMI.UserControls | |
| LOET/RFID_IND/UcRFID_IND.xaml(.cs) | Project_LOET/RFID_IND/ | LOET_HMI.UserControls | |

> **Rückfrage 5:** LOET-Controls sind kundenspezifisch und schon sinnvoll gruppiert.
> Vorschlag: als `Project_LOET/` (bzw. einfach `LOET/` belassen und nur umbenennen)
> zusammenhalten, **nicht** in Devices/Status zerstreuen. OK so?

### Common/ (NEU)
| Datei (aktuell) | Neuer Pfad | Akt. NS | Hinweis |
|---|---|---|---|
| UcContainer.xaml(.cs) | Common/ | LOET_HMI.UserControls | |
| UcCounter.xaml(.cs) | Common/ | LOET_HMI.UserControls | alternativ Status |
| TimeControl.xaml(.cs) | Common/ | LOET_HMI.UserControls | |
| UcTimePickerCHP.xaml(.cs) | Common/ | LOET_HMI.UserControls | |

---

## 3. Sonderfall: Bild-Ressourcen `RENA - Graphical UCs/UcImages/`

Im `.csproj` sind unter `UserControls\RENA - Graphical UCs\UcImages\` mehrere `<Resource>`-
Bilder registriert (Presswerkzeug*, PW.png, Rena_Logo*, …). Diese werden von den Graphical-
Controls referenziert.

> **Rückfrage 6:** Beim Verschieben der Graphical-Controls nach `Devices/*`:
> - UcImages mitverschieben (z. B. nach `Devices/_Images/`) **und** die `<Resource>`-Pfade
>   im `.csproj` sowie etwaige Bildpfade in den Control-XAMLs nachziehen, **oder**
> - UcImages an Ort und Stelle belassen?
>
> Empfehlung: in einem **separaten** Commit behandeln, nachdem die Controls verschoben sind,
> um den Diff klein zu halten. Vor dem eigentlichen Verschieben prüfe ich die konkreten
> Bild-Referenzen in den betroffenen XAMLs.

---

## 4. Offene Entscheidungen / Rückfragen (Zusammenfassung)

1. **Namespace-Strategie:** Option A (nur Pfade, empfohlen) oder B (Namespaces angleichen)?
2. **Devices-Granularität:** Unterordner (Cylinder/Valves/Sensors/Axis/OnOff/Passive) oder flach?
3. **UcPressureReducingValve:** Valves oder Passive?
4. **Einsortierung mehrdeutiger Controls:** UcStateBlock, UcValueInOut, UcAnimationBusy,
   UcGrSettingAnalogDINT, UcCounter, UcOverviewDispl — Vorschläge stehen in der Tabelle,
   bitte bestätigen oder korrigieren.
5. **LOET-Controls:** als `Project_LOET/` zusammenhalten (empfohlen) oder anders?
6. **UcImages:** mitverschieben (+ Resource-Pfade/XAML anpassen) oder belassen?

---

## 5. Umsetzungsplan (NACH Freigabe der Tabelle)

Mehrere kleine Commits, ein Commit je Zielordner. Pro Commit:
1. Dateien physisch verschieben (`git mv`, erhält Historie).
2. Bei Option A: nur `<Compile>`/`<Page>`-Include-Pfade im `.csproj` anpassen.
   Bei Option B zusätzlich: `x:Class`, `namespace`, alle Consumer-`xmlns`/Präfixe/`using`.
3. `Rebuild` zur Absicherung.
4. Commit mit klarer Beschreibung.

Vorgeschlagene Commit-Reihenfolge: Charts → Buttons → Status → Parameters → Navigation →
Common → Devices (ggf. je Unterordner) → Project_LOET → (optional) UcImages.

**Ich verschiebe nichts, bevor diese Tabelle freigegeben ist.**
