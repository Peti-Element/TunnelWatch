# AI.md: TunnelWatch Android

Context for AI assistants working on this repo. Read this before changing code.

## Project

- App: TunnelWatch, Android
- Repo path: `Software\Android` (Windows, PowerShell)
- Package: `hr.foi.tunnelwatch`
- Language: Kotlin
- UI: Jetpack Compose with Material 3
- State: `ViewModel` with `StateFlow`
- Persistence: Jetpack DataStore (Preferences)
- Process: test-driven development in XP style

## Working rules

- Write the failing test first (red), then the minimum code to pass it (green), then refactor.
- One test at a time. Do not write a batch of tests up front.
- Commit at green. Use `feat:` for implementation commits and `test:` for test-only commits.
- Keep changes small and tied to one behavior.
- Use `Locale.US` for all number and date formatting.
- Build and test commands (run from `Software\Android`):
  - Unit tests: `.\gradlew.bat test`
  - UI tests: `.\gradlew.bat connectedAndroidTest` (needs an emulator or device)
- Do not use `git add .`. Stage files by name.

## Test locations

- `app\src\test\java\hr\foi\tunnelwatch\`: JVM unit tests. Fast. Run constantly.
  - `AppSettingsTest.kt`
  - `SettingsViewModelTest.kt`
  - `MockDataGeneratorTest.kt`
  - `MockTableTest.kt`
  - `DataStoreSettingsRepositoryTest.kt`
  - Helpers: `MainDispatcherRule.kt`, `FakeSettingsRepository.kt`
- `app\src\androidTest\java\hr\foi\tunnelwatch\`: Compose UI tests. Slow. Run less often.
  - `SettingsScreenTest.kt`
  - `HomeScreenTest.kt`
  - Tags: `theme_<MODE>`, `mock_row_<index>`

## Architecture

All main code is in `app\src\main\java\hr\foi\tunnelwatch\`.

| File | Role |
|---|---|
| `AppSettings.kt` | Settings data class. Also holds the enums `ThemeMode`, `AccentColor`, `TableDensity`. Has `useDarkTheme(systemIsDark)`. |
| `SettingsRepository.kt` | Interface: `settings: Flow<AppSettings>` plus one `suspend set...` per setting. |
| `DataStoreImplementation.kt` | `DataStoreSettingsRepository`, the DataStore implementation of the interface. |
| `SettingsStore.kt` | `Context.settingsDataStore` property. |
| `SettingsViewModel.kt` | Exposes `settings: StateFlow<AppSettings>`. Setters clamp values to safe ranges. |
| `SettingsScreen.kt` | Stateless screen. Theme radio buttons and sliders. Callbacks default to no-ops. |
| `HomeScreen.kt` | Stateless screen. Shows filler text and the mock table, from `AppSettings`. |
| `MockDataGenerator.kt` | `MockDataGenerator(seed)` with `table(rowCount)` and `paragraph(wordCount)`. `MockTable` has `onlyColumns(set)`. |
| `AppTheme.kt` | `AppTheme(settings) { }`. Applies dark or light mode and the font scale. |
| `MainActivity.kt` | Holds the `SettingsViewModel`. Toggles between Home and Settings with a button. |

## Settings and ranges

| Field | Type | Default | Range or values |
|---|---|---|---|
| `themeMode` | `ThemeMode` | `SYSTEM` | SYSTEM, LIGHT, DARK |
| `fontScale` | `Float` | `1.0` | 0.85 to 1.3 |
| `mockRowCount` | `Int` | `10` | 1 to 50 |
| `loremWords` | `Int` | `30` | 5 to 200 |
| `accentColor` | `AccentColor` | `TEAL` | TEAL, INDIGO, ROSE, AMBER |
| `tableDensity` | `TableDensity` | `COMFORTABLE` | COMFORTABLE, COMPACT |
| `visibleColumns` | `Set<String>` | all five | id, name, status, date, amount |
| `showFiller` | `Boolean` | `true` | |
| `showTable` | `Boolean` | `true` | |

DataStore keys: `theme_mode`, `font_scale`, `mock_row_count`, `lorem_words`, `accent_color`, `table_density`.

## How to add a new setting

Follow this order, with a red test first at each step that changes behavior:

1. Add the field and default to `AppSettings`. Test the default.
2. Add the setter to `SettingsRepository`.
3. Implement it in `FakeSettingsRepository` (test helper).
4. Implement it in `DataStoreSettingsRepository`: key, read with default, write. Test the round trip.
5. Add a clamped setter to `SettingsViewModel`. Test the clamping.
6. Add the UI control to `SettingsScreen`, with a `testTag` and a UI test.
7. Use the value in `HomeScreen` or `AppTheme`, with a UI test.

Conventions:
- Parse enums with `runCatching { X.valueOf(it) }.getOrNull()` and fall back to the default.
- Screens stay stateless. Pass `AppSettings` in and callbacks out.

## Status (2026-11-25)

Done:
- Mock data generator, with tests for row count, headers, and non-blank cells. Committed.
- Phase 1: settings screen, DataStore persistence, home screen with mock data. Committed on `feature/settings`, with a PR to `dev`.
- Phase 2, in progress on `feature/settings-phase-2`:
  - Cycle 1: phase 2 defaults. Written, not yet confirmed by running.
  - Cycle 2: `MockTable.onlyColumns`. Written, not yet confirmed.
  - Cycle 3: `setAccentColor` through the ViewModel and DataStore. Written, not yet confirmed.
  - Cycle 4: DataStore round trip for accent color and table density. Written, not yet confirmed.

Still to do in phase 2:
- DataStore persistence for `showFiller`, `showTable`, and `visibleColumns`. Use `booleanPreferencesKey` and `stringSetPreferencesKey`.
- ViewModel setters for density, visible columns, and the show toggles, with tests.
- Apply `accentColor` in `AppTheme` as the primary color.
- Home screen: table in a card, status shown as a colored chip, density and visible columns applied, sections hidden by the show toggles. Each with a UI test.
- Settings screen sections for the new options, with UI tests.

Known gaps:
- Phase 1 code was written before its tests. The tests were added afterward. This is the reason for the TDD rule above.
- `HomeScreen` uses seed `1L` for both table and filler. This is intentional. Seeds are not part of the product.
- If `androidTest` has a template `ExampleInstrumentedTest.kt`, delete it.