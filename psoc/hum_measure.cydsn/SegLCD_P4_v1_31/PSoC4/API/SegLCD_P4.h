/*******************************************************************************
* File Name: `$INSTANCE_NAME`.h
* Version `$CY_MAJOR_VERSION`.`$CY_MINOR_VERSION`
*
* Description:
*  This file provides constants and parameter values for the Segment LCD
*  component.
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

#if !defined(CY_SegLCD_P4_`$INSTANCE_NAME`_H)
#define CY_SegLCD_P4_`$INSTANCE_NAME`_H

#include "cyfitter.h"
#include "cytypes.h"
#include "CyLib.h"


/***************************************
*   Conditional Compilation Parameters
****************************************/

#define `$INSTANCE_NAME`_SUBFR_DIV                (`$SubfrDiv`u)
#define `$INSTANCE_NAME`_DEAD_DIV                 (`$DeadDiv`u)
#define `$INSTANCE_NAME`_LCD_MODE                 (`$LCDMode`u)
#define `$INSTANCE_NAME`_WAVEFORM_TYPE            (`$WaveformType`u)
#define `$INSTANCE_NAME`_DRIVING_MODE             (`$DrivingMode`u)
#define `$INSTANCE_NAME`_BIAS                     (`$BiasType`u)
#define `$INSTANCE_NAME`_COM_NUM                  (`$NumCommonLines`u)
#define `$INSTANCE_NAME`_CONTRAST                 (`$Contrast`u)

#define `$INSTANCE_NAME`_BUFFER_LENGTH            (0x05u)


/***************************************
*        Function Prototypes
***************************************/

void    `$INSTANCE_NAME`_Init(void);
void    `$INSTANCE_NAME`_Enable(void);
void    `$INSTANCE_NAME`_Start(void);
void    `$INSTANCE_NAME`_Stop(void);
void    `$INSTANCE_NAME`_SetSpeedMode(uint32 mode);
void    `$INSTANCE_NAME`_SetOperationMode(uint32 mode);
void    `$INSTANCE_NAME`_SetBiasType(uint32 bias);
void    `$INSTANCE_NAME`_SetWaveformType(uint32 type);
uint32  `$INSTANCE_NAME`_SetContrast(uint32 contrast);
void    `$INSTANCE_NAME`_WriteInvertState(uint32 invertState);
uint32  `$INSTANCE_NAME`_ReadInvertState(void);
void    `$INSTANCE_NAME`_ClearDisplay(void);
uint32  `$INSTANCE_NAME`_WritePixel(uint32 pixelNumber, uint32 pixelState);
uint32  `$INSTANCE_NAME`_ReadPixel(uint32 pixelNumber);
void    `$INSTANCE_NAME`_Sleep(void);
void    `$INSTANCE_NAME`_Wakeup(void);
void    `$INSTANCE_NAME`_SaveConfig(void);
void    `$INSTANCE_NAME`_RestoreConfig(void);

`$writerHFuncDeclarations`

/* Calculates pixel location in the frame buffer. */
#define `$INSTANCE_NAME`_FIND_PIXEL(port, pin, row)    (((uint32)((uint32)(row) << `$INSTANCE_NAME`_ROW_SHIFT)) | \
                                   ((uint32)((uint32)(port) << `$INSTANCE_NAME`_BYTE_SHIFT)) | ((uint32)(pin)))

/* Internal macros that extract pixel information from a pixel number */
#define `$INSTANCE_NAME`_EXTRACT_ROW(pixel)       ((uint32)(((pixel) & `$INSTANCE_NAME`_ROW_MASK) >> \
                                                                      `$INSTANCE_NAME`_ROW_SHIFT))
#define `$INSTANCE_NAME`_EXTRACT_PORT(pixel)      ((uint32)(((pixel) & `$INSTANCE_NAME`_BYTE_MASK) >> \
                                                                      `$INSTANCE_NAME`_BYTE_SHIFT))
#define `$INSTANCE_NAME`_EXTRACT_PIN(pixel)       ((uint32)((pixel) & `$INSTANCE_NAME`_BIT_MASK))


/***************************************
*           Global Variables
***************************************/

extern uint32 `$INSTANCE_NAME`_initVar;


/***************************************
*           API Constants
***************************************/

#define `$INSTANCE_NAME`_NOT_CON                  (0xFFFFu)

#define `$INSTANCE_NAME`_MAX_CONTRAST             (100u)
#define `$INSTANCE_NAME`_MIN_CONTRAST             (10u)
#define `$INSTANCE_NAME`_CONTRAST_DIVIDERS_NUMBER (10u)

