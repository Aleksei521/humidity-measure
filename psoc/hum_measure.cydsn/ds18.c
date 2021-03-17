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
#include "UART_DS.h"
#include "UART_DS_SPI_UART.h"
#include "UART_DS_SCB_IRQ.h"
#include "Clock_uart.h"
#include "ds18.h"

#define DIV_UART_9600 156
#define DIV_UART_115200 13


struct {
    uint64 id;
    int8  temperature;
    enum DS18_READY ready;
} ds18;

struct scratchpad volatile scratchpad_ds18;

volatile uint8 ds18_bit_cnt, ds18_tx_byte, ds18_byte_cnt, *ds18_data_ptr, ds18_rx_byte, ds18_crc;

unsigned char check_rx_error_ds18(uint32 data);
void send_bit_ds18(void);
void receive_bit_ds18(uint32 data);
uint8 calc_ds18_crc(uint8 data, uint8 crc);
void send_reset_ds18(void);

enum {IDLE, WR_SCRATCHPAD, RD_SCRATCHPAD, RD_ROM, START_CONVERT} STATE_DS18=IDLE;
enum {CMD_NOP, CMD_RESET, CMD_SKIP_ROM, CMD_READ, CMD_WRITE, CMD_READ_ROM, CMD_WAIT_CONVERT} CMD_DS18=CMD_NOP;
#define SKIP_ROM 0xCC
#define CONVERT_TEMP 0x44
#define READ_SCRATCHPAD 0xBE
#define WRITE_SCRATCHPAD 0x4E
#define READ_ROM 0x33

CY_ISR_PROTO(UART_HANDLER);

