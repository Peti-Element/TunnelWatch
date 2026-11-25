package hr.foi.tunnelwatch

import org.junit.Assert.assertEquals
import org.junit.Test

class MockTableTest {

    @Test
    fun `keeps only the visible columns in their original order`() {
        val table = MockTable(
            headers = listOf("id", "name", "status"),
            rows = listOf(listOf("1", "Alpha", "Active")),
        )

        val filtered = table.onlyColumns(setOf("status", "id"))

        assertEquals(listOf("id", "status"), filtered.headers)
        assertEquals(listOf(listOf("1", "Active")), filtered.rows)
    }
}