package hr.foi.tunnelwatch

import kotlinx.coroutines.flow.Flow

interface SettingsRepository {
    val settings: Flow<AppSettings>
    suspend fun setThemeMode(mode: ThemeMode)
    suspend fun setFontScale(scale: Float)
    suspend fun setMockRowCount(count: Int)
    suspend fun setLoremWords(words: Int)
    suspend fun setAccentColor(color: AccentColor)
}