CY_ISR(UART_HANDLER)
{
    uint32 mask;
    uint32 uart_rx_data;
    mask=UART_DS_GetRxInterruptSource();
    if(mask==UART_DS_INTR_RX_OVERFLOW || mask==UART_DS_INTR_RX_UNDERFLOW || mask==UART_DS_INTR_RX_FRAME_ERROR)
    {
        STATE_DS18=IDLE;
        CMD_DS18=CMD_NOP;
        ds18.ready=DS18_UART_ERROR;
    }
    else
        if(mask == UART_DS_INTR_RX_NOT_EMPTY)
        {
            uart_rx_data = UART_DS_UartGetByte();
            switch(CMD_DS18)
            {
                case CMD_RESET:
                    if(uart_rx_data!=0xF0)
                    {
                        Clock_uart_SetDividerValue(DIV_UART_115200);
                        switch(STATE_DS18)
                        {
                            case RD_ROM:
                                CMD_DS18=CMD_READ_ROM;
                                ds18_tx_byte=READ_ROM;
                                break;
                            default:
                                CMD_DS18=CMD_SKIP_ROM;
                                ds18_tx_byte=SKIP_ROM;
                        }
                        ds18_bit_cnt=0;
                        send_bit_ds18();
                    }
                    else
                    {
                        CMD_DS18=CMD_NOP;
                        STATE_DS18=IDLE;
                    }
                    break;
                case CMD_READ_ROM:
                    if(ds18_bit_cnt!=8)
                    {
                        send_bit_ds18();
                    }
                    else
                    {
                        ds18_bit_cnt=0;
                        ds18_byte_cnt=0;
                        ds18_rx_byte=0;
                        CMD_DS18=CMD_READ;
                        ds18_data_ptr=(uint8*)&ds18.id;
                        UART_DS_SpiUartWriteTxData(0xFF);
                    }
                    break;
                case CMD_SKIP_ROM:
                    if(ds18_bit_cnt!=8)
                    {
                        send_bit_ds18();
                    }
                    else
                    {
                        CMD_DS18=CMD_WRITE;
                        ds18_bit_cnt=0;
                        ds18_byte_cnt=0;
                        switch(STATE_DS18)
                        {
                            case WR_SCRATCHPAD:
                                ds18_tx_byte=WRITE_SCRATCHPAD;
                                ds18_data_ptr=&scratchpad_ds18.temp_lsb;
                                break;
                            case RD_SCRATCHPAD:
                                ds18_tx_byte=READ_SCRATCHPAD;
                                ds18_data_ptr=&scratchpad_ds18.temp_lsb;
                                break;
                            case START_CONVERT:
                                ds18_tx_byte=CONVERT_TEMP;
                                break;
                            default: break;
                        }
                        send_bit_ds18();
                    }
                    break;
                case CMD_READ:
                    receive_bit_ds18(uart_rx_data);
                    if(ds18_bit_cnt!=8)
                    {
                        UART_DS_SpiUartWriteTxData(0xFF);
                    }
                    else
                    {
                        *(ds18_data_ptr++)=ds18_rx_byte;
                        switch(STATE_DS18)
                        {
                            case RD_SCRATCHPAD:
                                if(ds18_byte_cnt!=sizeof(scratchpad_ds18))
                                {
                                    ds18_bit_cnt=0;
                                    ds18_byte_cnt++;
                                    ds18_rx_byte=0;
                                    UART_DS_SpiUartWriteTxData(0xFF);
                                }
                                else
                                {
                                    int tmp_crc=0;
                                    uint8 *tmp_ptr=(uint8*)&scratchpad_ds18;
                                    for(uint8 i=0;i<sizeof(scratchpad_ds18);i++)
                                        tmp_crc=calc_ds18_crc(tmp_ptr[i],tmp_crc);
                                    if(!tmp_crc)                                        
                                        ds18.ready=DS18_DATA_READY;
                                    else
                                        ds18.ready=DS18_CRC_ERROR;
                                    CMD_DS18=CMD_NOP;
                                    STATE_DS18=IDLE;
                                }
                                break;
                            case RD_ROM:
                                if(ds18_byte_cnt!=sizeof(ds18.id))
                                {
                                    ds18_bit_cnt=0;
                                    ds18_byte_cnt++;
                                    ds18_rx_byte=0;
                                    UART_DS_SpiUartWriteTxData(0xFF);
                                }
                                else
                                {
                                    int tmp_crc=0;
                                    uint8 *tmp_ptr=(uint8*)&ds18.id;
                                    for(uint8 i=0;i<sizeof(ds18.id);i++)
                                        tmp_crc=calc_ds18_crc(tmp_ptr[i],tmp_crc);
                                    if(!tmp_crc)                                        
                                        ds18.ready=DS18_DATA_READY;
                                    else
                                        ds18.ready=DS18_CRC_ERROR;
                                    CMD_DS18=CMD_NOP;
                                    STATE_DS18=IDLE;
                                }
                                break;
                            default: break;
                        }
                    }
                    break;
                case CMD_WRITE:
                    if(ds18_bit_cnt!=8)
                    {
                        send_bit_ds18();
                    }
                    else
                    {
                        switch(STATE_DS18)
                        {
                            case WR_SCRATCHPAD:
                                if(ds18_byte_cnt!=sizeof(scratchpad_ds18))
                                {
                                    ds18_bit_cnt=0;
                                    ds18_byte_cnt++;
                                    ds18_tx_byte=*(ds18_data_ptr++);
                                    send_bit_ds18();
                                }
                                else
                                {
                                    STATE_DS18=IDLE;
                                    ds18.ready=DS18_DATA_READY;
                                    CMD_DS18=CMD_NOP;
                                }
                                break;
                            case RD_SCRATCHPAD:
                                CMD_DS18=CMD_READ;
                                ds18_bit_cnt=0;
                                ds18_rx_byte=0;
                                UART_DS_SpiUartWriteTxData(0xFF);
                                break;
                            case START_CONVERT:
                                CMD_DS18=CMD_WAIT_CONVERT;
                                UART_DS_SpiUartWriteTxData(0xFF);
                            default: break;
                        }
                    }
                    break;
                case CMD_WAIT_CONVERT:
                    if(uart_rx_data!=0xFF)
                    {
                        UART_DS_SpiUartWriteTxData(0xFF);
                    }
                    else
                    {
                        ds18.ready=DS18_DATA_READY;
                        STATE_DS18=IDLE;
                        CMD_DS18=CMD_NOP;
                    }
                        
                    break;
                default: break;
            }
        }
    UART_DS_ClearRxInterruptSource(mask);
}

void send_bit_ds18(void)
{
    if(ds18_tx_byte&1)
        UART_DS_SpiUartWriteTxData(0xFF);
    else
        UART_DS_SpiUartWriteTxData(0);
    ds18_bit_cnt++;
    ds18_tx_byte >>=1;
}

void receive_bit_ds18(uint32 data)
{
    ds18_rx_byte >>=1;
    ds18_bit_cnt++;
    if(data == 0xFF)
        ds18_rx_byte |=0x80;
}

unsigned char check_rx_error_ds18(uint32 data)
{
    switch(data)
    {
    case UART_DS_UART_RX_OVERFLOW:
    case UART_DS_UART_RX_UNDERFLOW:
    case UART_DS_UART_RX_FRAME_ERROR:
        return data;
    default: return 0;
    }
}

