package hr.foi.tunnelwatch

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.floatPreferencesKey
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import kotlinx.coroutines.flow.map

class DataStoreSettingsRepository(
    private val store: DataStore<Preferences>
) : SettingsRepository {
    private val THEME = stringPreferencesKey("theme_mode")
    private val FONT_SCALE = floatPreferencesKey("font_scale")
    private val MOCK_ROWS = intPreferencesKey("mock_row_count")
    private val LOREM_WORDS = intPreferencesKey("lorem_words")
    private val ACCENT = stringPreferencesKey("accent_color")

    override val settings = store.data.map { p ->
        AppSettings(
            themeMode = p[THEME]?.let { runCatching { ThemeMode.valueOf(it) }.getOrNull() }
                ?: ThemeMode.SYSTEM,
            fontScale = p[FONT_SCALE] ?: 1.0f,
            mockRowCount = p[MOCK_ROWS] ?: 10,
            loremWords = p[LOREM_WORDS] ?: 30,
            accentColor = p[ACCENT]?.let { runCatching { AccentColor.valueOf(it) }.getOrNull() }
                ?: AccentColor.TEAL,
        )
    }

    override suspend fun setThemeMode(mode: ThemeMode) { store.edit { it[THEME] = mode.name } }
    override suspend fun setFontScale(scale: Float) { store.edit { it[FONT_SCALE] = scale } }
    override suspend fun setMockRowCount(count: Int) { store.edit { it[MOCK_ROWS] = count } }
    override suspend fun setLoremWords(words: Int) { store.edit { it[LOREM_WORDS] = words } }
    override suspend fun setAccentColor(color: AccentColor) { store.edit { it[ACCENT] = color.name } }
}