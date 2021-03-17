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
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using CyDesigner.Extensions.Common;
using CyDesigner.Extensions.Gde;

namespace SegLCD_P4_v1_31
{
    #region Parameters range constants
    public static class CyParamRange
    {
        public const int FRAME_RATE_MIN = 30;
        public const int FRAME_RATE_MAX = 150;
        public const int CONTRAST_MIN = 10;
        public const int CONTRAST_MAX = 100;
    }
    #endregion

    #region Enums
    public enum CyDrivingModeType
    {
        [Description("PWM")]
        PWM,
        [Description("Digital correlation")]
        DigitalCorrelation
    };
    public enum CyBiasTypes
    {
        [Description("1/2 Bias")]
        Bias_12,
        [Description("1/3 Bias")]
        Bias_13,
        [Description("1/4 Bias")]
        Bias_14,
        [Description("1/5 Bias")]
        Bias_15
    };
    public enum CyLCDModeType
    {
        [Description("Low speed")]
        LowSpeed,
        [Description("High speed")]
        HighSpeed
    };
    public enum CyWaveformTypes
    {
        [Description("Type A")]
        TypeA,
        [Description("Type B")]
        TypeB
    };
    #endregion Enums

    public class CyLCDParameters
    {
        #region Const
        // Parameters
        public const string PARAM_NUMCOMMONLINES = "NumCommonLines";
        public const string PARAM_NUMSEGMENTLINES = "NumSegmentLines";
        public const string PARAM_DRIVINGMODE = "DrivingMode";
        public const string PARAM_BIASTYPE = "BiasType";
        public const string PARAM_WAVEFORMTYPE = "WaveformType";
        public const string PARAM_FRAMERATE = "FrameRate";
        public const string PARAM_CONTRAST = "Contrast";
        public const string PARAM_LCDMODE = "LCDMode";
        public const string PARAM_HELPERS = "Helpers";
        public const string PARAM_CUSTOMCHARSLIST = "CustomCharsList";
        public const string PARAM_EXTERNALCLOCK = "ExternalClock";

        public const string TERM_CLK_NAME = "hs_clk";
        public const int LS_CLOCK_FREQ = 32000;
        public const double CLK_EPS = 0.0001;

        public const string UNASSIGNED_PIXEL_NAME = "PIX";

        public const string LCD_FEATURE_NAME = "m0s8lcd";
        public const string MAX_LCD_COMMONS_PARAMETER = "COM_NR";
        public const string MAX_LCD_PINS_PARAMETER = "PIN_NR";
        public const string PORTS_NUMBER_4_COMS_PARAMETER = "NUMPORTS";
        public const string PORTS_NUMBER_8_COMS_PARAMETER = "NUMPORTS8";
        public const string PORTS_NUMBER_16_COMS_PARAMETER = "NUMPORTS16";

        public const int PSOC_4A_LCD_IP_BLOCK_VER = 0;
        public const int PSOC_4A_LCD_COMMONS_NUMBER = 4;
        public const int PSOC_4A_LCD_PINS_NUMBER = 36;
        public const int PSOC_4A_LCD_4_COMS_PORTS_NUMBER = 5;
        public const int PSOC_4A_LCD_8_COMS_PORTS_NUMBER = 0;
        public const int PSOC_4A_LCD_16_COMS_PORTS_NUMBER = 0;

        #endregion Const

        #region Variables
        private readonly ICyInstQuery_v1 m_instQuery;
        private readonly ICyInstEdit_v1 m_instEdit;
        private readonly ICyTerminalQuery_v1 m_termQuery;

        public List<CyHelperInfo> m_helpersConfig;
        private String m_serializedHelpers;

        public CyColorList m_colorChooser = new CyColorList();
        public CyLCDModeType m_desiredLCDMode;

