package hr.foi.tunnelwatch

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.RadioButton
import androidx.compose.material3.Slider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.testTag
import androidx.compose.ui.semantics.Role
import androidx.compose.foundation.selection.selectable
import androidx.compose.ui.unit.dp
import kotlin.math.roundToInt

@Composable
fun SettingsScreen(
    settings: AppSettings,
    onThemeModeChange: (ThemeMode) -> Unit,
    onMockRowCountChange: (Int) -> Unit = {},
    onLoremWordsChange: (Int) -> Unit = {},
) {
    Column(Modifier.padding(16.dp)) {
        Text("Theme", style = MaterialTheme.typography.titleMedium)
        ThemeMode.entries.forEach { mode ->
            val selected = settings.themeMode == mode
            Row(
                Modifier
                    .fillMaxWidth()
                    .selectable(selected, onClick = { onThemeModeChange(mode) }, role = Role.RadioButton)
                    .testTag("theme_${mode.name}")
                    .padding(8.dp),
                verticalAlignment = Alignment.CenterVertically,
            ) {
                RadioButton(selected = selected, onClick = null)
                Text(mode.label)
            }
        }

        Spacer(Modifier.height(24.dp))
        Text("Home page content", style = MaterialTheme.typography.titleMedium)

        Text("Table rows: ${settings.mockRowCount}")
        Slider(
            value = settings.mockRowCount.toFloat(),
            onValueChange = { onMockRowCountChange(it.roundToInt()) },
            valueRange = 1f..50f,
        )

        Text("Filler words: ${settings.loremWords}")
        Slider(
            value = settings.loremWords.toFloat(),
            onValueChange = { onLoremWordsChange(it.roundToInt()) },
            valueRange = 5f..200f,
        )
    }
}