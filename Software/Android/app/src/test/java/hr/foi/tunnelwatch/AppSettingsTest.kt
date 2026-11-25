package hr.foi.tunnelwatch

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class AppSettingsTest {

    @Test
    fun `defaults are system theme, normal font size, 10 rows and 30 words`() {
        val settings = AppSettings()

        assertEquals(ThemeMode.SYSTEM, settings.themeMode)
        assertEquals(1.0f, settings.fontScale, 0.0f)
        assertEquals(10, settings.mockRowCount)
        assertEquals(30, settings.loremWords)
    }

    @Test
    fun `SYSTEM theme follows the device setting`() {
        val settings = AppSettings(themeMode = ThemeMode.SYSTEM)

        assertTrue(settings.useDarkTheme(systemIsDark = true))
        assertFalse(settings.useDarkTheme(systemIsDark = false))
    }

    @Test
    fun `LIGHT theme is never dark`() {
        val settings = AppSettings(themeMode = ThemeMode.LIGHT)

        assertFalse(settings.useDarkTheme(systemIsDark = true))
    }

    @Test
    fun `DARK theme is always dark`() {
        val settings = AppSettings(themeMode = ThemeMode.DARK)

        assertTrue(settings.useDarkTheme(systemIsDark = false))
    }

    @Test
    fun `phase 2 defaults show everything in the default style`() {
        val settings = AppSettings()

        assertEquals(AccentColor.TEAL, settings.accentColor)
        assertEquals(TableDensity.COMFORTABLE, settings.tableDensity)
        assertEquals(setOf("id", "name", "status", "date", "amount"), settings.visibleColumns)
        assertTrue(settings.showFiller)
        assertTrue(settings.showTable)
    }
}