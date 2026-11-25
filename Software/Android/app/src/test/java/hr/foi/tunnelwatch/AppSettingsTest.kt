package hr.foi.tunnelwatch

import org.junit.Assert.assertEquals
import org.junit.Test

class AppSettingsTest {

    @Test
    fun `defaults are system theme and normal font size`() {
        val settings = AppSettings()

        assertEquals(ThemeMode.SYSTEM, settings.themeMode)
        assertEquals(1.0f, settings.fontScale, 0.0f)
    }
}