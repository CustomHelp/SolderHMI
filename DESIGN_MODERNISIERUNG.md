# Design-Modernisierung (Branch: feature/design-modernisierung)

Umstellung vom "Lila-überall"-Look auf **Dunkelgrau (#2D2D2D) als Flächenfarbe**
mit **Lila (#782182) und Türkis (#00AEC8) als Akzente**.

## Zentrale Resources (CHP_Style_2020.xaml)
| Resource | Wert | Zweck |
|----------|------|-------|
| `Brand_DarkColor` / `Brand_Dark` | `#FF2D2D2D` | Header/Navigation/Dialog-Header (neu) |
| `Brand_PrimaryFaint` | `#33782182` | Hintergrund aktiver Menüpunkt (rgba 0.2) |
| `Brand_PrimaryColor` / `Brand_Primary` | `#FF782182` | Lila – jetzt **nur** Akzent |
| `Brand_SecondaryColor` / `Brand_Secondary` | `#FF00AEC8` | Türkis – Hover/Akzent |

## Geänderte Bereiche
1. **MainWindow-Header** (`MainWindow.xaml`): Hintergrund → `Brand_Dark`, neue 3px-Lila-Trennlinie unten.
2. **Linke Navigation** (`RENA_NavButtonStyle`): Hintergrund → `Brand_Dark`; Hover → Türkis; **aktiver Menüpunkt** neu: 3px-Lila-Balken links + `Brand_PrimaryFaint`-Hintergrund (gesetzt in `GlobalFunc.RefreshNavVert` via `Tag="active"`).
3. **Dropdown-Items** (`MainWindow.xaml.cs`): hardcodiertes `#66509B` → zentrale `Brand_Dark`-Resource.
4. **Alle Dialoge** (`RENA_PopUpStlye`, 14 Fenster): Header-Verlauf → `Brand_Dark` + 3px-Lila-Trennlinie; Dialog-Buttons (`RENA_ButtonStyle_PopUp`) → Dunkelgrau mit weißem Text + Türkis-Hover.
5. **Viewer-Popup-Header** (Logo/Excel/Pdf/Video): Lila- bzw. Schwarz-Header → `Brand_Dark` + 3px-Lila-Trennlinie.
6. **Reset-Zoom-Button** (`PgTempPowerDiagramm.xaml`): Lila → `Brand_Dark`.

## Bewusst unverändert
- Lila-Akzente: Progressbar-Verlauf (Lila→Türkis), LED-Hintergrund, aktiver Menüpunkt, Trennlinien.
- Semantische Farben: Status Rot/Orange/Gelb/MediumPurple, Navy `#013F84` für Überschriften.
- Content-/Maschinenansicht (weiße Karten), `GlobalFunc.GetComponentStateColor` Normal-State (bleibt Lila über `CHP_ColorBrush`).
- `WindowPopUpTemplate.xaml` (Navy-Header).
- Viewer-Body-Hintergrund (Schwarz→Indigo) der Dokument-Viewer bleibt als dunkler Rahmen bestehen.
