/*******************************************************************************
* File Name: `$INSTANCE_NAME`.c
* Version `$CY_MAJOR_VERSION`.`$CY_MINOR_VERSION`
*
* Description:
*  This file provides the source code to the API for the Segment LCD component.
*
* Note:
*  None
*
********************************************************************************
* Copyright 2013-2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
*******************************************************************************/

#include "`$INSTANCE_NAME`.h"

static void `$INSTANCE_NAME`_WriteFrameBuffer(reg32 regAddr[], uint32 bitNumber, uint32 state);
static uint32 `$INSTANCE_NAME`_ReadFrameBuffer(const reg32 regAddr[], uint32 bitNumber);

/* This section contains look-up tables for different kinds of displays. */
#ifdef `$INSTANCE_NAME`_7SEG
    static const uint8 `$INSTANCE_NAME`_7SegDigits[] = {
      /*  '0'    '1'    '2'    '3'    '4'    '5'    '6'    '7' */
        0x3fu, 0x06u, 0x5bu, 0x4fu, 0x66u, 0x6du, 0x7du, 0x07u,
      /*  '8'    '9'    'A'    'B'    'C'    'D'    'E'    'F'   null */
        0x7fu, 0x6fu, 0x77u, 0x7cu, 0x39u, 0x5eu, 0x79u, 0x71u, 0x00u};
#endif /* `$INSTANCE_NAME`_7SEG */

#ifdef `$INSTANCE_NAME`_14SEG
    static const uint16 `$INSTANCE_NAME`_14SegChars[] = {
    /*------------------------------------------------------------*/
    /*                           Blank                            */
    /*------------------------------------------------------------*/
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    /*------------------------------------------------------------*/
    /*         !       "       #       $       %       &       '  */
    /*------------------------------------------------------------*/
    0x0000u,0x0006u,0x0120u,0x3fffu,0x156du,0x2ee4u,0x2a8du,0x0200u,
    /*------------------------------------------------------------*/
    /* (       )       *       +       ,       -       .       /  */
    /*------------------------------------------------------------*/
    0x0a00u,0x2080u,0x3fc0u,0x1540u,0x2000u,0x0440u,0x1058u,0x2200u,
    /*------------------------------------------------------------*/
    /* 0       1       2       3       4       5       6       7  */
    /*------------------------------------------------------------*/
    0x223fu,0x0206u,0x045bu,0x040fu,0x0466u,0x0869u,0x047du,0x1201u,
    /*------------------------------------------------------------*/
    /* 8       9       :       ;       <       =       >       ?  */
    /*------------------------------------------------------------*/
    0x047fu,0x046fu,0x1100u,0x2100u,0x0a00u,0x0448u,0x2080u,0x1423u,
    /*------------------------------------------------------------*/
    /* @       A       B       C       D       E       F       G  */
    /*------------------------------------------------------------*/
    0x053bu,0x0477u,0x150fu,0x0039u,0x110Fu,0x0079u,0x0071u,0x043du,
    /*------------------------------------------------------------*/
    /* H       I       J       K       L       M       N       O  */
    /*------------------------------------------------------------*/
    0x0476u,0x1100u,0x001eu,0x0a70u,0x0038u,0x02b6u,0x08b6u,0x003fu,
    /*------------------------------------------------------------*/
    /* P       Q       R       S       T       U       V       W  */
    /*------------------------------------------------------------*/
    0x0473u,0x083fu,0x0C73u,0x046du,0x1101u,0x003eu,0x2230u,0x2836u,
    /*------------------------------------------------------------*/
    /* X       Y       Z       [       \       ]       ^       _  */
    /*------------------------------------------------------------*/
    0x2a80u,0x1462u,0x2209u,0x0039u,0x0880u,0x000fu,0x0003u,0x0008u,
    /*------------------------------------------------------------*/
    /* @       a       b       c       d       e       f       g  */
    /*------------------------------------------------------------*/
    0x053bu,0x0477u,0x150fu,0x0039u,0x110Fu,0x0079u,0x0071u,0x043du,
    /*------------------------------------------------------------*/
    /* h       i       j       k       l       m       n       o  */
    /*------------------------------------------------------------*/
    0x0476u,0x1100u,0x001eu,0x0a70u,0x0038u,0x02b6u,0x08b6u,0x003fu,
    /*------------------------------------------------------------*/
    /* p       q       r       s       t       u       v       w  */
    /*------------------------------------------------------------*/
    0x0473u,0x083fu,0x0C73u,0x046du,0x1101u,0x003eu,0x2230u,0x2836u,
    /*------------------------------------------------------------*/
    /* x       y       z       {       |       }        ~      O  */
    /*------------------------------------------------------------*/
    0x2a80u,0x1280u,0x2209u,0x0e00u,0x1100u,0x20C0u,0x0452u,0x003fu};
