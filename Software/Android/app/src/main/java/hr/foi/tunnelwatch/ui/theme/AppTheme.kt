package hr.foi.tunnelwatch.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.unit.Density
import hr.foi.tunnelwatch.AppSettings

@Composable
fun AppTheme(settings: AppSettings, content: @Composable () -> Unit) {
    val dark = settings.useDarkTheme(isSystemInDarkTheme())
    val d = LocalDensity.current
    MaterialTheme(colorScheme = if (dark) darkColorScheme() else lightColorScheme()) {
        CompositionLocalProvider(
            LocalDensity provides Density(d.density, d.fontScale * settings.fontScale),
            content = content,
        )
    }
}