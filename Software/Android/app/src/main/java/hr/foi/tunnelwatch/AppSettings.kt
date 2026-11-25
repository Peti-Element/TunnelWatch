package hr.foi.tunnelwatch

enum class ThemeMode(val label: String) { SYSTEM("System"), LIGHT("Light"), DARK("Dark") }
enum class AccentColor { TEAL, INDIGO, ROSE, AMBER }
enum class TableDensity { COMFORTABLE, COMPACT }

data class AppSettings(
    val themeMode: ThemeMode = ThemeMode.SYSTEM,
    val fontScale: Float = 1.0f,
    val mockRowCount: Int = 10,
    val loremWords: Int = 30,
    val accentColor: AccentColor = AccentColor.TEAL,
    val tableDensity: TableDensity = TableDensity.COMFORTABLE,
    val visibleColumns: Set<String> = setOf("id", "name", "status", "date", "amount"),
    val showFiller: Boolean = true,
    val showTable: Boolean = true,
) {
    fun useDarkTheme(systemIsDark: Boolean) = when (themeMode) {
        ThemeMode.SYSTEM -> systemIsDark
        ThemeMode.LIGHT -> false
        ThemeMode.DARK -> true
    }
}