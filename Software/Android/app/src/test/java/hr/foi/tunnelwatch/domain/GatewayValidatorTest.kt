package hr.foi.tunnelwatch.domain

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Test

class GatewayValidatorTest {

    @Test
    fun emptyAddress_isInvalid() {
        val result = GatewayValidator.validate(address = "", portText = "10000")
        assertFalse(result.isValid)
        assertEquals(GatewayFieldError.ADDRESS_EMPTY, result.addressError)
    }

    @Test
    fun blankAddress_onlySpaces_isInvalid() {
        val result = GatewayValidator.validate(address = "   ", portText = "10000")
        assertEquals(GatewayFieldError.ADDRESS_EMPTY, result.addressError)
    }

    @Test
    fun validAddress_isTrimmed() {
        val result = GatewayValidator.validate(address = "  192.168.0.100 ", portText = "10000")
        assertTrue(result.isValid)
        assertEquals("192.168.0.100", result.address)
    }

    @Test
    fun emptyPort_usesDefault10000() {
        val result = GatewayValidator.validate(address = "192.168.0.100", portText = "")
        assertTrue(result.isValid)
        assertEquals(10000, result.port)
    }

    @Test
    fun defaultPortConstant_is10000() {
        assertEquals(10000, GatewayValidator.DEFAULT_PORT)
    }

    @Test
    fun portBoundaries_1And65535_areValid() {
        assertEquals(1, GatewayValidator.validate("gw", "1").port)
        assertEquals(65535, GatewayValidator.validate("gw", "65535").port)
    }

    @Test
    fun port0_isOutOfRange() {
        val result = GatewayValidator.validate("gw", "0")
        assertEquals(GatewayFieldError.PORT_OUT_OF_RANGE, result.portError)
        assertNull(result.port)
    }

    @Test
    fun port65536_isOutOfRange() {
        val result = GatewayValidator.validate("gw", "65536")
        assertEquals(GatewayFieldError.PORT_OUT_OF_RANGE, result.portError)
    }

    @Test
    fun negativePort_isOutOfRange() {
        assertEquals(GatewayFieldError.PORT_OUT_OF_RANGE, GatewayValidator.validate("gw", "-5").portError)
    }

    @Test
    fun nonNumericPort_isNotANumber() {
        assertEquals(GatewayFieldError.PORT_NOT_A_NUMBER, GatewayValidator.validate("gw", "abc").portError)
    }

    @Test
    fun bothFieldsInvalid_reportsBothErrors() {
        val result = GatewayValidator.validate("", "99999")
        assertEquals(GatewayFieldError.ADDRESS_EMPTY, result.addressError)
        assertEquals(GatewayFieldError.PORT_OUT_OF_RANGE, result.portError)
    }
}
