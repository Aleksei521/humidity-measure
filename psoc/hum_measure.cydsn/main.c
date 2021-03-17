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

#include "project.h"
#include <stdlib.h>
#include "ds18.h"

//Калибровочные данные датчика влажности HIH-4010-004
//Channel 40
//Serial Number 48094300016
//Wafer number 92511B19
//MRP number 78150431
//Calculated Values at 5V
//Vout 0%RH 0.851713 V
//Vout 75.3%RH 3.189375 V
//Accuracy @ 25 C 3.5%RH
//Zero offset 0.85713
//Slope 31.044653 mv/%RH
//Sensor RH: (Vout-0.852)/0.031
//Ratiometric response for 0 to 100%RH
//Vout=Vsupply(0.170 to 0.79)
#define AVDC 5160000ul//uV
#define RH_SHIFT 165061ul//Zero offset * 10^6 / Vsupplay
#define RH_SLOPE 6016ul//Slope * 10^6 / Vsupplay
#define T_SHIFT 1054600l//temperature shift * 10^6
#define T_SLOPE 2160l//temperature slope * 10^6
#define ADC_STEP (AVDC/4096ul)//uV
#define MIN_BAT 3000l
#define MAX_BAT 4200l
#define BAT_STEP ((MAX_BAT- MIN_BAT)/8l)
struct {
    uint16 ch0;
    uint16 ch1;
    uint32 rh;
    uint32 bat;
    uint8 eoc;
    int16 temperature;
} volatile adc_result={0,0,0,0,0,25};
#define BUTTON_PRESS_CNT 4
#define BUTTON_LONG_PRESS_CNT 138
#define BLINKING_CNT 14
#define TIMEOUT 164797
#define OFF_TIME 228
#define DS18_TIMEOUT 37
struct{
    uint8 button_press_cnt;
    uint8 ds18_timeout_cnt;
    uint32 off_timeout_cnt;
    uint8 off_time_cnt;
    
    uint32 button_state :1;
    uint32 button_state_storage :1;
    uint32 button_press_flag :1;
    uint32 button_press_out_flag;
    uint32 button_long_press_out_flag :1;
    uint32 off_time_flag :1;
    uint32 timeout_off_flag :1;
    uint32 ds18_timeout_flag :1;
} volatile timer={0,0,0,0,0,0,0,0,0,0,0,0};

void show_temperature(int16 temperature);
void clear_digit_position(uint8 position);
void display_error(uint8 position);
void display_r(uint8 position);
void clear_point(uint8 position);
void show_rh(uint32 rh);
void display_up_o(uint8 position);
void show_bat(int32 bat_step);
void enter_to_stop_mode(void);
uint32 convert_temperature(int16 temperature);

enum { MAIN, BATTERY_VOLTAGE, CONTRAST, OFF_TIMEOUT } DISPLAY_STATE=MAIN;
CY_ISR_PROTO(ADC_HANDLER);
CY_ISR_PROTO(TIMER_HANDLER);

CY_ISR(TIMER_HANDLER)
{
    //isr triggered every 21.845ms
    //debounce button 3*21.845ms=65.536ms
    //long press button 3sec/21.845ms=138 counts
    //time off 1min=60sec/21.845ms=2746 counts
    //time off 3min=180sec/21.845ms=8239 counts
    //time off 5min=300sec/21.845ms=13733 counts
    //time off 10min=600sec/21.845ms=27466 counts
    //time off 15min=900sec/21.845ms=41199 counts
    //time off 30min=1800sec/21.845ms=82398 counts
    //time off 60min=3600sec/21.845ms=164797 counts
    //time off 90min=5400sec/21.845=247139 counts
    uint32 intr_status;
    uint8 button;
    intr_status = TCPWM_1_GetInterruptSource();
        if(intr_status | TCPWM_1_INTR_MASK_TC)
        {
            if(timer.off_timeout_cnt<TIMEOUT && timer.timeout_off_flag==0)
                timer.off_timeout_cnt++;
            else
            {
                if(timer.off_timeout_cnt==TIMEOUT)
                {
                    timer.timeout_off_flag=1;
                    timer.off_timeout_cnt=0;
                }
            }
            if(timer.off_time_flag)
            {
                if(timer.off_time_cnt<OFF_TIME)
                    timer.off_time_cnt++;
                else
                    timer.off_time_flag=0;
            }
            if(timer.ds18_timeout_flag==1 && timer.ds18_timeout_cnt<DS18_TIMEOUT)
                timer.ds18_timeout_cnt++;
            else
            {
                timer.ds18_timeout_flag=0;
                timer.ds18_timeout_cnt=0;
            }
            
            button=button_Read();
            if(button == 0)
                timer.button_state=1;
            else
                timer.button_state=0;
            if(timer.button_state==timer.button_state_storage)
            {
                if(timer.button_press_cnt < BUTTON_PRESS_CNT)
                    timer.button_press_cnt++;
                else
                {
                    if(timer.button_state==1)
                    {
                        if(timer.button_press_cnt==BUTTON_PRESS_CNT)
                            timer.button_press_flag=1;
                        if(timer.button_press_cnt<BUTTON_LONG_PRESS_CNT)
                            timer.button_press_cnt++;
                        else
                        {
                            if(timer.button_press_cnt==BUTTON_LONG_PRESS_CNT)
                            {
                                timer.button_long_press_out_flag=1;
                                timer.button_press_flag=0;
                                timer.button_press_cnt++;
                            }
                        }
                    }
                    else
                    {
                        if(timer.button_press_cnt==BUTTON_PRESS_CNT)
                        {
                            timer.button_press_cnt++;
                            if(timer.button_press_flag==1)
                            {
                                timer.button_press_out_flag=1;
                                timer.button_press_flag=0;
                            }
                        }
                    }
                }
            }
            else
            {
                timer.button_state_storage=timer.button_state;
                timer.button_press_cnt=0;
            }
        }
    TCPWM_1_ClearInterrupt(intr_status);
}

