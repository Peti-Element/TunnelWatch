package hr.foi.tunnelwatch.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey
import hr.foi.tunnelwatch.domain.GatewayValidator

/**
 * Jedan spremljeni gateway (red u tablici "gateways").
 * id = 0 znači "još nije spremljen" – Room tada sam dodijeli novi id (autoGenerate).
 */
@Entity(tableName = "gateways")
data class GatewayEntity(
    @PrimaryKey(autoGenerate = true) val id: Long = 0,
    val name: String,
    val address: String,
    val port: Int = GatewayValidator.DEFAULT_PORT,
)
