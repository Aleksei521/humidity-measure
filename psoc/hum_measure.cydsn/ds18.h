/* ========================================
 *
 * Copyright YOUR COMPANY, THE YEAR
 * All Rights Reserved
 * UNPUBLISHED, LICENSED SOFTWARE.
 *
 * CONFIDENTIAL AND PROPRIETARY INFORMATION
 * WHICH IS THE PROPERTY OF your company.
 *
 * ========================================
*/
#include <cytypes.h>

struct scratchpad{
    uint8 temp_lsb;
    uint8 temp_msb;
    uint8 th;
    uint8 tl;
    uint8 config;
    uint8 reserve_0xFF;
    uint8 reserve;
    uint8 reserve_0x10;
    uint8 crc;
};

enum DS18_READY {DS18_DATA_NOT_READY=0, DS18_DATA_READY=1, DS18_CRC_ERROR, DS18_UART_ERROR};

void ini_ds18(void);
void get_id_ds18(void);
void set_scratchpad(void);
void get_scratchpad(void);
void start_measure_ds18(void);
enum DS18_READY get_ready(void);
int16 read_temperature(void);
uint64 read_id(void);
struct scratchpad *get_ptr_scratchpad(void);
void reset_uart(void);
/* [] END OF FILE */