CY_ISR(ADC_HANDLER)
{
uint32 intr_status;
intr_status = ADC_SAR_Seq_1_SAR_INTR_REG;
    adc_result.ch0 = ADC_SAR_Seq_1_GetResult16(0);
    adc_result.ch1 = ADC_SAR_Seq_1_GetResult16(1);
    adc_result.rh=((adc_result.ch0*1000000ul)/4096ul-RH_SHIFT)/RH_SLOPE;
    int32 rh=((int32)adc_result.rh*1000000l)/(T_SHIFT-T_SLOPE*adc_result.temperature);
    adc_result.rh=(uint32)rh;
    adc_result.bat=adc_result.ch1*ADC_STEP/1000l;//mV
    adc_result.eoc=1;
ADC_SAR_Seq_1_SAR_INTR_REG = intr_status;
}


int main(void)
{
    uint8 wakeup=0;
    uint32 contrast=40;
    int16 temperature=0;
    enum DS18_READY ds18_ready;
    
    if(CySysPmGetResetReason()==CY_PM_RESET_REASON_WAKEUP_STOP)
    {
        CySysPmUnfreezeIo();
        wakeup=1;
    }
    CyGlobalIntEnable; /* Enable global interrupts. */
    TCPWM_1_Start();
    isr_timer_StartEx(TIMER_HANDLER);
    LCD_Seg_1_Start();
    ADC_SAR_Seq_1_Start();
    ADC_SAR_Seq_1_IRQ_StartEx(ADC_HANDLER);
    //Clock_lcd_Start();
    ini_ds18();
    LCD_Seg_1_ClearDisplay();
    pwr_en_Write(1);
    adc_result.eoc=0;
    if(CySysWdtGetEnabledStatus(CY_SYS_WDT_COUNTER0_RESET)==0)
    {
        CySysWdtSetMode(CY_SYS_WDT_COUNTER0,CY_SYS_WDT_MODE_RESET);
        CySysWdtSetMatch(CY_SYS_WDT_COUNTER0,65535);
        CySysWdtEnable(CY_SYS_WDT_COUNTER0_MASK);
    }
    display_r(0);
    if(wakeup)
    {
        LCD_Seg_1_Write7SegDigit_0(0,1);
        LCD_Seg_1_WritePixel(LCD_Seg_1_H7SEG7_C,LCD_Seg_1_PIXEL_STATE_ON);
    }
    else
    {
        display_r(4);
        LCD_Seg_1_Write7SegDigit_0(0xE,3);
        LCD_Seg_1_Write7SegDigit_0(5,2);
        LCD_Seg_1_Write7SegDigit_0(0xE,1);
        LCD_Seg_1_WritePixel(LCD_Seg_1_H7SEG7_D,LCD_Seg_1_PIXEL_STATE_ON);
        LCD_Seg_1_WritePixel(LCD_Seg_1_H7SEG7_F,LCD_Seg_1_PIXEL_STATE_ON);
    }
    start_measure_ds18();
    CySysWdtResetCounters(CY_SYS_WDT_COUNTER0_RESET);
    CyDelay(1000);
    CySysWdtResetCounters(CY_SYS_WDT_COUNTER0_RESET);
    LCD_Seg_1_ClearDisplay();
    for(;;)
    {
        CySysWdtResetCounters(CY_SYS_WDT_COUNTER0_RESET);
        if(timer.button_long_press_out_flag || timer.timeout_off_flag==1)
        {
            timer.button_long_press_out_flag=0;
            enter_to_stop_mode();
        }
        timer.ds18_timeout_flag=1;
        timer.ds18_timeout_cnt=0;
        do{
            ds18_ready=get_ready();
        } while(ds18_ready==DS18_DATA_NOT_READY && timer.ds18_timeout_flag==1);
        if(ds18_ready==DS18_DATA_READY)
        {
            get_scratchpad();
            timer.ds18_timeout_flag=1;
            timer.ds18_timeout_cnt=0;
            do{
                ds18_ready=get_ready();
            } while(ds18_ready==DS18_DATA_NOT_READY && timer.ds18_timeout_flag==1);
            if(ds18_ready==DS18_DATA_READY)
            {
                temperature=read_temperature();
                show_temperature(temperature);
            }
            else
            {
                display_error(7);
                reset_uart();
            }
        }
        else
        {
            display_error(7);
            reset_uart();
        }
        ADC_SAR_Seq_1_StartConvert();
        while(!adc_result.eoc);
        adc_result.eoc=0;
        show_rh(adc_result.rh);
        show_bat(((int32)(adc_result.bat)-MIN_BAT)/BAT_STEP);
        start_measure_ds18();
        adc_result.temperature=convert_temperature(temperature)/10000;
        if(adc_result.temperature<0 && contrast!=50)
        {
            contrast=50;
            LCD_Seg_1_SetContrast(50);
        }
        else
        {
            if(adc_result.temperature>=0 && adc_result.temperature<=25 && contrast!=40)
            {
                contrast=40;
                LCD_Seg_1_SetContrast(40);
            }
            else
            {
                if(adc_result.temperature>25 && contrast!=30)
                {
                    contrast=30;
                    LCD_Seg_1_SetContrast(30);
                }
            }
        }
                
    }
}