#define `$INSTANCE_NAME`_ROW_SHIFT                (0x10u)
#define `$INSTANCE_NAME`_ROW_MASK                 ((uint32)((uint32)0xFFu << `$INSTANCE_NAME`_ROW_SHIFT))
#define `$INSTANCE_NAME`_BYTE_SHIFT               (0x08u)
#define `$INSTANCE_NAME`_BYTE_MASK                ((uint32)((uint32)0xFFu << `$INSTANCE_NAME`_BYTE_SHIFT))
#define `$INSTANCE_NAME`_BIT_SHIFT                (0x00u)
#define `$INSTANCE_NAME`_BIT_MASK                 ((uint32)((uint32)0xFFu << `$INSTANCE_NAME`_BIT_SHIFT))

#define `$INSTANCE_NAME`_ENABLE_MASK              ((uint32)0x03u)
#define `$INSTANCE_NAME`_SPEED_LS                 (0x00u)
#define `$INSTANCE_NAME`_SPEED_HS                 (0x01u)
#define `$INSTANCE_NAME`_MODE_PWM                 (0x00u)
#define `$INSTANCE_NAME`_MODE_DIG_COR             (0x01u)
#define `$INSTANCE_NAME`_BIAS_1_2                 (0x00u)
#define `$INSTANCE_NAME`_BIAS_1_3                 (0x01u)
#define `$INSTANCE_NAME`_BIAS_1_4                 (0x02u)
#define `$INSTANCE_NAME`_BIAS_1_5                 (0x03u)
#define `$INSTANCE_NAME`_TYPE_A                   (0x00u)
#define `$INSTANCE_NAME`_TYPE_B                   (0x01u)

#define `$INSTANCE_NAME`_STATE_NORMAL             (0x00u)
#define `$INSTANCE_NAME`_STATE_INVERTED           (0x01u)

/* Number of pixels for different kind of LCDs */
#define `$INSTANCE_NAME`_7SEG_PIX_NUM             (0x07u)
#define `$INSTANCE_NAME`_14SEG_PIX_NUM            (0x0Eu)
#define `$INSTANCE_NAME`_16SEG_PIX_NUM            (0x10u)
#define `$INSTANCE_NAME`_DM_CHAR_HEIGHT           (0x08u)
#define `$INSTANCE_NAME`_DM_CHAR_WIDTH            (0x05u)

/* API parameter pixel state constants */
#define `$INSTANCE_NAME`_PIXEL_STATE_OFF          ((uint32)0x00u)
#define `$INSTANCE_NAME`_PIXEL_STATE_ON           ((uint32)0x01u)
#define `$INSTANCE_NAME`_PIXEL_STATE_INVERT       ((uint32)0x02u)
#define `$INSTANCE_NAME`_PIXEL_UNKNOWN_STATE      ((uint32)0xFFu)

/* 0 - No leading zeros, 1 - leading zeros */
#define `$INSTANCE_NAME`_NO_LEADING_ZEROES        (0x00u)
#define `$INSTANCE_NAME`_LEADING_ZEROES           (0x01u)

#define `$INSTANCE_NAME`_DEAD_DIVS                (0x00u)
#define `$INSTANCE_NAME`_SUBFR_DIVS               (0x01u)

#define `$INSTANCE_NAME`_MAX_BUFF_ROWS            (0x04u)

#define `$INSTANCE_NAME`_PINS_PER_PORT            (0x08u)

`$writerHPixelDef`


/***************************************
*    Enumerated Types and Parameters
***************************************/


/***************************************
*    Initial Parameter Constants
***************************************/


/***************************************
*             Registers
***************************************/

/* LCD's fixed block registers */
#define `$INSTANCE_NAME`_DIVIDER_REG              (*(reg32*) CYREG_LCD_DIVIDER)
#define `$INSTANCE_NAME`_DIVIDER_PTR              ((reg32*) CYREG_LCD_DIVIDER)
#define `$INSTANCE_NAME`_CONTROL_REG              (*(reg32*) CYREG_LCD_CONTROL)
#define `$INSTANCE_NAME`_CONTROL_PTR              ((reg32*) CYREG_LCD_CONTROL)
`$writerHRegistersDef`


/***************************************
*       Register Constants
***************************************/

/* Offset between LCD Pin Data Registers for different Commons (0x0100) divided by number of byte in one Register (4) */
#define `$INSTANCE_NAME`_DATA_REGS_OFFSET         (64u)

/* Divider Register bits */
#define `$INSTANCE_NAME`_SUBFR_DIV_MASK_SHIFT     (0x00u)
#define `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT      (0x10u)

#define `$INSTANCE_NAME`_SUBFR_DIV_MASK           ((uint32)((uint32)0xFFFFu << `$INSTANCE_NAME`_SUBFR_DIV_MASK_SHIFT))
#define `$INSTANCE_NAME`_DEAD_DIV_MASK            ((uint32)((uint32)0xFFFFu << `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT))
#define `$INSTANCE_NAME`_DIVIDER_MASK             (`$INSTANCE_NAME`_DEAD_DIV_MASK | `$INSTANCE_NAME`_SUBFR_DIV_MASK)