#endif /* `$INSTANCE_NAME`_14SEG */

#ifdef `$INSTANCE_NAME`_16SEG
    static const uint16 `$INSTANCE_NAME`_16SegChars[] = {
    /*------------------------------------------------------------*/
    /*                           Blank                            */
    /*------------------------------------------------------------*/
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,0x0000u,
    /*------------------------------------------------------------*/
    /*         !       "       #       $       %       &       '  */
    /*------------------------------------------------------------*/
    0x0000u,0x000cu,0x0480u,0xffffu,0x55bbu,0xdd99u,0xaa3bu,0x0800u,
    /*------------------------------------------------------------*/
    /* (       )       *       +       ,       -       .       /  */
    /*------------------------------------------------------------*/
    0x2800u,0x8200u,0xff00u,0x5500u,0x8000u,0x1100u,0x4160u,0x8800u,
    /*------------------------------------------------------------*/
    /* 0       1       2       3       4       5       6       7  */
    /*------------------------------------------------------------*/
    0x88ffu,0x000cu,0x1177u,0x103fu,0x118cu,0x21b3u,0x11fbu,0x4803u,
    /*------------------------------------------------------------*/
    /* 8       9       :       ;       <       =       >       ?  */
    /*------------------------------------------------------------*/
    0x11ffu,0x11bfu,0x4400u,0x8400u,0x2800u,0x1130u,0x8200u,0x5087u,
    /*------------------------------------------------------------*/
    /* @       A       B       C       D       E       F       G  */
    /*------------------------------------------------------------*/
    0x14f7u,0x11cfu,0x543fu,0x00f3u,0x443fu,0x01f3u,0x01c3u,0x10fbu,
    /*------------------------------------------------------------*/
    /* H       I       J       K       L       M       N       O  */
    /*------------------------------------------------------------*/
    0x11ccu,0x4400u,0x007eu,0x29c0u,0x00f0u,0x0accu,0x22ccu,0x00ffu,
    /*------------------------------------------------------------*/
    /* P       Q       R       S       T       U       V       W  */
    /*------------------------------------------------------------*/
    0x11c7u,0x20ffu,0x31c7u,0x11bbu,0x4403u,0x00fcu,0x88c0u,0xa0ccu,
    /*------------------------------------------------------------*/
    /* X       Y       Z       [       \       ]       ^       _  */
    /*------------------------------------------------------------*/
    0xaa00u,0x5184u,0x8833u,0x4412u,0x2200u,0x4421u,0x0006u,0x0030u,
    /*------------------------------------------------------------*/
    /* @       a       b       c       d       e       f       g  */
    /*------------------------------------------------------------*/
    0x14f7u,0x11cfu,0x543fu,0x00f3u,0x443fu,0x01f3u,0x01c3u,0x10fbu,
    /*------------------------------------------------------------*/
    /* h       i       j       k       l       m       n       o  */
    /*------------------------------------------------------------*/
    0x11ccu,0x4400u,0x007eu,0x29c0u,0x00f0u,0x0accu,0x22ccu,0x00ffu,
    /*------------------------------------------------------------*/
    /* p       q       r       s       t       u       v       w  */
    /*------------------------------------------------------------*/
    0x11c7u,0x20ffu,0x31c7u,0x11bbu,0x4403u,0x00fcu,0x88c0u,0xa0ccu,
    /*------------------------------------------------------------*/
    /* x       y       z       {       |       }        ~         */
    /*------------------------------------------------------------*/
    0xaa00u,0x4A00u,0x8833u,0x3800u,0x4400u,0x8300u,0x1144u,0x0000u};
