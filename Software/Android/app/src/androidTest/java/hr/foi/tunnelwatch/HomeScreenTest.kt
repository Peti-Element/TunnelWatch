package hr.foi.tunnelwatch

import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithTag
import org.junit.Rule
import org.junit.Test

class HomeScreenTest {

    @get:Rule val compose = createComposeRule()

    @Test
    fun `shows as many table rows as the settings ask for`() {
        compose.setContent { HomeScreen(AppSettings(mockRowCount = 3)) }

        compose.onNodeWithTag("mock_row_2").assertExists()
        compose.onNodeWithTag("mock_row_3").assertDoesNotExist()
    }
}