#define `$INSTANCE_NAME`_SUBFR_DIVIDER            ((uint32)((uint32)`$INSTANCE_NAME`_SUBFR_DIV << \
                                                                    `$INSTANCE_NAME`_SUBFR_DIV_MASK_SHIFT))
#define `$INSTANCE_NAME`_DEAD_DIVIDER             ((uint32)((uint32)`$INSTANCE_NAME`_DEAD_DIV << \
                                                                    `$INSTANCE_NAME`_DEAD_DIV_MASK_SHIFT))
#define `$INSTANCE_NAME`_INIT_DIVIDERS            (`$INSTANCE_NAME`_SUBFR_DIVIDER | `$INSTANCE_NAME`_DEAD_DIVIDER)

/* Control Register bits */
#define `$INSTANCE_NAME`_LS_EN_SHIFT              (0x00u)
#define `$INSTANCE_NAME`_HS_EN_SHIFT              (0x01u)
#define `$INSTANCE_NAME`_MODE_SHIFT               (0x02u)
#define `$INSTANCE_NAME`_TYPE_SHIFT               (0x03u)
#define `$INSTANCE_NAME`_OP_MODE_SHIFT            (0x04u)
#define `$INSTANCE_NAME`_BIAS_MASK_SHIFT          (0x05u)
#define `$INSTANCE_NAME`_COM_NUM_MASK_SHIFT       (0x08u)
#define `$INSTANCE_NAME`_LS_EN_STAT_SHIFT         (0x1Fu)

#define `$INSTANCE_NAME`_LS_EN                    ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_LS_EN_SHIFT))
#define `$INSTANCE_NAME`_HS_EN                    ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_HS_EN_SHIFT))
#define `$INSTANCE_NAME`_MODE                     ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_MODE_SHIFT))
#define `$INSTANCE_NAME`_TYPE                     ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_TYPE_SHIFT))
#define `$INSTANCE_NAME`_OP_MODE                  ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_OP_MODE_SHIFT))
#define `$INSTANCE_NAME`_BIAS_MASK                ((uint32)((uint32)0x03u << `$INSTANCE_NAME`_BIAS_MASK_SHIFT))
#define `$INSTANCE_NAME`_COM_NUM_MASK             ((uint32)((uint32)0x0Fu << `$INSTANCE_NAME`_COM_NUM_MASK_SHIFT))
#define `$INSTANCE_NAME`_LS_EN_STAT               ((uint32)((uint32)0x01u << `$INSTANCE_NAME`_LS_EN_STAT_SHIFT))
#define `$INSTANCE_NAME`_CONFIG_MASK              ((uint32)(~(`$INSTANCE_NAME`_MODE | `$INSTANCE_NAME`_TYPE | \
                                                   `$INSTANCE_NAME`_OP_MODE | `$INSTANCE_NAME`_BIAS_MASK | \
                                                   `$INSTANCE_NAME`_COM_NUM_MASK)))

#define `$INSTANCE_NAME`_LCD_SPEED_MODE           ((uint32)((uint32)`$INSTANCE_NAME`_LCD_MODE << \
                                                                    `$INSTANCE_NAME`_MODE_SHIFT))
#define `$INSTANCE_NAME`_WAVEFORMS_TYPE           ((uint32)((uint32)`$INSTANCE_NAME`_WAVEFORM_TYPE << \
                                                                    `$INSTANCE_NAME`_TYPE_SHIFT))
#define `$INSTANCE_NAME`_DRIVING_OP_MODE          ((uint32)((uint32)`$INSTANCE_NAME`_DRIVING_MODE << \
                                                                    `$INSTANCE_NAME`_OP_MODE_SHIFT))
#define `$INSTANCE_NAME`_PWM_BIAS                 ((uint32)((uint32)`$INSTANCE_NAME`_BIAS << \
                                                                    `$INSTANCE_NAME`_BIAS_MASK_SHIFT))
#define `$INSTANCE_NAME`_COM_NUMBER               ((uint32)((uint32)(`$INSTANCE_NAME`_COM_NUM - 2u) << \
                                                                     `$INSTANCE_NAME`_COM_NUM_MASK_SHIFT))
#define `$INSTANCE_NAME`_INIT_CONTROL             (`$INSTANCE_NAME`_LCD_SPEED_MODE | `$INSTANCE_NAME`_WAVEFORMS_TYPE | \
                                                   `$INSTANCE_NAME`_DRIVING_OP_MODE | `$INSTANCE_NAME`_PWM_BIAS | \
                                                   `$INSTANCE_NAME`_COM_NUMBER)

#endif /* End CY_SegLCD_P4_`$INSTANCE_NAME`_H */


/* [] END OF FILE */
