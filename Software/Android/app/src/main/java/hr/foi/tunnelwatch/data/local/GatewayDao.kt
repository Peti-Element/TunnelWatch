package hr.foi.tunnelwatch.data.local

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Update
import kotlinx.coroutines.flow.Flow

@Dao
interface GatewayDao {

    /** Flow: popis se sam osvježi svaki put kad se tablica promijeni (dodavanje, uređivanje, brisanje). */
    @Query("SELECT * FROM gateways ORDER BY name COLLATE NOCASE")
    fun observeAll(): Flow<List<GatewayEntity>>

    /** Za formu kod uređivanja; null ako gateway ne postoji. */
    @Query("SELECT * FROM gateways WHERE id = :id")
    suspend fun getById(id: Long): GatewayEntity?

    /** Vraća id novog retka. */
    @Insert
    suspend fun insert(gateway: GatewayEntity): Long

    @Update
    suspend fun update(gateway: GatewayEntity)

    @Delete
    suspend fun delete(gateway: GatewayEntity)
}