void enter_to_stop_mode(void)
{
    CySysPmSetWakeupPolarity(CY_PM_STOP_WAKEUP_ACTIVE_LOW);
    LCD_Seg_1_ClearDisplay();
    LCD_Seg_1_Write7SegDigit_0(0,2);
    LCD_Seg_1_Write7SegDigit_0(0xF,1);
    LCD_Seg_1_Write7SegDigit_0(0xF,0);
    if(timer.timeout_off_flag)
        timer.timeout_off_flag=0;
    else
        CyDelay(50);
    timer.button_press_out_flag=0;
    timer.off_time_cnt=0;
    timer.off_time_flag=1;
    CySysWdtDisable(CY_SYS_WDT_COUNTER0_MASK);
    while(timer.off_time_flag==1 && timer.button_press_out_flag==0);
    if(timer.button_press_out_flag==0)
    {
        isr_timer_Stop();
        LCD_Seg_1_Stop();
        TCPWM_1_Stop();
        ADC_SAR_Seq_1_Stop();
        pwr_en_Write(0);
        CySysPmStop();
    }
    else
    {
        timer.button_press_out_flag=0;
        timer.timeout_off_flag=0;
        timer.button_long_press_out_flag=0;
        LCD_Seg_1_ClearDisplay();
        CySysWdtEnable(CY_SYS_WDT_COUNTER0_MASK);
    }
}

