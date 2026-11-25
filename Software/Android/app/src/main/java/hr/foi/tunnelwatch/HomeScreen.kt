package hr.foi.tunnelwatch

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.testTag
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp

@Composable
fun HomeScreen(settings: AppSettings) {
    val table = remember(settings.mockRowCount) {
        MockDataGenerator(seed = 1L).table(settings.mockRowCount)
    }
    val filler = remember(settings.loremWords) {
        MockDataGenerator(seed = 1L).paragraph(settings.loremWords)
    }
    Column(Modifier.padding(16.dp).verticalScroll(rememberScrollState())) {
        Text(filler, style = MaterialTheme.typography.bodyLarge)
        Spacer(Modifier.height(16.dp))
        TableRow(table.headers, bold = true)
        HorizontalDivider()
        table.rows.forEachIndexed { index, row ->
            TableRow(row, Modifier.testTag("mock_row_$index"))
        }
    }
}

@Composable
private fun TableRow(cells: List<String>, modifier: Modifier = Modifier, bold: Boolean = false) {
    Row(modifier.fillMaxWidth().padding(vertical = 4.dp)) {
        cells.forEach { cell ->
            Text(
                cell,
                modifier = Modifier.weight(1f),
                style = MaterialTheme.typography.bodySmall,
                fontWeight = if (bold) FontWeight.Bold else FontWeight.Normal,
            )
        }
    }
}