        public List<int> m_symbolIndexes_7SEG = new List<int>();
        public List<int> m_symbolIndexes_14SEG = new List<int>();
        public List<int> m_symbolIndexes_16SEG = new List<int>();
        public List<int> m_symbolIndexes_BAR = new List<int>();
        public List<int> m_symbolIndexes_MATRIX = new List<int>();

        public List<int> m_helperIndexes_7SEG = new List<int>();
        public List<int> m_helperIndexes_14SEG = new List<int>();
        public List<int> m_helperIndexes_16SEG = new List<int>();
        public List<int> m_helperIndexes_BAR = new List<int>();
        public List<int> m_helperIndexes_MATRIX = new List<int>();

        private bool m_globalEditMode;
        private double m_externalClockHz;

        //Tabs
        public CyBasicConfiguration m_cyBasicConfigurationTab;
        public CyHelpers m_cyHelpersTab;

        public XmlSerializer m_serializer;
        public XmlSerializerNamespaces m_customSerNamespace;
        #endregion Variables

        #region Properties
        public ICyInstQuery_v1 InstQuery
        {
            get { return m_instQuery; }
        }

        public ICyTerminalQuery_v1 TermQuery
        {
            get { return m_termQuery; }
        }

        public bool GlobalEditMode
        {
            get { return m_globalEditMode; }
            set { m_globalEditMode = value; }
        }

        public byte NumCommonLines
        {
            get { return GetValue<byte>(PARAM_NUMCOMMONLINES); }
            set
            {
                SetValue(PARAM_NUMCOMMONLINES, value);
            }
        }

        public byte NumSegmentLines
        {
            get { return GetValue<byte>(PARAM_NUMSEGMENTLINES); }
            set { SetValue(PARAM_NUMSEGMENTLINES, value); }
        }

        public CyDrivingModeType DrivingMode
        {
            get { return GetValue<CyDrivingModeType>(PARAM_DRIVINGMODE); }
            set { SetValue(PARAM_DRIVINGMODE, value); }
        }

        public CyBiasTypes BiasType
        {
            get { return GetValue<CyBiasTypes>(PARAM_BIASTYPE); }
            set { SetValue(PARAM_BIASTYPE, value); }
        }

        public CyWaveformTypes WaveformType
        {
            get { return GetValue<CyWaveformTypes>(PARAM_WAVEFORMTYPE); }
            set { SetValue(PARAM_WAVEFORMTYPE, value); }
        }

        public byte FrameRate
        {
            get { return GetValue<byte>(PARAM_FRAMERATE); }
            set { SetValue(PARAM_FRAMERATE, value); }
        }

        public byte Contrast
        {
            get { return GetValue<byte>(PARAM_CONTRAST); }
            set { SetValue(PARAM_CONTRAST, value); }
        }

        public CyLCDModeType LCDMode
        {
            get { return GetValue<CyLCDModeType>(PARAM_LCDMODE); }
            set { SetValue(PARAM_LCDMODE, value); }
        }

        public string SerializedHelpers
        {
            get { return GetValue<string>(PARAM_HELPERS); }
            set
            {
                if (value.Replace("\r\n", " ") != m_serializedHelpers)
                {
                    m_serializedHelpers = value;
                    m_serializedHelpers = m_serializedHelpers.Replace("\r\n", " ");
                    SetValue(PARAM_HELPERS, m_serializedHelpers);
                }
            }
        }

        public double ExternalClockHz
        {
            get { return ((m_desiredLCDMode == CyLCDModeType.LowSpeed) ? LS_CLOCK_FREQ : m_externalClockHz); }
            set { m_externalClockHz = value; }
        }

        public double ExternalClockHzParam
        {
            get { return GetValue<double>(PARAM_EXTERNALCLOCK); }
            set { SetValue(PARAM_EXTERNALCLOCK, Math.Max(0, value)); }
        }

        public string CustomCharsList
        {
            get { return GetValue<string>(PARAM_CUSTOMCHARSLIST); }
            set { SetValue(PARAM_CUSTOMCHARSLIST, value); }
        }

