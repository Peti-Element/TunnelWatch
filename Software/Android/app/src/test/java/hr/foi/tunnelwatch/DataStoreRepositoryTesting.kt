package hr.foi.tunnelwatch

import androidx.datastore.preferences.core.PreferenceDataStoreFactory
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.test.TestScope
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test
import org.junit.rules.TemporaryFolder
import java.io.File

class DataStoreSettingsRepositoryTest {

    @get:Rule val tmp = TemporaryFolder()

    private fun TestScope.repo() = DataStoreSettingsRepository(
        PreferenceDataStoreFactory.create(scope = backgroundScope) {
            File(tmp.root, "settings.preferences_pb")
        }
    )

    @Test
    fun `accent color is saved and read back`() = runTest {
        val repo = repo()

        repo.setAccentColor(AccentColor.AMBER)

        assertEquals(AccentColor.AMBER, repo.settings.first().accentColor)
    }

    @Test
    fun `table density is saved and read back`() = runTest {
        val repo = repo()

        repo.setTableDensity(TableDensity.COMPACT)

        assertEquals(TableDensity.COMPACT, repo.settings.first().tableDensity)
    }
}