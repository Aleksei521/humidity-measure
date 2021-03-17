/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SegLCD_P4_v1_31
{
    public partial class CyBasicConfiguration : CyEditingWrapperControl
    {
        #region Fields
        private bool m_updateComboboxLocked = true;
        private bool m_internalComboboxUpdate = false;
        #endregion Fields

        #region Constructors
        public CyBasicConfiguration()
        {
            InitializeComponent();
        }

        public CyBasicConfiguration(CyLCDParameters parameters)
            : base(parameters)
        {            
            InitializeComponent();

            errProvider.SetIconAlignment(labelNoClock, ErrorIconAlignment.MiddleLeft);

            m_parameters.m_cyBasicConfigurationTab = this;

            InitComboBoxes();
            LoadValuesFromParameters();
            ValidateCommonsSegmentNumber();
        }

        private void CyBasicConfiguration_Load(object sender, EventArgs e)
        {
            if (m_parameters.TermQuery != null)
            {
                m_parameters.UpdateClock(m_parameters.InstQuery, m_parameters.TermQuery);
            }
        }
        #endregion Constructors

        #region CyEditingWrapperControl overriden functions
        public override string TabName
        {
            get { return "General"; }
        }
        #endregion CyEditingWrapperControl overriden functions

        #region Initialization
        public void LoadValuesFromParameters()
        {
            numUpDownNumCommonLines.Value = m_parameters.NumCommonLines;
            numUpDownNumSegmentLines.Value = m_parameters.NumSegmentLines;            

            comboBoxBiasType.SelectedItem = m_parameters.BiasType;
            comboBoxDrivingMode.SelectedIndex = (byte)m_parameters.DrivingMode;
            comboBoxWaveformType.SelectedIndex = (byte)m_parameters.WaveformType;
            comboBoxLCDMode.SelectedIndex = (byte)m_parameters.LCDMode;
        }

        private void InitComboBoxes()
        {
            comboBoxDrivingMode.DataSource = Enum.GetValues(typeof(CyDrivingModeType));
            comboBoxDrivingMode.Format += comboBoxesType_Format;            

            comboBoxWaveformType.DataSource = Enum.GetValues(typeof(CyWaveformTypes));
            comboBoxWaveformType.Format += comboBoxesType_Format;

            comboBoxLCDMode.DataSource = Enum.GetValues(typeof(CyLCDModeType));
            comboBoxLCDMode.Format += comboBoxesType_Format;

            InitBiasTypeCombobox(m_parameters.BiasType);
            comboBoxBiasType.Format += comboBoxesType_Format;

            InitFrameRateCombobox();
            InitContrastCombobox();
            comboBoxesUpdate();
        }

        void comboBoxesType_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = CyLCDParameters.GetEnumDisplayName(e.ListItem);
        }

        void comboBoxesUpdate()
        {
            const int THRESHOLD_COMMONS_NUMBER = 8;

            if (m_parameters.NumCommonLines >= THRESHOLD_COMMONS_NUMBER)
            {
                comboBoxDrivingMode.SelectedIndex = (byte)CyDrivingModeType.PWM;
                comboBoxLCDMode.SelectedIndex = (byte)CyLCDModeType.HighSpeed;
            }

            comboBoxLCDMode.Enabled = (m_parameters.NumCommonLines < THRESHOLD_COMMONS_NUMBER);
            comboBoxDrivingMode.Enabled = (m_parameters.NumCommonLines < THRESHOLD_COMMONS_NUMBER);
        }

        private void InitBiasTypeCombobox(CyBiasTypes index)
        {
            if (m_parameters.LCDMode == CyLCDModeType.HighSpeed)
            {
                comboBoxBiasType.DataSource = Enum.GetValues(typeof(CyBiasTypes));
            }
            else
            {
                List<CyBiasTypes> biasList = new List<CyBiasTypes>();

                foreach (CyBiasTypes val in Enum.GetValues(typeof(CyBiasTypes)))
                {
                    if ((val == CyBiasTypes.Bias_14) || (val == CyBiasTypes.Bias_15))
                        continue;

                    biasList.Add(val);
                }

                comboBoxBiasType.DataSource = biasList;
            }

            if (comboBoxBiasType.Items.Contains(index))
            {
                comboBoxBiasType.SelectedIndex = (byte)index;
            }
        }

        private void InitFrameRateCombobox()
        {
            m_updateComboboxLocked = true;
            m_internalComboboxUpdate = true;
            string lastValue = m_parameters.FrameRate.ToString();

            if (m_parameters.ExternalClockHz <= 0)
            {
                comboBoxFrameRate.DataSource = null;
            }
            else
            {
                comboBoxFrameRate.DataSource = m_parameters.UpdateFrameRateRange();

                if (comboBoxFrameRate.Items.Count == 0)
                {
                    errProvider.SetError(comboBoxFrameRate, Resources.EMPTY_FRAMERATE_LIST);
                }
                else
                {
                    errProvider.SetError(comboBoxFrameRate, "");
                }
            }
            m_internalComboboxUpdate = false;

            if (comboBoxFrameRate.Items.Contains(lastValue))
                comboBoxFrameRate.Text = lastValue;
            else
                comboBoxFrameRate.SelectedIndex = comboBoxFrameRate.Items.Count - 1;

            m_updateComboboxLocked = false;
        }

        private void InitContrastCombobox()
        {
            m_updateComboboxLocked = true;
            m_internalComboboxUpdate = true;
            string lastValue = m_parameters.Contrast.ToString();

            if (m_parameters.ExternalClockHz <= 0)
            {
                comboBoxContrast.DataSource = null;
            }
            else
            {
                comboBoxContrast.DataSource = m_parameters.UpdateContrastRange();
            }
            m_internalComboboxUpdate = false;

            if (comboBoxContrast.Items.Contains(lastValue))
                comboBoxContrast.Text = lastValue;
            else
                comboBoxContrast.SelectedIndex = comboBoxContrast.Items.Count - 1;

            m_updateComboboxLocked = false;
        }

        #endregion Initialization

        #region Event handlers
        private void numUpDownNumLines_ValueChanged(object sender, EventArgs e)
        {
            bool isCommonChanged = sender == numUpDownNumCommonLines;
            NumericUpDown numUpDown = (NumericUpDown)sender;
            byte paramValue = isCommonChanged ? m_parameters.NumCommonLines : m_parameters.NumSegmentLines;            

            if (isCommonChanged)
            {
                numUpDownNumSegmentLines.Maximum = m_parameters.MaxPinsNumber() - (int)numUpDownNumCommonLines.Value;
            }

            bool updateHelpers = numUpDown.Value != paramValue;

            if (paramValue != (byte)numUpDown.Value)
            {
                if (isCommonChanged)
                {
                    m_parameters.NumCommonLines = (byte)numUpDown.Value;
                    comboBoxesUpdate();
                    InitFrameRateCombobox();
                    InitContrastCombobox();
                }
                else
                    m_parameters.NumSegmentLines = (byte)numUpDown.Value;
            }

            //Update Empty helper
            if (updateHelpers)
            {
                m_parameters.m_cyHelpersTab.ComSegLinesNumChanged();
            }
            
            ValidateCommonsSegmentNumber();
        }

        private void comboBoxDrivingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_parameters.DrivingMode = (CyDrivingModeType)comboBoxDrivingMode.SelectedIndex;

            comboBoxBiasType.Enabled = (m_parameters.DrivingMode == CyDrivingModeType.PWM);
        }

        private void comboBoxWaveformType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_parameters.WaveformType = (CyWaveformTypes)comboBoxWaveformType.SelectedIndex;
        }

        private void comboBoxBiasType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_parameters.BiasType = (CyBiasTypes)comboBoxBiasType.SelectedIndex;
        }

        private void comboBoxLCDMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_parameters.LCDMode != (CyLCDModeType)comboBoxLCDMode.SelectedIndex)
            {
                m_parameters.LCDMode = (CyLCDModeType)comboBoxLCDMode.SelectedIndex;
                m_parameters.m_desiredLCDMode = m_parameters.LCDMode;

                InitBiasTypeCombobox(m_parameters.BiasType);
                ClockUpdated();
            }
        }

        private void comboBoxFrameRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFrameRate.SelectedIndex < 0)
                return;

            if (m_internalComboboxUpdate == false)
                m_parameters.FrameRate = Convert.ToByte(comboBoxFrameRate.Text);

            if (m_updateComboboxLocked == false)
            {
                InitContrastCombobox();
            }
        }

        private void comboBoxContrast_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxContrast.SelectedIndex < 0)
                return;

            if (m_internalComboboxUpdate == false)
                m_parameters.Contrast = Convert.ToByte(comboBoxContrast.Text);

            if (m_updateComboboxLocked == false)
            {
                InitFrameRateCombobox();
            }
        }

        public void ClockUpdated()
        {            
            InitFrameRateCombobox();
            InitContrastCombobox();

            labelNoClock.Visible = (m_parameters.LCDMode == CyLCDModeType.HighSpeed) && (m_parameters.ExternalClockHz <= 0);
            errProvider.SetError(labelNoClock, labelNoClock.Visible ? " ": "");
        }

        private void ValidateCommonsSegmentNumber()
        {
            int maxSegmentsNumber = m_parameters.MaxPinsNumber() - (int)numUpDownNumCommonLines.Value;

            if (m_parameters.NumCommonLines > m_parameters.MaxCommonsNumber())
            {
                errProvider.SetError(numUpDownNumCommonLines, Resources.NUMBER_OF_COMMONS_ERROR);
            }
            else
            {
                errProvider.SetError(numUpDownNumCommonLines, String.Empty);
            }

            if (m_parameters.NumSegmentLines > maxSegmentsNumber)
            {
                errProvider.SetError(numUpDownNumSegmentLines, Resources.NUMBER_OF_COMMONS_ERROR);
            }
            else
            {
                errProvider.SetError(numUpDownNumSegmentLines, String.Empty); 
            }

            //Set Max limits for Commons and Segments
            numUpDownNumCommonLines.Maximum = Math.Max(m_parameters.MaxCommonsNumber(), m_parameters.NumCommonLines);
            numUpDownNumSegmentLines.Maximum = Math.Max(maxSegmentsNumber, m_parameters.NumSegmentLines);
        }
        #endregion Event handlers
    }
}