        public bool IsPSoC4A
        {
            get { return (InstQuery.DeviceQuery.GetFeatureVersion(LCD_FEATURE_NAME) == PSOC_4A_LCD_IP_BLOCK_VER); }
        }
        #endregion Properties

        #region Common
        private CyLCDParameters(ICyInstQuery_v1 instQuery, ICyInstEdit_v1 instEdit, ICyTerminalQuery_v1 termQuery)
        {
            m_instQuery = instQuery;
            m_instEdit = instEdit;
            m_termQuery = termQuery;

            GetParams();

            // Create XML Serializer
            Type[] theExtraTypes = new Type[2];
            theExtraTypes[0] = typeof(CyHelperInfo);
            theExtraTypes[1] = typeof(ArrayList);
            m_serializer = new XmlSerializer(typeof(ArrayList), theExtraTypes);
            m_customSerNamespace = new XmlSerializerNamespaces();
            string curNamespace = Assembly.GetExecutingAssembly().FullName;
            string version = curNamespace.Substring(curNamespace.LastIndexOf("_v") + 1);
            m_customSerNamespace.Add("CustomizerVersion", version);
            m_desiredLCDMode = LCDMode;
        }

        public CyLCDParameters(ICyInstQuery_v1 instQuery)
            : this(instQuery, null, null)
        {
        }

        public CyLCDParameters(ICyInstEdit_v1 instEdit, ICyTerminalQuery_v1 termQuery)
            : this(instEdit, instEdit, termQuery)
        {
        }

        private void GetParams()
        {
            if (m_instQuery != null)
            {
                DeserializeHelpers(SerializedHelpers);
            }
            else
            {
                // This method is never called when m_instQuery is not specified
                Debug.Assert(false);
            }
        }

        public void GetExprViewParams()
        {
            if (m_instQuery != null)
            {
                m_cyBasicConfigurationTab.LoadValuesFromParameters();
            }
            else
            {
                // This method is never called when m_instQuery is not specified
                Debug.Assert(false);
            }
        }

        public static T GetValue<T>(string paramName, ICyInstQuery_v1 instQuery)
        {
            if (instQuery == null) return default(T);
            T value;
            CyCustErr err = instQuery.GetCommittedParam(paramName).TryGetValueAs<T>(out value);
            return (err != null && err.IsOK) ? value : default(T);
        }

        private T GetValue<T>(string paramName)
        {
            return GetValue<T>(paramName, m_instQuery);
        }

        public static CyCustErr SetValue<T>(ICyInstEdit_v1 instEdit, string paramName, T value)
        {
            if (instEdit != null)
            {
                string valueToSet = (value == null) ? String.Empty : value.ToString();
                if (value is bool)
                {
                    valueToSet = valueToSet.ToLower();
                }
                if (value is byte || value is UInt16 || value is UInt32 || value is UInt64)
                {
                    valueToSet += "u";
                }
                if (instEdit.SetParamExpr(paramName, valueToSet))
                {
                    instEdit.CommitParamExprs();
                }
            }

            CyCompDevParam param = instEdit.GetCommittedParam(paramName);
            return param.ErrorCount > 0 ? new CyCustErr(param.ErrorMsgs) : CyCustErr.OK;
        }

        private void SetValue<T>(string paramName, T value)
        {
            if (m_globalEditMode)
            {
                SetValue(m_instEdit, paramName, value);
            }
        }

        public void CommitHelpers()
        {
            SerializedHelpers = CyHelperInfo.SerializeHelpers(m_helpersConfig, m_serializer, m_customSerNamespace);
        }

