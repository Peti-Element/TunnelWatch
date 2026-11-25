package hr.foi.tunnelwatch

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Surface
import androidx.compose.material3.TextButton
import androidx.compose.material3.Text
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.lifecycle.viewmodel.compose.viewModel
import hr.foi.tunnelwatch.ui.theme.AppTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            val vm: SettingsViewModel = viewModel {
                SettingsViewModel(DataStoreSettingsRepository(applicationContext.settingsDataStore))
            }
            val settings by vm.settings.collectAsStateWithLifecycle()
            var showSettings by remember { mutableStateOf(false) }

            AppTheme(settings) {
                Surface(Modifier.fillMaxSize()) {
                    Column(Modifier.fillMaxSize()) {
                        TextButton(onClick = { showSettings = !showSettings }) {
                            Text(if (showSettings) "Back" else "Settings")
                        }
                        Box(Modifier.weight(1f)) {
                            if (showSettings) {
                                SettingsScreen(
                                    settings,
                                    onThemeModeChange = vm::setThemeMode,
                                    onMockRowCountChange = vm::setMockRowCount,
                                    onLoremWordsChange = vm::setLoremWords,
                                )
                            } else {
                                HomeScreen(settings)
                            }
                        }
                    }
                }
            }
        }
    }
}