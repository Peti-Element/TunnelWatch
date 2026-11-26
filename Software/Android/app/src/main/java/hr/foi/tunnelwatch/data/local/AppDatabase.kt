package hr.foi.tunnelwatch.data.local

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase

/**
 * Jedna zajednička Room baza aplikacije. Kasnije ovdje dolaze i omiljeni signali:
 * nova entiteta u listi + povećanje version.
 */
@Database(
    entities = [GatewayEntity::class],
    version = 1,
    exportSchema = false,
)
abstract class AppDatabase : RoomDatabase() {

    abstract fun gatewayDao(): GatewayDao

    companion object {
        private const val DATABASE_NAME = "tunnelwatch.db"

        /** Poziva se jednom, iz AppContainera (ručni DI), koji drži jedinu instancu. */
        fun create(context: Context): AppDatabase =
            Room.databaseBuilder(
                context.applicationContext,
                AppDatabase::class.java,
                DATABASE_NAME,
            ).build()
    }
}