void clear_digit_position(uint8 position)
{
    const uint32 dig_pos[8][7]={{LCD_Seg_1_H7SEG7_A,LCD_Seg_1_H7SEG7_B,LCD_Seg_1_H7SEG7_C,LCD_Seg_1_H7SEG7_D,LCD_Seg_1_H7SEG7_E,LCD_Seg_1_H7SEG7_F,LCD_Seg_1_H7SEG7_G},\
                                {LCD_Seg_1_H7SEG6_A,LCD_Seg_1_H7SEG6_B,LCD_Seg_1_H7SEG6_C,LCD_Seg_1_H7SEG6_D,LCD_Seg_1_H7SEG6_E,LCD_Seg_1_H7SEG6_F,LCD_Seg_1_H7SEG6_G},\
                                {LCD_Seg_1_H7SEG5_A,LCD_Seg_1_H7SEG5_B,LCD_Seg_1_H7SEG5_C,LCD_Seg_1_H7SEG5_D,LCD_Seg_1_H7SEG5_E,LCD_Seg_1_H7SEG5_F,LCD_Seg_1_H7SEG5_G},\
                                {LCD_Seg_1_H7SEG4_A,LCD_Seg_1_H7SEG4_B,LCD_Seg_1_H7SEG4_C,LCD_Seg_1_H7SEG4_D,LCD_Seg_1_H7SEG4_E,LCD_Seg_1_H7SEG4_F,LCD_Seg_1_H7SEG4_G},\
                                {LCD_Seg_1_H7SEG3_A,LCD_Seg_1_H7SEG3_B,LCD_Seg_1_H7SEG3_C,LCD_Seg_1_H7SEG3_D,LCD_Seg_1_H7SEG3_E,LCD_Seg_1_H7SEG3_F,LCD_Seg_1_H7SEG3_G},
                                {LCD_Seg_1_H7SEG2_A,LCD_Seg_1_H7SEG2_B,LCD_Seg_1_H7SEG2_C,LCD_Seg_1_H7SEG2_D,LCD_Seg_1_H7SEG2_E,LCD_Seg_1_H7SEG2_F,LCD_Seg_1_H7SEG2_G},\
                                {LCD_Seg_1_H7SEG1_A,LCD_Seg_1_H7SEG1_B,LCD_Seg_1_H7SEG1_C,LCD_Seg_1_H7SEG1_D,LCD_Seg_1_H7SEG1_E,LCD_Seg_1_H7SEG1_F,LCD_Seg_1_H7SEG1_G},\
                                {LCD_Seg_1_H7SEG0_A,LCD_Seg_1_H7SEG0_B,LCD_Seg_1_H7SEG0_C,LCD_Seg_1_H7SEG0_D,LCD_Seg_1_H7SEG0_E,LCD_Seg_1_H7SEG0_F,LCD_Seg_1_H7SEG0_G}};
    for(int i=0;i<7;i++)
        LCD_Seg_1_WritePixel(dig_pos[position][i],LCD_Seg_1_PIXEL_STATE_OFF);
}

void display_error(uint8 position)
{
    const uint32 seg_or[8][2]={{LCD_Seg_1_H7SEG7_C,LCD_Seg_1_H7SEG7_D},\
                            {LCD_Seg_1_H7SEG6_C,LCD_Seg_1_H7SEG6_D},\
                            {LCD_Seg_1_H7SEG5_C,LCD_Seg_1_H7SEG5_D},\
                            {LCD_Seg_1_H7SEG4_C,LCD_Seg_1_H7SEG4_D},\
                            {LCD_Seg_1_H7SEG3_C,LCD_Seg_1_H7SEG3_D},\
                            {LCD_Seg_1_H7SEG2_C,LCD_Seg_1_H7SEG2_D},\
                            {LCD_Seg_1_H7SEG1_C,LCD_Seg_1_H7SEG1_D},\
                            {LCD_Seg_1_H7SEG0_C,LCD_Seg_1_H7SEG0_D}};
    for(int i=0;i<5;i++)
    {
        clear_digit_position(position-i);
        clear_point(position-i);
    }
    LCD_Seg_1_Write7SegDigit_0(0xE, position);
    display_r(position-1); 
    display_r(position-2); 
    display_r(position-3); 
    LCD_Seg_1_WritePixel(seg_or[position-3][0],LCD_Seg_1_PIXEL_STATE_ON);
    LCD_Seg_1_WritePixel(seg_or[position-3][1],LCD_Seg_1_PIXEL_STATE_ON);
    display_r(position-4); 
}

void clear_point(uint8 position)
{
    const uint32 point_seg[8]={LCD_Seg_1_HBAR0,LCD_Seg_1_HBAR1,LCD_Seg_1_HBAR2,LCD_Seg_1_HBAR3,LCD_Seg_1_HBAR4,LCD_Seg_1_HBAR5,LCD_Seg_1_HBAR6,LCD_Seg_1_HBAR7};
    LCD_Seg_1_WritePixel(point_seg[position],LCD_Seg_1_PIXEL_STATE_OFF);
}