        public static string GetEnumDisplayName(object value)
        {
            if (value == null)
            {
                Debug.Fail("Null reference argument passed into GetEnumDisplayName() method.");
                return String.Empty;
            }

            if (value is CyDrivingModeType)
            {
                switch ((CyDrivingModeType)value)
                {
                    case CyDrivingModeType.PWM:
                        return Resources.EnumDrivingModePWM;
                    case CyDrivingModeType.DigitalCorrelation:
                        return Resources.EnumDrivingModeDigitalCorrelation;
                    default:
                        Debug.Fail("Unhandled value of CyDrivingModeType enum.");
                        return String.Empty;
                }
            }

            if (value is CyBiasTypes)
            {
                switch ((CyBiasTypes)value)
                {
                    case CyBiasTypes.Bias_12:
                        return Resources.EnumBias_12;
                    case CyBiasTypes.Bias_13:
                        return Resources.EnumBias_13;
                    case CyBiasTypes.Bias_14:
                        return Resources.EnumBias_14;
                    case CyBiasTypes.Bias_15:
                        return Resources.EnumBias_15;
                    default:
                        Debug.Fail("Unhandled value of CyBiasTypes enum.");
                        return String.Empty;
                }
            }

            if (value is CyWaveformTypes)
            {
                switch ((CyWaveformTypes)value)
                {
                    case CyWaveformTypes.TypeA:
                        return Resources.EnumWaveformTypeA;
                    case CyWaveformTypes.TypeB:
                        return Resources.EnumWaveformTypeB;
                    default:
                        Debug.Fail("Unhandled value of CyWaveformTypes enum.");
                        return String.Empty;
                }
            }

            if (value is CyLCDModeType)
            {
                switch ((CyLCDModeType)value)
                {
                    case CyLCDModeType.LowSpeed:
                        return Resources.EnumLCDModeLowSpeed;
                    case CyLCDModeType.HighSpeed:
                        return Resources.EnumLCDModeHighSpeed;
                    default:
                        Debug.Fail("Unhandled value of CyLCDModeType enum.");
                        return String.Empty;
                }
            }

            Debug.Fail("Unknown argument of GetEnumDisplayName() method.");
            return String.Empty;
        }
        #endregion Common

        #region Calculations
        private double GetRefrPeriod(byte frameRate)
        {
            return (m_desiredLCDMode == CyLCDModeType.LowSpeed) ?
                (LS_CLOCK_FREQ / frameRate) : (ExternalClockHzParam / frameRate);
        }

        public UInt32 GetSubfrDiv(byte frameRate, byte contrast, byte numCommonLines)
        {
            double subfrDiv = ((GetRefrPeriod(frameRate) / (4 * numCommonLines) - 1) *
                               ((double)contrast / (double)CyParamRange.CONTRAST_MAX));
            return (UInt32)Math.Round(subfrDiv);
        }

        public UInt32 GetDeadDiv(byte frameRate, byte contrast, byte numCommonLines)
        {
            double deadDiv = (GetRefrPeriod(frameRate) * ((1 - (double)contrast / (double)CyParamRange.CONTRAST_MAX)));
            return (UInt32)Math.Round(deadDiv);
        }

        public string[] UpdateFrameRateRange()
        {
            List<string> frameRateList = new List<string>();
            UInt16 divMax = (m_desiredLCDMode == CyLCDModeType.LowSpeed) ? byte.MaxValue : UInt16.MaxValue;
            for (byte i = CyParamRange.FRAME_RATE_MIN; i <= CyParamRange.FRAME_RATE_MAX; i += 10)
            {
                if (GetSubfrDiv(i, Contrast, NumCommonLines) <= divMax)
                {
                    if (frameRateList.Contains(i.ToString()) == false)
                        frameRateList.Add(i.ToString());
                }
            }
            return frameRateList.ToArray();
        }
        public string[] UpdateContrastRange()
        {
            List<string> contrastList = new List<string>();
            UInt16 divMax = (m_desiredLCDMode == CyLCDModeType.LowSpeed) ? byte.MaxValue : UInt16.MaxValue;

            for (byte j = CyParamRange.CONTRAST_MIN; j < CyParamRange.CONTRAST_MAX; j += 10)
            {
                if (GetDeadDiv(FrameRate, j, NumCommonLines) <= divMax)
                {
                    if (contrastList.Contains(j.ToString()) == false)
                        contrastList.Add(j.ToString());
                }
            }

            contrastList.Add(CyParamRange.CONTRAST_MAX.ToString());

            return contrastList.ToArray();
        }
        #endregion

