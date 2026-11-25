package hr.foi.tunnelwatch

import androidx.compose.ui.test.assertIsNotSelected
import androidx.compose.ui.test.assertIsSelected
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithTag
import androidx.compose.ui.test.onNodeWithText
import androidx.compose.ui.test.performClick
import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test

class SettingsScreenTest {

    @get:Rule val compose = createComposeRule()

    @Test
    fun clickingDark_reportsDark() {
        var picked: ThemeMode? = null
        compose.setContent {
            SettingsScreen(AppSettings(), onThemeModeChange = { picked = it })
        }

        compose.onNodeWithText("Dark").performClick()

        assertEquals(ThemeMode.DARK, picked)
    }

    @Test
    fun currentThemeIsShownAsSelected() {
        compose.setContent {
            SettingsScreen(AppSettings(themeMode = ThemeMode.DARK), onThemeModeChange = {})
        }

        compose.onNodeWithTag("theme_DARK").assertIsSelected()
        compose.onNodeWithTag("theme_LIGHT").assertIsNotSelected()
    }

    @Test
    fun sliderLabelsShowCurrentValues() {
        compose.setContent {
            SettingsScreen(AppSettings(mockRowCount = 7, loremWords = 42), onThemeModeChange = {})
        }

        compose.onNodeWithText("Table rows: 7").assertExists()
        compose.onNodeWithText("Filler words: 42").assertExists()
    }
}