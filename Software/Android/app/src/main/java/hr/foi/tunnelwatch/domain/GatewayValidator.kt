package hr.foi.tunnelwatch.domain

enum class GatewayFieldError {
    ADDRESS_EMPTY,
    PORT_NOT_A_NUMBER,
    PORT_OUT_OF_RANGE,
}

/**
 * Rezultat provjere forme. Ako je isValid, address je očišćen od razmaka, a port je broj spreman za spremanje.
 */
data class GatewayValidationResult(
    val address: String,
    val port: Int?,
    val addressError: GatewayFieldError? = null,
    val portError: GatewayFieldError? = null,
) {
    val isValid: Boolean get() = addressError == null && portError == null
}

/**
 * Čista Kotlin logika bez Androida, pa se testira običnim JUnit testom (bez emulatora).
 */
object GatewayValidator {
    const val DEFAULT_PORT = 10000
    const val MIN_PORT = 1
    const val MAX_PORT = 65535

    fun validate(address: String, portText: String): GatewayValidationResult {
        val trimmedAddress = address.trim()
        val addressError = if (trimmedAddress.isEmpty()) GatewayFieldError.ADDRESS_EMPTY else null

        val trimmedPort = portText.trim()
        val port: Int?
        val portError: GatewayFieldError?
        if (trimmedPort.isEmpty()) {
            port = DEFAULT_PORT
            portError = null
        } else {
            port = trimmedPort.toIntOrNull()
            portError = when {
                port == null -> GatewayFieldError.PORT_NOT_A_NUMBER
                port !in MIN_PORT..MAX_PORT -> GatewayFieldError.PORT_OUT_OF_RANGE
                else -> null
            }
        }

        return GatewayValidationResult(
            address = trimmedAddress,
            port = if (portError == null) port else null,
            addressError = addressError,
            portError = portError,
        )
    }
}
