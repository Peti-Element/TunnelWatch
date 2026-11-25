package hr.foi.tunnelwatch

import java.util.Locale
import kotlin.random.Random

data class MockTable(val headers: List<String>, val rows: List<List<String>>)

class MockDataGenerator(private val seed: Long) {

    fun table(rowCount: Int): MockTable {
        val random = Random(seed)
        val rows = List(rowCount) { index ->
            listOf(
                (index + 1).toString(),
                NAMES.random(random),
                STATUSES.random(random),
                String.format(Locale.US, "2026-%02d-%02d", random.nextInt(1, 13), random.nextInt(1, 29)),
                String.format(Locale.US, "%.2f", random.nextDouble(1.0, 500.0)),
            )
        }
        return MockTable(HEADERS, rows)
    }

    fun paragraph(wordCount: Int): String {
        val random = Random(seed)
        return List(wordCount) { WORDS.random(random) }.joinToString(" ")
    }

    companion object {
        val HEADERS = listOf("id", "name", "status", "date", "amount")
        private val NAMES = listOf("Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot")
        private val STATUSES = listOf("Active", "Pending", "Closed", "Failed")
        private val WORDS = listOf("lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit")
    }
}