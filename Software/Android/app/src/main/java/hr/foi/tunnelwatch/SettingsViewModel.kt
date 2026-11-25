package hr.foi.tunnelwatch

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch

class SettingsViewModel(private val repo: SettingsRepository) : ViewModel() {
    val settings: StateFlow<AppSettings> = repo.settings
        .stateIn(viewModelScope, SharingStarted.Eagerly, AppSettings())

    fun setThemeMode(mode: ThemeMode) {
        viewModelScope.launch { repo.setThemeMode(mode) }
    }

    fun setFontScale(scale: Float) {
        viewModelScope.launch { repo.setFontScale(scale.coerceIn(0.85f, 1.3f)) }
    }

    fun setMockRowCount(count: Int) {
        viewModelScope.launch { repo.setMockRowCount(count.coerceIn(1, 50)) }
    }

    fun setLoremWords(words: Int) {
        viewModelScope.launch { repo.setLoremWords(words.coerceIn(5, 200)) }
    }
    fun setAccentColor(color: AccentColor) {
        viewModelScope.launch { repo.setAccentColor(color) }
    }
}