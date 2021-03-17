/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SegLCD_P4_v1_31
{
    public partial class CyCustomCharsEditor : CyEditingWrapperControl
    {
        #region CHAR_DOT_MATRIX const
        private static readonly byte[,] CHAR_DOT_MATRIX = {
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0x3E, 0x1C, 0x1C, 0x08, 0x08},
                                                    {0x08, 0x08, 0x1C, 0x1C, 0x3E},
                                                    {0x02, 0x04, 0x08, 0x10, 0x20}, {0x00, 0x4F, 0x00, 0x4F, 0x00},
                                                    {0x06, 0x0F, 0x7F, 0x01, 0x7F},
                                                    {0x48, 0x56, 0x55, 0x35, 0x09}, {0x0C, 0x0C, 0x0C, 0x0C, 0x0C},
                                                    {0x10, 0x38, 0x54, 0x10, 0x1F},
                                                    {0x04, 0x02, 0x7F, 0x02, 0x04}, {0x10, 0x20, 0x7F, 0x20, 0x10},
                                                    {0x7F, 0x3E, 0x1C, 0x08, 0x7F},
                                                    {0x7F, 0x08, 0x1C, 0x3E, 0x7F}, {0x7F, 0x08, 0x2A, 0x1C, 0x08},
                                                    {0x08, 0x1C, 0x2A, 0x08, 0x7F},
                                                    {0x02, 0x0E, 0x3E, 0x0E, 0x02}, {0x20, 0x38, 0x3E, 0x38, 0x20},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x00, 0x00, 0x4F, 0x00, 0x00}, {0x00, 0x07, 0x00, 0x07, 0x00},
                                                    {0x14, 0x7F, 0x14, 0x7F, 0x14},
                                                    {0x24, 0x2A, 0x7F, 0x2A, 0x12}, {0x23, 0x13, 0x08, 0x64, 0x62},
                                                    {0x36, 0x49, 0x55, 0x22, 0x50},
                                                    {0x00, 0x05, 0x03, 0x00, 0x00}, {0x00, 0x1C, 0x22, 0x41, 0x00},
                                                    {0x00, 0x41, 0x22, 0x1C, 0x00},
                                                    {0x14, 0x08, 0x3E, 0x08, 0x14}, {0x08, 0x08, 0x3E, 0x08, 0x08},
                                                    {0x00, 0x50, 0x30, 0x00, 0x00},
                                                    {0x08, 0x08, 0x08, 0x08, 0x08}, {0x00, 0x60, 0x60, 0x00, 0x00},
                                                    {0x20, 0x10, 0x08, 0x04, 0x02},
                                                    {0x3E, 0x51, 0x49, 0x45, 0x3E}, {0x00, 0x42, 0x7F, 0x40, 0x00},
                                                    {0x42, 0x61, 0x51, 0x49, 0x46},
                                                    {0x21, 0x41, 0x45, 0x4B, 0x31}, {0x18, 0x14, 0x12, 0x7F, 0x10},
                                                    {0x27, 0x45, 0x45, 0x45, 0x39},
                                                    {0x3C, 0x4A, 0x49, 0x49, 0x30}, {0x01, 0x71, 0x09, 0x05, 0x03},
                                                    {0x36, 0x49, 0x49, 0x49, 0x36},
                                                    {0x06, 0x49, 0x49, 0x29, 0x1E}, {0x00, 0x36, 0x36, 0x00, 0x00},
                                                    {0x00, 0x56, 0x36, 0x00, 0x00},
                                                    {0x08, 0x14, 0x22, 0x41, 0x00}, {0x14, 0x14, 0x14, 0x14, 0x14},
                                                    {0x00, 0x41, 0x22, 0x14, 0x08},
                                                    {0x02, 0x01, 0x51, 0x09, 0x06}, {0x32, 0x49, 0x79, 0x41, 0x3E},
                                                    {0x7E, 0x11, 0x11, 0x11, 0x7E},
                                                    {0x7F, 0x49, 0x49, 0x49, 0x36}, {0x3E, 0x41, 0x41, 0x41, 0x22},
                                                    {0x7F, 0x41, 0x41, 0x22, 0x1C},
                                                    {0x7F, 0x49, 0x49, 0x49, 0x41}, {0x7F, 0x09, 0x09, 0x09, 0x01},
                                                    {0x3E, 0x41, 0x49, 0x49, 0x3A},
                                                    {0x7F, 0x08, 0x08, 0x08, 0x7F}, {0x00, 0x41, 0x7F, 0x41, 0x00},
                                                    {0x20, 0x40, 0x41, 0x3F, 0x01},
                                                    {0x7F, 0x08, 0x14, 0x22, 0x41}, {0x7F, 0x40, 0x40, 0x40, 0x40},
                                                    {0x7F, 0x02, 0x0C, 0x02, 0x7F},
                                                    {0x7F, 0x04, 0x08, 0x10, 0x7F}, {0x3E, 0x41, 0x41, 0x41, 0x3E},
                                                    {0x7F, 0x09, 0x09, 0x09, 0x06},
                                                    {0x3E, 0x41, 0x51, 0x21, 0x5E}, {0x7F, 0x09, 0x19, 0x29, 0x46},
                                                    {0x46, 0x49, 0x49, 0x49, 0x31},
                                                    {0x01, 0x01, 0x7F, 0x01, 0x01}, {0x3F, 0x40, 0x40, 0x40, 0x3F},
                                                    {0x1F, 0x20, 0x40, 0x20, 0x1F},
                                                    {0x3F, 0x40, 0x38, 0x40, 0x3F}, {0x63, 0x14, 0x08, 0x14, 0x63},
                                                    {0x07, 0x08, 0x70, 0x08, 0x07},
                                                    {0x61, 0x51, 0x49, 0x45, 0x43}, {0x00, 0x7F, 0x41, 0x41, 0x00},
                                                    {0x15, 0x16, 0x7C, 0x16, 0x15},
                                                    {0x00, 0x41, 0x41, 0x7F, 0x00}, {0x04, 0x02, 0x01, 0x02, 0x04},
                                                    {0x40, 0x40, 0x40, 0x40, 0x40},
                                                    {0x00, 0x01, 0x02, 0x04, 0x00}, {0x20, 0x54, 0x54, 0x54, 0x78},
                                                    {0x7F, 0x48, 0x44, 0x44, 0x38},
                                                    {0x38, 0x44, 0x44, 0x44, 0x20}/*100*/, {0x38, 0x44, 0x44, 0x48, 0x7F},
                                                    {0x38, 0x54, 0x54, 0x54, 0x18},
                                                    {0x08, 0x7E, 0x09, 0x01, 0x02}, {0x0C, 0x52, 0x52, 0x52, 0x3E},
                                                    {0x7F, 0x08, 0x04, 0x04, 0x78},
                                                    {0x00, 0x44, 0x7D, 0x40, 0x00}, {0x20, 0x40, 0x44, 0x3D, 0x00},
                                                    {0x7F, 0x10, 0x28, 0x44, 0x00},
                                                    {0x00, 0x41, 0x7F, 0x40, 0x00}, {0x7C, 0x04, 0x18, 0x04, 0x78},
                                                    {0x7C, 0x08, 0x04, 0x04, 0x78},
                                                    {0x38, 0x44, 0x44, 0x44, 0x38}, {0x7C, 0x14, 0x14, 0x14, 0x08},
                                                    {0x08, 0x14, 0x14, 0x18, 0x7C},
                                                    {0x7C, 0x08, 0x04, 0x04, 0x08}, {0x48, 0x54, 0x54, 0x54, 0x20},
                                                    {0x04, 0x3F, 0x44, 0x40, 0x20},
                                                    {0x3C, 0x40, 0x40, 0x20, 0x7C}, {0x1C, 0x20, 0x40, 0x20, 0x1C},
                                                    {0x3C, 0x40, 0x30, 0x40, 0x3C},
                                                    {0x44, 0x28, 0x10, 0x28, 0x44}, {0x0C, 0x50, 0x50, 0x50, 0x3C},
                                                    {0x44, 0x64, 0x54, 0x4C, 0x44},
                                                    {0x00, 0x08, 0x36, 0x41, 0x00}, {0x00, 0x00, 0x7F, 0x00, 0x00},
                                                    {0x00, 0x41, 0x36, 0x08, 0x00},
                                                    {0x08, 0x08, 0x2A, 0x1C, 0x08}, {0x08, 0x1C, 0x2A, 0x08, 0x08},
                                                    {0x44, 0x44, 0x5F, 0x44, 0x44},
                                                    {0x22, 0x14, 0x08, 0x14, 0x22}, {0x1C, 0x3E, 0x3E, 0x3E, 0x1C},
                                                    {0x7F, 0x41, 0x41, 0x41, 0x7F},
                                                    {0x7F, 0x5B, 0x41, 0x5F, 0x7F}, {0x7F, 0x4D, 0x55, 0x59, 0x7F},
                                                    {0x1D, 0x15, 0x17, 0x00, 0x00},
                                                    {0x15, 0x15, 0x1F, 0x00, 0x00}, {0x17, 0x08, 0x74, 0x56, 0x5D},
                                                    {0x17, 0x08, 0x24, 0x32, 0x79},
                                                    {0x35, 0x1F, 0x28, 0x34, 0x7A}, {0x08, 0x14, 0x2A, 0x14, 0x22},
                                                    {0x22, 0x14, 0x2A, 0x14, 0x08},
                                                    {0x08, 0x04, 0x08, 0x10, 0x08}, {0x14, 0x0A, 0x14, 0x28, 0x14},
                                                    {0x2A, 0x55, 0x2A, 0x55, 0x2A},
                                                    {0x24, 0x3B, 0x2A, 0x7E, 0x2A}, {0x40, 0x3F, 0x15, 0x15, 0x7F},
                                                    {0x46, 0x20, 0x1F, 0x20, 0x46},
                                                    {0x24, 0x14, 0x7F, 0x18, 0x24}, {0x24, 0x14, 0x7F, 0x14, 0x24},
                                                    {0x44, 0x6A, 0x79, 0x6A, 0x44},
                                                    {0x40, 0x44, 0x7F, 0x44, 0x40}, {0x7F, 0x49, 0x49, 0x49, 0x7F},
                                                    {0x02, 0x4C, 0x30, 0x0C, 0x02},
                                                    {0x04, 0x04, 0x3C, 0x44, 0x44}, {0x49, 0x55, 0x7F, 0x55, 0x49},
                                                    {0x3A, 0x45, 0x45, 0x45, 0x39},
                                                    {0x40, 0x3E, 0x10, 0x10, 0x1E}, {0x08, 0x54, 0x3E, 0x15, 0x08},
                                                    {0x7F, 0x7F, 0x7F, 0x7F, 0x7F},
                                                    {0x55, 0x2A, 0x55, 0x2A, 0x55}, {0x00, 0x00, 0x00, 0x00, 0x00},
                                                    {0x70, 0x50, 0x70, 0x00, 0x00},
                                                    {0x00, 0x00, 0x0F, 0x01, 0x01}, {0x40, 0x40, 0x70, 0x00, 0x00},
                                                    {0x10, 0x20, 0x40, 0x00, 0x00},
                                                    {0x00, 0x18, 0x18, 0x00, 0x00}, {0x0A, 0x0A, 0x4A, 0x2A, 0x1E},
                                                    {0x04, 0x44, 0x34, 0x14, 0x0C},
                                                    {0x20, 0x10, 0x78, 0x04, 0x00}, {0x18, 0x08, 0x4C, 0x48, 0x38},
                                                    {0x48, 0x48, 0x78, 0x48, 0x48},
                                                    {0x48, 0x28, 0x18, 0x7C, 0x08}, {0x08, 0x7C, 0x08, 0x28, 0x18},
                                                    {0x40, 0x48, 0x48, 0x78, 0x40},
                                                    {0x54, 0x54, 0x54, 0x7C, 0x00}, {0x18, 0x00, 0x58, 0x40, 0x38},
                                                    {0x08, 0x08, 0x08, 0x08, 0x08},
                                                    {0x01, 0x41, 0x3D, 0x09, 0x07}, {0x10, 0x08, 0x7C, 0x02, 0x01},
                                                    {0x06, 0x02, 0x43, 0x22, 0x1E},
                                                    {0x42, 0x42, 0x7E, 0x42, 0x42}, {0x22, 0x12, 0x0A, 0x7F, 0x02},
                                                    {0x42, 0x3F, 0x02, 0x42, 0x3E},
                                                    {0x0A, 0x0A, 0x7F, 0x0A, 0x0A}, {0x08, 0x46, 0x42, 0x22, 0x1E},
                                                    {0x04, 0x03, 0x42, 0x3E, 0x02},
                                                    {0x42, 0x42, 0x42, 0x42, 0x7E}, {0x02, 0x4F, 0x22, 0x1F, 0x02},
                                                    {0x4A, 0x4A, 0x40, 0x20, 0x1C},
                                                    {0x42, 0x22, 0x12, 0x2A, 0x46}, {0x02, 0x3F, 0x42, 0x4A, 0x46},
                                                    {0x06, 0x48, 0x40, 0x20, 0x1E},
                                                    {0x08, 0x46, 0x4A, 0x32, 0x1E}, {0x0A, 0x4A, 0x3E, 0x09, 0x08},
                                                    {0x0E, 0x00, 0x4E, 0x20, 0x1E},
                                                    {0x04, 0x45, 0x3D, 0x05, 0x04}, {0x00, 0x7F, 0x08, 0x10, 0x00},
                                                    {0x44, 0x24, 0x1F, 0x04, 0x04},
                                                    {0x40, 0x42, 0x42, 0x42, 0x40}, {0x42, 0x2A, 0x12, 0x2A, 0x06},
                                                    {0x22, 0x12, 0x7B, 0x16, 0x22},
                                                    {0x00, 0x40, 0x20, 0x1F, 0x00}, {0x78, 0x00, 0x02, 0x04, 0x78},
                                                    {0x3F, 0x44, 0x44, 0x44, 0x44},
                                                    {0x02, 0x42, 0x42, 0x22, 0x1E}, {0x04, 0x02, 0x04, 0x08, 0x30},
                                                    {0x32, 0x02, 0x7F, 0x02, 0x32},
                                                    {0x02, 0x12, 0x22, 0x52, 0x0E}, {0x00, 0x2A, 0x2A, 0x2A, 0x40},
                                                    {0x38, 0x24, 0x22, 0x20, 0x70},
                                                    {0x40, 0x28, 0x10, 0x28, 0x06}, {0x0A, 0x3E, 0x4A, 0x4A, 0x4A},
                                                    {0x04, 0x7F, 0x04, 0x14, 0x0C},
                                                    {0x40, 0x42, 0x42, 0x7E, 0x40}, {0x4A, 0x4A, 0x4A, 0x4A, 0x7E},
                                                    {0x04, 0x05, 0x45, 0x25, 0x1C},
                                                    {0x0F, 0x40, 0x20, 0x1F, 0x00}, {0x7C, 0x00, 0x7E, 0x40, 0x30},
                                                    {0x7E, 0x40, 0x20, 0x10, 0x08},
                                                    {0x7E, 0x42, 0x42, 0x42, 0x7E}, {0x0E, 0x02, 0x42, 0x22, 0x1E},
                                                    {0x42, 0x42, 0x40, 0x20, 0x18},
                                                    {0x02, 0x04, 0x01, 0x02, 0x00}, {0x07, 0x05, 0x07, 0x00, 0x00},
                                                    {0x38, 0x44, 0x48, 0x30, 0x4C},
                                                    {0x20, 0x55, 0x54, 0x55, 0x78}, {0xF8, 0x54, 0x54, 0x54, 0x28},
                                                    {0x28, 0x54, 0x54, 0x44, 0x20},
                                                    {0xFF, 0x10, 0x10, 0x08, 0x1F}, {0x38, 0x44, 0x4C, 0x54, 0x24},
                                                    {0xF8, 0x24, 0x22, 0x22, 0x1C},
                                                    {0x1E, 0xA1, 0xA1, 0xA1, 0x7F}, {0x20, 0x40, 0x3C, 0x04, 0x04},
                                                    {0x04, 0x04, 0x00, 0x0E, 0x00},
                                                    {0x40, 0x80, 0x84, 0x7D, 0x00}, {0x0A, 0x04, 0x0A, 0x00, 0x00},
                                                    {0x18, 0x24, 0x7E, 0x24, 0x10},
                                                    {0x14, 0x7F, 0x54, 0x40, 0x40}, {0x7C, 0x09, 0x05, 0x05, 0x78},
                                                    {0x38, 0x45, 0x44, 0x45, 0x38},
                                                    {0xFF, 0x12, 0x11, 0x11, 0x0E}, {0x0E, 0x11, 0x11, 0x12, 0xFF},
                                                    {0x3C, 0x4A, 0x4A, 0x4A, 0x3C},
                                                    {0x30, 0x28, 0x10, 0x28, 0x18}, {0x58, 0x64, 0x04, 0x64, 0x58},
                                                    {0x3C, 0x41, 0x40, 0x21, 0x7C},
                                                    {0x63, 0x55, 0x49, 0x41, 0x41}, {0x44, 0x3C, 0x04, 0x7C, 0x44},
                                                    {0x45, 0x29, 0x11, 0x29, 0x45},
                                                    {0x0F, 0x90, 0x90, 0x90, 0x7F}, {0x14, 0x14, 0x7C, 0x14, 0x12},
                                                    {0x44, 0x3C, 0x14, 0x14, 0x74},
                                                    {0x7C, 0x14, 0x1C, 0x14, 0x7C}, {0x10, 0x10, 0x54, 0x10, 0x10},
                                                    {0x00, 0x00, 0x00, 0x00, 0x00}, {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}
                                        };

        #endregion CHAR_DOT_MATRIX const

        #region Fields, Properties
        private byte[,] m_defaultCharsArray;
        private byte[,] m_currentCharsArray;
        private List<int> m_changedCharsArray = new List<int>();
        private bool m_isCharsArrayDefault;
        private bool m_isAnyCharChanged;

        private bool IsCharsArrayDefault
        {
            get { return m_isCharsArrayDefault; }
            set
            {
                m_isCharsArrayDefault = value;
                if (m_isCharsArrayDefault)
                    toolStripButtonDefaultList.Enabled = false;
                else
                    toolStripButtonDefaultList.Enabled = true;
            }
        }

        private bool IsAnyCharChanged
        {
            get { return m_isAnyCharChanged; }
            set
            {
                m_isAnyCharChanged = value;
                if (m_isAnyCharChanged)
                    toolStripButtonResetAll.Enabled = true;
                else
                    toolStripButtonResetAll.Enabled = false;
            }
        }

        public byte[,] CurrentCharsArray
        {
            get
            {
                return m_currentCharsArray;
            }
            set
            {
                m_currentCharsArray = value;
                m_parameters.CustomCharsList = Byte2DArrayToString(m_currentCharsArray);
            }
        }

        #endregion Fields, Properties

        #region Constructors

        public CyCustomCharsEditor()
        {
            InitializeComponent();
        }

        public CyCustomCharsEditor(CyLCDParameters parameters)
            : base(parameters)
        {
            InitializeComponent();

            cyCustomCharacter1.CharChangedEvent += new EventHandler(cyCustomCharacter1_CharChangedEvent);

            if (String.IsNullOrEmpty(m_parameters.CustomCharsList) || (m_parameters.CustomCharsList.Length < 3))
            {
                m_defaultCharsArray = (byte[,])CHAR_DOT_MATRIX.Clone();
            }
            else
            {
                try
                {
                    m_defaultCharsArray = StringToByte2DArray(m_parameters.CustomCharsList);
                }
                catch
                {
                    MessageBox.Show(String.Format(Resources.WRONG_FORMAT_PARAM_MSG,
                                    CyLCDParameters.PARAM_CUSTOMCHARSLIST),
                                    Resources.ERROR_MSG_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            IsCharsArrayDefault = ArraysEqual(CHAR_DOT_MATRIX, m_defaultCharsArray);
            CurrentCharsArray = (byte[,])m_defaultCharsArray.Clone();
        }
        #endregion Constructors

        #region Events

        private void CyCharsCustomizer_Load(object sender, EventArgs e)
        {
            cyCustomCharacter1.CheckBoxSize();
            UpdateList();
        }

        private void listBoxChars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxChars.SelectedIndex > -1)
            {
                ChangeChar(listBoxChars.SelectedIndex);

                toolStripButtonReset.Enabled = true;
            }
            else
            {
                toolStripButtonReset.Enabled = false;
            }
        }

        void cyCustomCharacter1_CharChangedEvent(object sender, EventArgs e)
        {
            int selIndex = listBoxChars.SelectedIndex;
            if (selIndex > -1)
            {
                for (int i = 0; i < m_currentCharsArray.GetLength(1); i++)
                {
                    m_currentCharsArray[selIndex, i] = cyCustomCharacter1.m_charCode[i];
                }
                IsAnyCharChanged = true;
                CurrentCharsArray = m_currentCharsArray;

                // Add char index to the changed chars array or remove from it
                if (!m_changedCharsArray.Contains(selIndex))
                    m_changedCharsArray.Add(selIndex);
                bool isEqual = true;
                for (int i = 0; i < m_currentCharsArray.GetLength(1); i++)
                {
                    if (m_currentCharsArray[selIndex, i] != m_defaultCharsArray[selIndex, i])
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                    m_changedCharsArray.Remove(selIndex);

                // Repaint preview char
                pictureBoxCharPreview.Image = GetCharBitmap(selIndex);
            }
        }

        private void listBoxChars_Format(object sender, ListControlConvertEventArgs e)
        {
            string arg = (e.ListItem.ToString() == "\0") ? "" : e.ListItem.ToString();
            e.Value = string.Format("Char {0} ({1})", listBoxChars.Items.IndexOf(e.ListItem), arg);
        }

        private void toolStripButtonResetAll_Click(object sender, EventArgs e)
        {
            DialogResult dres = MessageBox.Show(Resources.CHARS_RESET_MSG,
                                                Resources.QUESTION_MSG_TITLE,
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dres == DialogResult.Yes)
            {
                ResetAllChars();
                ResetChar(listBoxChars.SelectedIndex);
                IsAnyCharChanged = false;
            }
        }

        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            ResetChar(listBoxChars.SelectedIndex);
        }

        private void toolStripButtonLoad_Click(object sender, EventArgs e)
        {
            LoadCharsTemplate();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveCharsTemplate();
        }

        private void toolStripButtonDefaultList_Click(object sender, EventArgs e)
        {
            DialogResult dres = MessageBox.Show(Resources.USE_DEFAULT_CHAR_LIST_MSG,
                                                Resources.QUESTION_MSG_TITLE, MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
            if (dres == DialogResult.Yes)
            {
                m_defaultCharsArray = (byte[,])CHAR_DOT_MATRIX.Clone();
                ChangeAlphabet();
                IsCharsArrayDefault = true;
            }
        }

        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            cyCustomCharacter1.Left = Math.Max(1, (splitContainer1.Panel2.Width - cyCustomCharacter1.Width) / 2);
            labelCharEditor.Left = Math.Max(1, (splitContainer1.Panel2.Width - labelCharEditor.Width) / 2);
        }

        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            labelCharList.Left = Math.Max(1, (splitContainer1.Panel1.Width - labelCharList.Width) / 2);
        }
        #endregion Events

        #region ProcessCmdKey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool result = true;

            if (keyData == (Keys.Control | Keys.O))
            {
                LoadCharsTemplate();
            }
            else if (keyData == (Keys.Control | Keys.S))
            {
                SaveCharsTemplate();
            }
            else
            {
                result = base.ProcessCmdKey(ref msg, keyData);
            }

            return result;
        }
        #endregion ProcessCmdKey

        #region Private functions

        private void UpdateList()
        {
            listBoxChars.Items.Clear();
            for (int i = 0; i < m_currentCharsArray.GetLength(0); i++)
            {
                listBoxChars.Items.Add((char)i);
            }
            if (listBoxChars.Items.Count > 0)
                listBoxChars.SelectedIndex = 0;
        }

        private void ChangeChar(int index)
        {
            if (index > -1)
            {
                for (int i = 0; i < m_currentCharsArray.GetLength(1); i++)
                {
                    cyCustomCharacter1.m_charCode[i] = m_currentCharsArray[index, i];
                }
                pictureBoxCharPreview.Image = GetCharBitmap(index);
                cyCustomCharacter1.ChangeChar();
            }
            else
            {
                cyCustomCharacter1.ResetAll();
                pictureBoxCharPreview.Image = null;
            }
        }

        private void ResetChar(int index)
        {
            if (index > -1)
            {
                for (int i = 0; i < m_currentCharsArray.GetLength(1); i++)
                {
                    m_currentCharsArray[index, i] = m_defaultCharsArray[index, i];
                }
                m_changedCharsArray.Remove(index);
            }
            ChangeChar(index);
            CurrentCharsArray = m_currentCharsArray;
        }

        private void ResetAllChars()
        {
            CurrentCharsArray = (byte[,])m_defaultCharsArray.Clone();
            m_changedCharsArray.Clear();
        }

        private void SaveCharsTemplate()
        {
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                saveFileDialog1.Filter = Resources.CHARS_DIALOG_FILTER;
                saveFileDialog1.Title = Resources.SAVE_CHARS_TITLE;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter strw = new StreamWriter(saveFileDialog1.FileName))
                        {
                            try
                            {
                                XmlSerializer s = new XmlSerializer(typeof(CyAlphabetTemplate));

                                CyAlphabetTemplate report = new CyAlphabetTemplate(m_currentCharsArray);

                                using (StringWriter sw = new StringWriter())
                                {
                                    s.Serialize(sw, report);
                                    string serializedXml = sw.ToString();
                                    strw.WriteLine(serializedXml);
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show(Resources.SAVE_TEMPLATE_ERROR_MSG,
                                                Resources.ERROR_MSG_TITLE,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Resources.WRITE_FILE_ERROR_MSG, Resources.ERROR_MSG_TITLE,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadCharsTemplate()
        {
            CyAlphabetTemplate result = null;
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = Resources.CHARS_DIALOG_FILTER;
                openFileDialog1.Title = Resources.OPEN_CHARS_TITLE;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                        {
                            try
                            {
                                string line, serializedstr = "";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    serializedstr += line;
                                }

                                XmlSerializer s = new XmlSerializer(typeof(CyAlphabetTemplate));
                                using (StringReader strreader = new StringReader(serializedstr))
                                {
                                    result = (CyAlphabetTemplate)s.Deserialize(strreader);
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show(Resources.LOAD_TEMPLATE_ERROR_MSG,
                                                Resources.ERROR_MSG_TITLE,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Resources.OPEN_FILE_ERROR_MSG, Resources.ERROR_MSG_TITLE,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (result != null)
            {
                m_defaultCharsArray = result.ConvertToArray2D();
                ChangeAlphabet();
                IsCharsArrayDefault = false;
            }
        }

        private void ChangeAlphabet()
        {
            cyCustomCharacter1.ResetAll();
            pictureBoxCharPreview.Image = null;
            m_changedCharsArray.Clear();
            CurrentCharsArray = (byte[,])m_defaultCharsArray.Clone();
            UpdateList();
            IsAnyCharChanged = false;
        }

        private static string Byte2DArrayToString(byte[,] arr)
        {
            StringBuilder res = new StringBuilder("{");
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                res.Append("{");
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    res.Append("0x" + arr[i, j].ToString("X2") + ", ");
                }
                res.Remove(res.Length - 2, 2);
                res.Append("}, ");
            }
            res.Remove(res.Length - 2, 2);
            res.Append("};");
            return res.ToString();
        }

        private static byte[,] StringToByte2DArray(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return null;
            }
            // Find dimension 1 in array
            int dim1 = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    dim1++;
                }
            }
            // Find dimension 2 in array
            int dim2 = 1;
            string tmp = str.Substring(2, str.IndexOf('}') - 2);
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] == ',')
                {
                    dim2++;
                }
            }
            if ((dim2 < 1) || (dim1 < 1))
            {
                return null;
            }

            byte[,] res = new byte[dim1, dim2];

            str = str.Replace("{", "");
            str = str.Replace("}", "");
            str = str.Replace(" ", "");
            str = str.Replace(";", "");
            string[] tmparr = str.Split(',');
            if (tmparr.Length == dim1 * dim2)
            {
                for (int i = 0; i < dim1; i++)
                    for (int j = 0; j < dim2; j++)
                    {
                        res[i, j] = Convert.ToByte(tmparr[i * dim2 + j].Replace("0x", ""), 16);
                    }
            }
            return res;
        }

        private static bool ArraysEqual<T>(T[,] a1, T[,] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if ((a1.GetLength(0) != a2.GetLength(0)) || (a1.GetLength(1) != a2.GetLength(1)))
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.GetLength(0); i++)
                for (int j = 0; j < a1.GetLength(1); j++)
                {
                    if (!comparer.Equals(a1[i, j], a2[i, j])) return false;
                }
            return true;
        }

        private Bitmap GetCharBitmap(int charIndex)
        {
            Bitmap bmp = new Bitmap(5, 8);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        if (((m_currentCharsArray[charIndex, i] >> j) & 1) > 0)
                        {
                            g.FillRectangle(Brushes.Black, i, j, 1, 1);
                        }
                    }
            }
            return bmp;
        }
        #endregion Private functions

        private void listBoxChars_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string text = string.Format("   Char {0}", listBoxChars.Items.IndexOf(listBoxChars.Items[e.Index]) + 1);
            e.DrawBackground();
            using (Brush myBrush = new SolidBrush(e.ForeColor))
            {
                using (Font fnt = new Font(e.Font, FontStyle.Bold))
                {
                    if (m_changedCharsArray.Contains(e.Index))
                        e.Graphics.DrawString(text, fnt, myBrush, e.Bounds, StringFormat.GenericDefault);
                    else
                        e.Graphics.DrawString(text, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                }

                e.Graphics.DrawImage(GetCharBitmap(e.Index), new Point(e.Bounds.Left, e.Bounds.Top + 2));
            }
            e.DrawFocusRectangle();
        }
    }

    #region Alphabet Template Classes

    [XmlRoot("Alphabet")]
    public class CyAlphabetTemplate
    {
        [XmlArray("Chars")]
        public List<CyAlphabetCharTemplate> m_charList;

        public CyAlphabetTemplate()
        {
            m_charList = new List<CyAlphabetCharTemplate>();
        }

        public CyAlphabetTemplate(byte[,] charsArray)
        {
            m_charList = new List<CyAlphabetCharTemplate>();
            for (int i = 0; i < charsArray.GetLength(0); i++)
            {
                byte[] tmpArray = new byte[charsArray.GetLength(1)];
                for (int j = 0; j < tmpArray.Length; j++)
                {
                    tmpArray[j] = charsArray[i, j];
                }
                m_charList.Add(new CyAlphabetCharTemplate(i, tmpArray));
            }
        }

        public byte[,] ConvertToArray2D()
        {
            byte[,] result = null;
            if (m_charList.Count > 0)
            {
                int x = m_charList.Count;
                int y = m_charList[0].Code.Length;
                result = new byte[x, y];
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                    {
                        result[i, j] = m_charList[i].Code[j];
                    }
            }
            return result;
        }
    }
    [XmlType("Char")]
    public class CyAlphabetCharTemplate
    {
        [XmlAttribute("Index")]
        public int m_charIndex;

        private byte[] m_code;

        [XmlAttribute("Code")]
        public string m_codeString;

        [XmlIgnore]
        public byte[] Code
        {
            get
            {
                string[] tmp = m_codeString.Split(',');
                m_code = new byte[tmp.Length];
                for (int i = 0; i < tmp.Length; i++)
                {
                    m_code[i] = Convert.ToByte(tmp[i].Replace("0x", "").Trim(' '), 16);
                }
                return m_code;
            }
            set
            {
                m_code = (byte[])value.Clone();
                m_codeString = "";
                for (int i = 0; i < m_code.Length; i++)
                {
                    m_codeString += "0x" + m_code[i].ToString("X2") + ", ";
                }
                m_codeString = m_codeString.TrimEnd(',', ' ');
            }
        }

        public CyAlphabetCharTemplate()
        {
            //m_Code = new byte();
        }
        public CyAlphabetCharTemplate(int index, byte[] code)
        {
            m_charIndex = index;
            Code = (byte[])code.Clone();
        }
    }

    #endregion Alphabet Template Classes

}
