package hr.foi.tunnelwatch

import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.update

class FakeSettingsRepository : SettingsRepository {
    private val state = MutableStateFlow(AppSettings())

    override val settings: Flow<AppSettings> = state
    override suspend fun setThemeMode(mode: ThemeMode) = state.update { it.copy(themeMode = mode) }
    override suspend fun setFontScale(scale: Float) = state.update { it.copy(fontScale = scale) }
    override suspend fun setMockRowCount(count: Int) = state.update { it.copy(mockRowCount = count) }
    override suspend fun setLoremWords(words: Int) = state.update { it.copy(loremWords = words) }
    override suspend fun setAccentColor(color: AccentColor) = state.update { it.copy(accentColor = color) }
}