package hr.foi.tunnelwatch

import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test

class SettingsViewModelTest {

    @get:Rule val mainDispatcherRule = MainDispatcherRule()

    private val repo = FakeSettingsRepository()
    // lazy: viewModelScope needs Dispatchers.Main, which the rule sets up first
    private val vm by lazy { SettingsViewModel(repo) }

    @Test
    fun `starts with defaults`() {
        assertEquals(AppSettings(), vm.settings.value)
    }

    @Test
    fun `changing theme updates state`() {
        vm.setThemeMode(ThemeMode.DARK)

        assertEquals(ThemeMode.DARK, vm.settings.value.themeMode)
    }

    @Test
    fun `font scale is clamped to a sane range`() {
        vm.setFontScale(5f)
        assertEquals(1.3f, vm.settings.value.fontScale, 0.0f)

        vm.setFontScale(0.1f)
        assertEquals(0.85f, vm.settings.value.fontScale, 0.0f)
    }

    @Test
    fun `table row count is clamped between 1 and 50`() {
        vm.setMockRowCount(0)
        assertEquals(1, vm.settings.value.mockRowCount)

        vm.setMockRowCount(99)
        assertEquals(50, vm.settings.value.mockRowCount)
    }

    @Test
    fun `filler word count is clamped between 5 and 200`() {
        vm.setLoremWords(1)
        assertEquals(5, vm.settings.value.loremWords)

        vm.setLoremWords(500)
        assertEquals(200, vm.settings.value.loremWords)
    }

    @Test
    fun `changing accent color updates state`() {
        vm.setAccentColor(AccentColor.ROSE)

        assertEquals(AccentColor.ROSE, vm.settings.value.accentColor)
    }
}