#endif /* `$INSTANCE_NAME`_16SEG */

#ifdef `$INSTANCE_NAME`_DOT_MATRIX
    `$charDotMatrix`
#endif /* `$INSTANCE_NAME`_DOT_MATRIX */

/* Start of customizer generated code */
`$ContrastDefines`

uint32 `$INSTANCE_NAME`_initVar = 0u;
static uint32 `$INSTANCE_NAME`_modeState = `$INSTANCE_NAME`_LCD_MODE;
static uint32 `$INSTANCE_NAME`_invertState = `$INSTANCE_NAME`_STATE_NORMAL;
static uint32 `$INSTANCE_NAME`_contrastState = `$INSTANCE_NAME`_CONTRAST;
`$writerCVariables`


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_WriteFrameBuffer
********************************************************************************
*
* Summary:
*  Sets or clears a particular bit in the frame buffer.
*
* Parameters:
*  reg32 regAddress[]: Frame buffer register address.
*  bitNumber : The predefined packed number that points to the bit's location
*              in the frame buffer.
*  state : Specifies bit state.
*
* Return:
*  None.
*
*******************************************************************************/
static void `$INSTANCE_NAME`_WriteFrameBuffer(reg32 regAddr[], uint32 bitNumber, uint32 state)
{
    uint32 row;
    uint32 port;
    uint32 pin;
    uint32 mask;

    /* Extract the bit information to locate desired bit in the frame buffer */
    row  = `$INSTANCE_NAME`_EXTRACT_ROW(bitNumber) % `$INSTANCE_NAME`_MAX_BUFF_ROWS;
    port = `$INSTANCE_NAME`_EXTRACT_PORT(bitNumber) + ((`$INSTANCE_NAME`_EXTRACT_ROW(bitNumber) /
           `$INSTANCE_NAME`_MAX_BUFF_ROWS) * `$INSTANCE_NAME`_DATA_REGS_OFFSET);
    pin  = `$INSTANCE_NAME`_EXTRACT_PIN(bitNumber) * `$INSTANCE_NAME`_MAX_BUFF_ROWS;

    /* Write new bit value to the frame buffer. */
    mask = (uint32)(~((uint32)(`$INSTANCE_NAME`_PIXEL_STATE_ON << (pin + row))));

    regAddr[port] = (regAddr[port] & mask) | (uint32)((state & `$INSTANCE_NAME`_PIXEL_STATE_ON) << (pin + row));
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_ReadFrameBuffer
********************************************************************************
*
* Summary:
*  Gets state of a particular bit in the frame buffer.
*
* Parameters:
*  reg32 regAddress[]: Frame buffer register address.
*  bitNumber : The predefined packed number that points to the bit's location
*              in the frame buffer.
*
* Return:
*  Returns the current status of the specified bit in the frame buffer.
*
*******************************************************************************/
static uint32 `$INSTANCE_NAME`_ReadFrameBuffer(const reg32 regAddr[], uint32 bitNumber)
{
    uint32 row;
    uint32 port;
    uint32 pin;
    uint32 pixelState;

    /* Extract the bit information to locate desired bit in the frame buffer */
    row  = `$INSTANCE_NAME`_EXTRACT_ROW(bitNumber) % `$INSTANCE_NAME`_MAX_BUFF_ROWS;
    port = `$INSTANCE_NAME`_EXTRACT_PORT(bitNumber) + ((`$INSTANCE_NAME`_EXTRACT_ROW(bitNumber) /
           `$INSTANCE_NAME`_MAX_BUFF_ROWS) * `$INSTANCE_NAME`_DATA_REGS_OFFSET);
    pin  = `$INSTANCE_NAME`_EXTRACT_PIN(bitNumber) * `$INSTANCE_NAME`_MAX_BUFF_ROWS;

    pixelState = (uint32)(regAddr[port] >> (pin + row)) & `$INSTANCE_NAME`_PIXEL_STATE_ON;

    return(pixelState);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_Init
********************************************************************************
*
* Summary:
*  Initialize/Restore default Segment LCD configuration.
*
* Parameters:
*  None.
*
* Return:
*  None.
*
* Side Effects:
*  Block will be stopped to change settings.
*
*******************************************************************************/
void `$INSTANCE_NAME`_Init(void)
{
    `$INSTANCE_NAME`_Stop();

    /* Set sub-frame and dead time dividers */
    `$INSTANCE_NAME`_DIVIDER_REG = `$INSTANCE_NAME`_INIT_DIVIDERS;

    /* Set Configuration Register */
    `$INSTANCE_NAME`_CONTROL_REG = `$INSTANCE_NAME`_INIT_CONTROL;

    /* Need to clear display to start normal work.
    *  `$INSTANCE_NAME`_ClearDisplay() function initializes Commons in the frame buffer as well.
    */
    `$INSTANCE_NAME`_ClearDisplay();
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_Enable
********************************************************************************
*
* Summary:
*  Enables the Segment LCD.
*
* Parameters:
*  None.
*
* Return:
*  None.
*
* Global variables:
*  `$INSTANCE_NAME`_modeState - holds the current LCD speed mode (LS/HS).
*
*******************************************************************************/
void `$INSTANCE_NAME`_Enable(void)
{
    if (`$INSTANCE_NAME`_SPEED_LS == `$INSTANCE_NAME`_modeState)
    {
        `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_ENABLE_MASK)))
                                       | `$INSTANCE_NAME`_LS_EN;
    }
    else /* (`$INSTANCE_NAME`_SPEED_HS == `$INSTANCE_NAME`_modeState) */
    {
        `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_ENABLE_MASK)))
                                       | `$INSTANCE_NAME`_HS_EN;
    }
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_Start
********************************************************************************
*
* Summary:
*  Initialize the Segment LCD with default customizer values and enables the
*  Segment LCD.
*
* Parameters:
*  None.
*
* Return:
*  None.
*
* Global variables:
*  `$INSTANCE_NAME`_initVar - is used to indicate initial configuration of
*  this component. The variable is initialized to zero and set to 1 the
*  first time `$INSTANCE_NAME`_Start() is called. This allows for component
*  initialization without re-initialization in all subsequent calls to the
*  `$INSTANCE_NAME`_Start() routine.
*
*******************************************************************************/
void `$INSTANCE_NAME`_Start(void)
{
    /* If not Initialized then initialize all required hardware and software */
    if (`$INSTANCE_NAME`_initVar == 0u)
    {
        `$INSTANCE_NAME`_initVar = 1u;
        `$INSTANCE_NAME`_Init();
    }

    `$INSTANCE_NAME`_Enable();
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_Stop
********************************************************************************
*
* Summary:
*  Disables the Segment LCD.
*
* Parameters:
*  None.
*
* Return:
*  None.
*
*******************************************************************************/
void `$INSTANCE_NAME`_Stop(void)
{
    `$INSTANCE_NAME`_CONTROL_REG &= (uint32)(~`$INSTANCE_NAME`_ENABLE_MASK);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_SetSpeedMode
********************************************************************************
*
* Summary:
*  Sets HS or LS LCD speed mode.
*
* Parameters:
*  mode : Sets the mode of the LCD clock speed operation:
*    Define                         Description
*     `$INSTANCE_NAME`_SPEED_LS      Low Speed mode.
*     `$INSTANCE_NAME`_SPEED_HS      High Speed mode.
*
* Return:
*  None.
*
* Global variables:
*  `$INSTANCE_NAME`_modeState - holds the current LCD speed mode (LS/HS).
*  `$INSTANCE_NAME`_contrastState - holds the current LCD contrast.
*
*******************************************************************************/
void `$INSTANCE_NAME`_SetSpeedMode(uint32 mode)
{
    uint32 contrast = (`$INSTANCE_NAME`_contrastState / 10u) - 1u;

    if (`$INSTANCE_NAME`_modeState != mode)
    {
        `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_MODE)))
                                       | (((uint32)(mode  << `$INSTANCE_NAME`_MODE_SHIFT)) & `$INSTANCE_NAME`_MODE);

        if (`$INSTANCE_NAME`_SPEED_LS == mode)
        {
            /* Set sub-frame and dead time dividers */
            `$INSTANCE_NAME`_DIVIDER_REG = (`$INSTANCE_NAME`_DIVIDER_REG & ((uint32)(~`$INSTANCE_NAME`_DIVIDER_MASK)))
                            | `$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_SUBFR_DIVS][contrast]
                            | ((uint32)((uint32)`$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_DEAD_DIVS][contrast] <<
                                                `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT));

            `$INSTANCE_NAME`_modeState = `$INSTANCE_NAME`_SPEED_LS;
        }
        else /* (`$INSTANCE_NAME`_SPEED_HS == mode) */
        {
            /* Set sub-frame and dead time dividers */
            `$INSTANCE_NAME`_DIVIDER_REG = (`$INSTANCE_NAME`_DIVIDER_REG & ((uint32)(~`$INSTANCE_NAME`_DIVIDER_MASK)))
                            | `$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_SUBFR_DIVS][contrast]
                            | ((uint32)((uint32)`$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_DEAD_DIVS][contrast] <<
                                                `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT));

            `$INSTANCE_NAME`_modeState = `$INSTANCE_NAME`_SPEED_HS;
        }
    }
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_SetOperationMode
********************************************************************************
*
* Summary:
*  Sets PWM or Digital Correlation LCD operation mode.
*
* Parameters:
*  mode : Sets the mode of the LCD operation:
*    Define                         Description
*     `$INSTANCE_NAME`_MODE_PWM      PWM operation mode.
*     `$INSTANCE_NAME`_MODE_DIG_COR  Digital Correlation mode.
*
* Return:
*  None.
*
*******************************************************************************/
void `$INSTANCE_NAME`_SetOperationMode(uint32 mode)
{
    `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_OP_MODE)))
                                   | (((uint32)(mode << `$INSTANCE_NAME`_OP_MODE_SHIFT)) & `$INSTANCE_NAME`_OP_MODE);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_SetBiasType