        /// <summary>
        /// Deserialize the list of Helper functions from the string stored in Parameters
        /// </summary>
        /// <param name="_sHelpers"> XML string representation of the Helpers list</param>
        public void DeserializeHelpers(string _sHelpers)
        {
            try
            {
                if (!string.IsNullOrEmpty(_sHelpers))
                {
                    m_helpersConfig = CyHelperInfo.DeserializeHelpers(_sHelpers);
                    // Add used helper and symbol indexes to the list
                    for (int i = 0; i < m_helpersConfig.Count; i++)
                    {
                        switch (m_helpersConfig[i].Kind)
                        {
                            case CyHelperKind.SEGMENT_7:
                                m_helperIndexes_7SEG.Add(m_helpersConfig[i].m_globalHelperIndex);
                                for (int j = 0; j < m_helpersConfig[i].m_helpSegInfo.Count; j++)
                                {
                                    if (!m_symbolIndexes_7SEG.Contains(
                                        m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum))
                                    {
                                        m_symbolIndexes_7SEG.Add(m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum);
                                    }
                                }
                                break;
                            case CyHelperKind.SEGMENT_14:
                                m_helperIndexes_14SEG.Add(m_helpersConfig[i].m_globalHelperIndex);
                                for (int j = 0; j < m_helpersConfig[i].m_helpSegInfo.Count; j++)
                                {
                                    if (!m_symbolIndexes_14SEG.Contains(
                                        m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum))
                                    {
                                        m_symbolIndexes_14SEG.Add(m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum);
                                    }
                                }
                                break;
                            case CyHelperKind.SEGMENT_16:
                                m_helperIndexes_16SEG.Add(m_helpersConfig[i].m_globalHelperIndex);
                                for (int j = 0; j < m_helpersConfig[i].m_helpSegInfo.Count; j++)
                                {
                                    if (
                                        !m_symbolIndexes_16SEG.Contains(
                                        m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum))
                                    {
                                        m_symbolIndexes_16SEG.Add(m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum);
                                    }
                                }
                                break;
                            case CyHelperKind.BAR:
                                m_helperIndexes_BAR.Add(m_helpersConfig[i].m_globalHelperIndex);
                                for (int j = 0; j < m_helpersConfig[i].m_helpSegInfo.Count; j++)
                                {
                                    if (!m_symbolIndexes_BAR.Contains(
                                                                  m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum))
                                        m_symbolIndexes_BAR.Add(m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum);
                                }
                                break;
                            case CyHelperKind.MATRIX:
                                m_helperIndexes_MATRIX.Add(m_helpersConfig[i].m_globalHelperIndex);
                                for (int j = 0; j < m_helpersConfig[i].m_helpSegInfo.Count; j++)
                                {
                                    if (!m_symbolIndexes_MATRIX.Contains(
                                       m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum))
                                        m_symbolIndexes_MATRIX.Add(m_helpersConfig[i].m_helpSegInfo[j].m_globalDigitNum);
                                }
                                break;
                            case CyHelperKind.EMPTY:
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    m_helpersConfig = new List<CyHelperInfo>();
                    CyHelperInfo.CreateHelper(CyHelperKind.EMPTY, this);
                }
            }
            catch
            {
                MessageBox.Show(Resources.PARAMETERS_LOADING_ERROR_MSG, Resources.ERROR_MSG_TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetEnumDescription(Type enumType, string value)
        {
            FieldInfo fi = enumType.GetField(value);
            DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value;
        }

        public static List<string> GetEnumValuesDescription(Type enumType)
        {
            List<string> descList = new List<string>();
            List<string> enumValues = new List<string>(Enum.GetNames(enumType));
            for (int i = 0; i < enumValues.Count; i++)
            {
                descList.Add(GetEnumDescription(enumType, enumValues[i]));
            }
            return descList;
        }

        #region Clock
        // Returns external clock value in Hz
        public double GetExternalClockInHz(ICyTerminalQuery_v1 termQuery, string clockName)
        {
            List<CyClockData> clkdata = termQuery.GetClockData(clockName, 0);

            if ((clkdata.Count > 0) && clkdata[0].IsFrequencyKnown)
            {
                return clkdata[0].Frequency * Math.Pow(10, clkdata[0].UnitAsExponent);
            }
            return (-1);
        }

        public void UpdateClock(ICyInstQuery_v1 edit, ICyTerminalQuery_v1 termQuery)
        {
            UpdateClock(edit, termQuery, true);
        }
        public void UpdateClock(ICyInstQuery_v1 edit, ICyTerminalQuery_v1 termQuery, bool updateForm)
        {
            double extClockNew = GetExternalClockInHz(termQuery, TERM_CLK_NAME);
            if (Math.Abs(extClockNew - ExternalClockHz) < CLK_EPS)
            {
                return;
            }

            ExternalClockHz = extClockNew;

            if (extClockNew > 0)
            {
                ExternalClockHzParam = extClockNew;
            }

            if (updateForm && (m_cyBasicConfigurationTab != null))
            {
                m_cyBasicConfigurationTab.ClockUpdated();
            }
        }
        #endregion

        #region m0s8lcd parameters
        public int MaxCommonsNumber()
        {
            Debug.Assert(InstQuery != null);
            if (IsPSoC4A)
            {
                return PSOC_4A_LCD_COMMONS_NUMBER;
            }
            else
            {
                return (InstQuery.DeviceQuery.GetFeatureParameter(LCD_FEATURE_NAME, MAX_LCD_COMMONS_PARAMETER));
            }
        }
        public int MaxPinsNumber()
        {
            Debug.Assert(InstQuery != null);
            if (IsPSoC4A)
            {
                return PSOC_4A_LCD_PINS_NUMBER;
            }
            else
            {
                return (InstQuery.DeviceQuery.GetFeatureParameter(LCD_FEATURE_NAME, MAX_LCD_PINS_PARAMETER));
            }
        }
        public int PortsNumberFor_4_Coms()
        {
            Debug.Assert(InstQuery != null);
            if (IsPSoC4A)
            {
                return PSOC_4A_LCD_4_COMS_PORTS_NUMBER;
            }
            else
            {
                return (InstQuery.DeviceQuery.GetFeatureParameter(LCD_FEATURE_NAME, PORTS_NUMBER_4_COMS_PARAMETER));
            }
        }
        public int PortsNumberFor_8_Coms()
        {
            Debug.Assert(InstQuery != null);
            if (IsPSoC4A)
            {
                return PSOC_4A_LCD_8_COMS_PORTS_NUMBER;
            }
            else
            {
                return (InstQuery.DeviceQuery.GetFeatureParameter(LCD_FEATURE_NAME, PORTS_NUMBER_8_COMS_PARAMETER));
            }
        }
        public int PortsNumberFor_16_Coms()
        {
            Debug.Assert(InstQuery != null);
            if (IsPSoC4A)
            {
                return PSOC_4A_LCD_16_COMS_PORTS_NUMBER;
            }
            else
            {
                return (InstQuery.DeviceQuery.GetFeatureParameter(LCD_FEATURE_NAME, PORTS_NUMBER_16_COMS_PARAMETER));
            }
        }
        #endregion
    }
}
