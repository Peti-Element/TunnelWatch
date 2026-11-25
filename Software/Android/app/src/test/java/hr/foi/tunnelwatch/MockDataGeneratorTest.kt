package hr.foi.tunnelwatch

import org.junit.Assert.assertEquals
import org.junit.Test

class MockDataGeneratorTest {

    @Test
    fun `generates the requested number of rows`() {
        val table = MockDataGenerator(seed = 1L).table(rowCount = 5)

        assertEquals(5, table.rows.size)
    }

    @Test
    fun `table has id, name, status, date and amount headers`() {
        val table = MockDataGenerator(seed = 1L).table(rowCount = 1)

        assertEquals(listOf("id", "name", "status", "date", "amount"), table.headers)
    }

    @Test
    fun `same seed produces the same table`() {
        val a = MockDataGenerator(seed = 42L).table(rowCount = 10)
        val b = MockDataGenerator(seed = 42L).table(rowCount = 10)

        assertEquals(a, b)
    }



}