void ini_ds18(void)
{
    UART_DS_Start();
    //Clock_uart_Start();
    UART_DS_SCB_IRQ_StartEx(UART_HANDLER);
    Clock_uart_SetDividerValue(DIV_UART_9600);
    STATE_DS18=IDLE;
    ds18.ready=DS18_DATA_NOT_READY;
}

void reset_uart(void)
{
    UART_DS_Stop();
    UART_DS_SCB_IRQ_Stop();
    CyDelay(1);
    ini_ds18();
    CyDelay(1);
}

uint64 read_id(void)
{
    return ds18.id;
}

struct scratchpad *  get_ptr_scratchpad(void)
{
    return (struct scratchpad*)&scratchpad_ds18;
}

void get_scratchpad(void)
{
    if(STATE_DS18==IDLE)
    {
        STATE_DS18=RD_SCRATCHPAD;
        send_reset_ds18();
    }
}

void set_scratchpad(void)
{
    if(STATE_DS18==IDLE)
    {
        scratchpad_ds18.crc=0;
        uint8 *tmp_ptr=(uint8*)&scratchpad_ds18;
        for(uint8 i=0;i<sizeof(scratchpad_ds18)-1;i++)
            scratchpad_ds18.crc=calc_ds18_crc(tmp_ptr[i],scratchpad_ds18.crc);
        STATE_DS18=WR_SCRATCHPAD;
        send_reset_ds18();
    }
}

enum DS18_READY get_ready(void)
{
    enum DS18_READY tmp_ready;
    uint32 mask;
    mask=CyEnterCriticalSection();
    tmp_ready=ds18.ready;
    CyExitCriticalSection(mask);
    return tmp_ready;
}
int16 read_temperature(void)
{
    int16 temp;
    temp=((scratchpad_ds18.temp_msb)<<8)|scratchpad_ds18.temp_lsb;
    return temp;
}

void start_measure_ds18(void)
{
    if(STATE_DS18==IDLE)
    {
        STATE_DS18=START_CONVERT;
        send_reset_ds18();
    }
}

void send_reset_ds18(void)
{
    ds18.ready=DS18_DATA_NOT_READY;
    Clock_uart_SetDividerValue(DIV_UART_9600);
    UART_DS_SpiUartWriteTxData(0xF0);
    CMD_DS18=CMD_RESET;
}

void get_id_ds18(void)
{
    if(STATE_DS18==IDLE)
    {
        STATE_DS18=RD_ROM;
        send_reset_ds18();
    }
}

uint8 calc_ds18_crc(uint8 data, uint8 crc)
{
    uint8 crc8table[256]={0, 94, 188, 226, 97, 63, 221, 131, 194, 156, 126, 32, 163, 253, 31, 65,
                    	157, 195, 33, 127, 252, 162, 64, 30, 95, 1, 227, 189, 62, 96, 130, 220,
                    	35, 125, 159, 193, 66, 28, 254, 160, 225, 191, 93, 3, 128, 222, 60, 98,
                    	190, 224, 2, 92, 223, 129, 99, 61, 124, 34, 192, 158, 29, 67, 161, 255,
                    	70, 24, 250, 164, 39, 121, 155, 197, 132, 218, 56, 102, 229, 187, 89, 7,
                    	219, 133, 103, 57, 186, 228, 6, 88, 25, 71, 165, 251, 120, 38, 196, 154,
                    	101, 59, 217, 135, 4, 90, 184, 230, 167, 249, 27, 69, 198, 152, 122, 36,
                    	248, 166, 68, 26, 153, 199, 37, 123, 58, 100, 134, 216, 91, 5, 231, 185,
                    	140, 210, 48, 110, 237, 179, 81, 15, 78, 16, 242, 172, 47, 113, 147, 205,
                    	17, 79, 173, 243, 112, 46, 204, 146, 211, 141, 111, 49, 178, 236, 14, 80,
                    	175, 241, 19, 77, 206, 144, 114, 44, 109, 51, 209, 143, 12, 82, 176, 238,
                    	50, 108, 142, 208, 83, 13, 239, 177, 240, 174, 76, 18, 145, 207, 45, 115,
                    	202, 148, 118, 40, 171, 245, 23, 73, 8, 86, 180, 234, 105, 55, 213, 139,
                    	87, 9, 235, 181, 54, 104, 138, 212, 149, 203, 41, 119, 244, 170, 72, 22,
                    	233, 183, 85, 11, 136, 214, 52, 106, 43, 117, 151, 201, 74, 20, 246, 168,
                    	116, 42, 200, 150, 21, 75, 169, 247, 182, 232, 10, 84, 215, 137, 107, 53};
    
    return crc8table[data^crc];
}
/* [] END OF FILE */