void display_up_o(uint8 position)
{
    const uint32 seg_r[8][4]={{LCD_Seg_1_H7SEG7_A,LCD_Seg_1_H7SEG7_B,LCD_Seg_1_H7SEG7_F,LCD_Seg_1_H7SEG7_G},\
                            {LCD_Seg_1_H7SEG6_A,LCD_Seg_1_H7SEG6_B,LCD_Seg_1_H7SEG6_F,LCD_Seg_1_H7SEG6_G},\
                            {LCD_Seg_1_H7SEG5_A,LCD_Seg_1_H7SEG5_B,LCD_Seg_1_H7SEG5_F,LCD_Seg_1_H7SEG5_G},\
                            {LCD_Seg_1_H7SEG4_A,LCD_Seg_1_H7SEG4_B,LCD_Seg_1_H7SEG4_F,LCD_Seg_1_H7SEG4_G},\
                            {LCD_Seg_1_H7SEG3_A,LCD_Seg_1_H7SEG3_B,LCD_Seg_1_H7SEG3_F,LCD_Seg_1_H7SEG3_G},\
                            {LCD_Seg_1_H7SEG2_A,LCD_Seg_1_H7SEG2_B,LCD_Seg_1_H7SEG2_F,LCD_Seg_1_H7SEG2_G},\
                            {LCD_Seg_1_H7SEG1_A,LCD_Seg_1_H7SEG1_B,LCD_Seg_1_H7SEG1_F,LCD_Seg_1_H7SEG1_G},\
                            {LCD_Seg_1_H7SEG0_A,LCD_Seg_1_H7SEG0_B,LCD_Seg_1_H7SEG0_F,LCD_Seg_1_H7SEG0_G}};
    for(int i=0;i<4;i++)
        LCD_Seg_1_WritePixel(seg_r[position][i],LCD_Seg_1_PIXEL_STATE_ON);
}

void display_r(uint8 position)
{
    const uint32 seg_r[8][2]={{LCD_Seg_1_H7SEG7_E,LCD_Seg_1_H7SEG7_G},{LCD_Seg_1_H7SEG6_E,LCD_Seg_1_H7SEG6_G},{LCD_Seg_1_H7SEG5_E,LCD_Seg_1_H7SEG5_G},\
                            {LCD_Seg_1_H7SEG4_E,LCD_Seg_1_H7SEG4_G},{LCD_Seg_1_H7SEG3_E,LCD_Seg_1_H7SEG3_G},{LCD_Seg_1_H7SEG2_E,LCD_Seg_1_H7SEG2_G},\
                            {LCD_Seg_1_H7SEG1_E,LCD_Seg_1_H7SEG1_G},{LCD_Seg_1_H7SEG0_E,LCD_Seg_1_H7SEG0_G}};
    LCD_Seg_1_WritePixel(seg_r[position][0],LCD_Seg_1_PIXEL_STATE_ON);
    LCD_Seg_1_WritePixel(seg_r[position][1],LCD_Seg_1_PIXEL_STATE_ON);
}

void show_temperature(int16 temperature)
{
    int32 tmp=0;
    int32 temp_int=convert_temperature(temperature);
    if((temp_int%1000)>=500)
        tmp=1;
    temp_int =temp_int/1000+tmp;
    clear_digit_position(7);
    clear_digit_position(6);
    clear_digit_position(3);
    LCD_Seg_1_Write7SegDigit_0(temp_int%10,4);
    temp_int /=10;
    LCD_Seg_1_Write7SegDigit_0(temp_int%10,5);
    temp_int /=10;
    if(temp_int!=0)
    {
        LCD_Seg_1_Write7SegDigit_0(temp_int%10,6);
        temp_int /=10;
        if(temp_int!=0)
            LCD_Seg_1_Write7SegDigit_0(temp_int%10,7);
        else
        {
        if(temperature<0)
            LCD_Seg_1_WritePixel(LCD_Seg_1_H7SEG0_G,LCD_Seg_1_PIXEL_STATE_ON);
        }
        
    }
    else
    {
        if(temperature<0)
            LCD_Seg_1_WritePixel(LCD_Seg_1_H7SEG1_G,LCD_Seg_1_PIXEL_STATE_ON);
    }
        
    LCD_Seg_1_WritePixel(LCD_Seg_1_HBAR5,LCD_Seg_1_PIXEL_STATE_ON);
    display_up_o(3);
}

uint32 convert_temperature(int16 temperature)
{
    return labs((int32)temperature)*(int32)625;
}

void show_bat(int32 bat_mV)
{
    LCD_Seg_1_WriteBargraph_2(0,0);//clear
    if(bat_mV>0)
    {
        LCD_Seg_1_WriteBargraph_2(8-bat_mV,-1);
    }
}

void show_rh(uint32 rh)
{
    for(int i=0;i<3;i++)
        clear_digit_position(i);
    LCD_Seg_1_Write7SegDigit_0(rh%10,0);
    rh/=10;
    if(rh)
    {
        LCD_Seg_1_Write7SegDigit_0(rh%10,1);
        rh/=10;
        if(rh)
            LCD_Seg_1_Write7SegDigit_0(rh%10,1);
    }
}

/* [] END OF FILE */