********************************************************************************
*
* Summary:
*  Sets bias type for PWM operation mode.
*
* Parameters:
*  bias : Sets the bias type for PWM operation mode:
*    Define                         Description
*     `$INSTANCE_NAME`_BIAS_1_2      1/2 Bias.
*     `$INSTANCE_NAME`_BIAS_1_3      1/3 Bias.
*     `$INSTANCE_NAME`_BIAS_1_4      1/4 Bias. Not supported in Low Speed mode.
*     `$INSTANCE_NAME`_BIAS_1_5      1/5 Bias. Not supported in Low Speed mode.
*
* Return:
*  None.
*
*******************************************************************************/
void `$INSTANCE_NAME`_SetBiasType(uint32 bias)
{
    `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_BIAS_MASK)))
                                 | (((uint32)(bias << `$INSTANCE_NAME`_BIAS_MASK_SHIFT)) & `$INSTANCE_NAME`_BIAS_MASK);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_SetWaveformType
********************************************************************************
*
* Summary:
*  Sets the LCD driving Waveform Type.
*
* Parameters:
*  type : Sets the Waveform Type:
*    Define                         Description
*     `$INSTANCE_NAME`_TYPE_A        Waveform type A.
*     `$INSTANCE_NAME`_TYPE_B        Waveform type B.
*
* Return:
*  None.
*
*******************************************************************************/
void `$INSTANCE_NAME`_SetWaveformType(uint32 type)
{
    `$INSTANCE_NAME`_CONTROL_REG = (`$INSTANCE_NAME`_CONTROL_REG & ((uint32)(~`$INSTANCE_NAME`_TYPE)))
                                   | (((uint32)(type << `$INSTANCE_NAME`_TYPE_SHIFT)) & `$INSTANCE_NAME`_TYPE);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_SetContrast
********************************************************************************
*
* Summary:
*  Sets the contrast control using "Dead Period" digital modulation.
*
* Parameters:
*  contrast : Sets the contrast for the LCD glass in percentage. Valid range
*             from 10% to 100% in 10% increments. Valid range can be restricted
*             because of dividers size (for LS mode 8 bit and for HS mode 16
*             bit). For greater frequencies, certain ratios between contrast and
*             frame rate can not be achieved.
*
* Return:
*  Pass or fail based on a validity check of the contrast value.
*    Define                     Description
*     CYRET_SUCCESS              Function completed successfully.
*     CYRET_BAD_PARAM            Evaluation of contrast parameter is failed.
*
* Global variables:
*  `$INSTANCE_NAME`_modeState - holds the current LCD speed mode (LS/HS).
*  `$INSTANCE_NAME`_contrastState - holds the current LCD contrast.
*
*******************************************************************************/
uint32 `$INSTANCE_NAME`_SetContrast(uint32 contrast)
{
    uint32 status = CYRET_BAD_PARAM;
    uint32 contrastIndex;

    if ((`$INSTANCE_NAME`_MIN_CONTRAST <= contrast) && (contrast <= `$INSTANCE_NAME`_MAX_CONTRAST))
    {
        contrastIndex = (contrast / 10u) - 1u;
        if (`$INSTANCE_NAME`_SPEED_LS == `$INSTANCE_NAME`_modeState)
        {
            if (`$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex] !=
                `$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex + 1u])
            {
                /* Set sub-frame and dead time dividers */
                `$INSTANCE_NAME`_DIVIDER_REG = (`$INSTANCE_NAME`_DIVIDER_REG
                        & ((uint32)(~`$INSTANCE_NAME`_DIVIDER_MASK)))
                        | `$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_SUBFR_DIVS][contrastIndex]
                        | ((uint32)((uint32)`$INSTANCE_NAME`_dividersLS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex] <<
                                            `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT));
                status = CYRET_SUCCESS;
            }
        }
        else /* (`$INSTANCE_NAME`_SPEED_HS == `$INSTANCE_NAME`_modeState) */
        {
            if (`$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex] !=
                `$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex + 1u])
            {
                /* Set sub-frame and dead time dividers */
                `$INSTANCE_NAME`_DIVIDER_REG = (`$INSTANCE_NAME`_DIVIDER_REG
                        & ((uint32)(~`$INSTANCE_NAME`_DIVIDER_MASK)))
                        | `$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_SUBFR_DIVS][contrastIndex]
                        | ((uint32)((uint32)`$INSTANCE_NAME`_dividersHS[`$INSTANCE_NAME`_DEAD_DIVS][contrastIndex] <<
                                            `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT));
                status = CYRET_SUCCESS;
            }
        }

        `$INSTANCE_NAME`_contrastState = contrast; /* Saves new contrast value */
    }

    return(status);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_WriteInvertState
********************************************************************************
*
* Summary:
*  Inverts the display based on an input parameter.
*
* Parameters:
*  invertState : Sets the invert state of the display:
*    Define                            Description
*     `$INSTANCE_NAME`_STATE_NORMAL     Normal non inverted display.
*     `$INSTANCE_NAME`_STATE_INVERTED   Inverted display.
*
* Return:
*  None.
*
* Global variables:
*  `$INSTANCE_NAME`_invertState - holds the current LCD state (Inverted display
*                                 or Normal (non inverted) display).
*  `$INSTANCE_NAME`_commons[] - holds the pixel locations for common lines.
*
*******************************************************************************/
void `$INSTANCE_NAME`_WriteInvertState(uint32 invertState)
{
    uint32 i;
    uint32 j;

    if (invertState != `$INSTANCE_NAME`_invertState)
    {
        /* Invert entire frame buffer */
`$writerWriteInvertState`

        /* Reinitialize the commons */
        for (i = 0u; i < `$INSTANCE_NAME`_COM_NUM; i++)
        {
            /* Clear commons pin data after frame buffer inverting */
            for (j = 0u; j < `$INSTANCE_NAME`_COM_NUM; j++)
            {
                `$INSTANCE_NAME`_WriteFrameBuffer(`$INSTANCE_NAME`_DATA0_PTR,
                ((`$INSTANCE_NAME`_commons[i] & ((uint32)(~`$INSTANCE_NAME`_ROW_MASK))) |
                ((uint32)(j << `$INSTANCE_NAME`_ROW_SHIFT))), `$INSTANCE_NAME`_PIXEL_STATE_OFF);
            }

            /* Set pin data for commons */
            `$INSTANCE_NAME`_WriteFrameBuffer(`$INSTANCE_NAME`_DATA0_PTR, `$INSTANCE_NAME`_commons[i],
                                              `$INSTANCE_NAME`_PIXEL_STATE_ON);
        }

        `$INSTANCE_NAME`_invertState = invertState;
    }
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_ReadInvertState
********************************************************************************
*
* Summary:
*  Returns the current value of the display invert state: normal or inverted.
*
* Parameters:
*  None.
*
* Return:
*  The invert state of the display:
*    Define                            Description
*     `$INSTANCE_NAME`_STATE_NORMAL     Normal non inverted display.
*     `$INSTANCE_NAME`_STATE_INVERTED   Inverted display.
*
* Global variables:
*  `$INSTANCE_NAME`_invertState - holds the current LCD state (Inverted display
*                                 or Normal (non inverted) display).
*
*******************************************************************************/
uint32 `$INSTANCE_NAME`_ReadInvertState(void)
{
    return (`$INSTANCE_NAME`_invertState);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_ClearDisplay
********************************************************************************
*
* Summary:
*  This function clears the display and the associated frame buffer RAM.
*
* Parameters:
*  None.
*
* Return:
*  None.
*
* Global variables:
*  `$INSTANCE_NAME`_commons[] - holds the pixel locations for common lines.
*  `$INSTANCE_NAME`_invertState - holds the current LCD state (Inverted display
*                                 or Normal (non inverted) display).
*
*******************************************************************************/
void `$INSTANCE_NAME`_ClearDisplay(void)
{
    uint32 i;

    /* Clear entire frame buffer to all zeroes */
`$writerClearDisplay`

    /* Reinitialize the commons */
    for (i = 0u; i < `$INSTANCE_NAME`_COM_NUM; i++)
    {
        `$INSTANCE_NAME`_WriteFrameBuffer(`$INSTANCE_NAME`_DATA0_PTR, `$INSTANCE_NAME`_commons[i],
                                          `$INSTANCE_NAME`_PIXEL_STATE_ON);
    }

    /* If we were in inverted state before the display was cleared, then we must
    * call WriteInvertState() as there is no separate API to clear inverted
    * display.
    */
    if (`$INSTANCE_NAME`_invertState == `$INSTANCE_NAME`_STATE_INVERTED)
    {
        `$INSTANCE_NAME`_WriteInvertState(`$INSTANCE_NAME`_STATE_INVERTED);
    }
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_WritePixel
********************************************************************************
*
* Summary:
*  Sets or clears a pixel based on the input parameter PixelState. The pixel is
*  addressed by a packed number. This packed number comes from pixel mapping
*  table of Display helpers. Pixel mapping table used to map the helper function
*  pixel to the actual frame buffer pixel. All pixels have packed number defines
*  which are resides in component header file.
*
* Parameters:
*  pixelNumber : The predefined packed number that points to the pixel's
*                location in the frame buffer. The LSB are the bit position in
*                the byte, the LSB+1 is the byte address in the multiplex row
*                and the MSB-1 is the multiplex row number.
*  pixelState : The pixelNumber specified is set to this pixel state:
*    Define                                Description
*     `$INSTANCE_NAME`_PIXEL_STATE_ON        Pixel on.
*     `$INSTANCE_NAME`_PIXEL_STATE_OFF       Pixel off.
*     `$INSTANCE_NAME`_PIXEL_STATE_INVERT    Pixel invert.
*
* Return:
*  Pass or fail based on a range check of the pixelNumber address:
*    Define                     Description
*     CYRET_SUCCESS              Function completed successfully.
*     CYRET_BAD_PARAM            Evaluation of pixelNumber parameter is failed.
*
*******************************************************************************/
uint32 `$INSTANCE_NAME`_WritePixel(uint32 pixelNumber, uint32 pixelState)
{
    uint32 status = CYRET_BAD_PARAM;

    if ((`$INSTANCE_NAME`_NOT_CON != pixelNumber) && (pixelState <= `$INSTANCE_NAME`_PIXEL_STATE_INVERT))
    {
        if (`$INSTANCE_NAME`_PIXEL_STATE_INVERT == pixelState)
        {
            /* Invert actual pixel state */
            pixelState = `$INSTANCE_NAME`_ReadPixel(pixelNumber);
            pixelState = ((uint32)(~pixelState)) & `$INSTANCE_NAME`_PIXEL_STATE_ON;
        }

        /* Write new pixel's value to the frame buffer. */
        `$INSTANCE_NAME`_WriteFrameBuffer(`$INSTANCE_NAME`_DATA0_PTR, pixelNumber, pixelState);

        status = CYRET_SUCCESS;
    }

    return(status);
}


/*******************************************************************************
* Function Name: `$INSTANCE_NAME`_ReadPixel
********************************************************************************
*
* Summary:
*  Reads the state of a pixel in the frame buffer. The pixel is addressed by a
*  packed number. This packed number comes from pixel mapping table of Display
*  helpers. Pixel mapping table used to map the helper function pixel to the
*  actual frame buffer pixel. All pixels have packed number defines which are
*  resides in component header file.
*
* Parameters:
*  pixelNumber : The predefined packed number that points to the pixel's
*                location in the frame buffer. The LSB are the bit position in
*                the byte, the LSB+1 is the byte address in the multiplex row
*                and the MSB-1 is the multiplex row number.
*
* Return:
*  Returns the current status of the PixelNumber specified:
*    Define                                Description
*     `$INSTANCE_NAME`_PIXEL_STATE_ON        Pixel on.
*     `$INSTANCE_NAME`_PIXEL_STATE_OFF       Pixel off.
*     `$INSTANCE_NAME`_PIXEL_UNKNOWN_STATE   Not assigned pixel.
*
*******************************************************************************/
uint32 `$INSTANCE_NAME`_ReadPixel(uint32 pixelNumber)
{
    uint32 pixelState = `$INSTANCE_NAME`_PIXEL_UNKNOWN_STATE;

    if (`$INSTANCE_NAME`_NOT_CON != pixelNumber)
    {
        pixelState = `$INSTANCE_NAME`_ReadFrameBuffer(`$INSTANCE_NAME`_DATA0_PTR, pixelNumber);
    }

    return(pixelState);
}
`$writerCFunctions`

/* [] END OF FILE */
