/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using CyDesigner.Extensions.Gde;


namespace SegLCD_P4_v1_31
{
    partial class CyAPIGenerator
    {
        CyLCDParameters m_parameters;

        const int MAX_LINE_LEN = 118;

        private readonly string m_instanceName;

        //------------------------------------------------------------------------------------------------------------

        #region Constructor(s)

        public CyAPIGenerator(CyLCDParameters parameters, string instanceName)
        {
            m_parameters = parameters;
            m_instanceName = instanceName;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------

        #region Header Data
        public void CollectApiHeader(ICyAPICustomizeArgs_v2 args, List<CyHelperInfo> helpersConfig,
                                    ref Dictionary<string, string> paramDict, int pNumCommonLines, int pNumSegmentLines)
        {
            int index = 0;
            using (TextWriter writerCVariables = new StringWriter(), writerHPixelDef = new StringWriter(), writerHRegistersDef = new StringWriter())
            {
                // If there is segment7 helper, generate define `$INSTANCE_NAME`_7SEG_BLANK_DIG
                for (int i = 0; i < helpersConfig.Count; i++)
                {
                    if (helpersConfig[i].Kind == CyHelperKind.SEGMENT_7)
                    {
                        writerHPixelDef.WriteLine("/* Defines index of \"blank\" digit in the look-up table for 7 segment helper */");
                        writerHPixelDef.WriteLine(GenerateDefine("7SEG_BLANK_DIG", "(0x10u)"));
                        writerHPixelDef.WriteLine();
                        break;
                    }
                }

                for (int i = 0; i < helpersConfig.Count; i++)
                {
                    CyHelperInfo hi = helpersConfig[i];
                    if (hi.Kind != CyHelperKind.EMPTY)
                    {
                        writerHPixelDef.WriteLine(WriteDigitNumVar(hi.SymbolsCount, index));
                        index++;
                    }
                }
                if (String.IsNullOrEmpty(writerHPixelDef.ToString()) == false)
                {
                    writerHPixelDef.WriteLine();
                }

                // Array of commons
                StringBuilder commons = new StringBuilder();
                for (int i = 0; i < pNumCommonLines; i++)
                {
                    commons.AppendFormat("{0}_Com{1}, ", m_instanceName, i);
                }
                commons = new StringBuilder(commons.ToString().TrimEnd(' ', ','));
                commons = new StringBuilder(String.Format("static uint32 {0}_commons[] = {{{1}}};", m_instanceName, commons));
                writerCVariables.WriteLine(StringLineBreaking(commons.ToString(), 0));

                //Common Generation Sample
                for (int i = 0; i < pNumCommonLines; i++)
                {
                    int comIndex = i;
                    writerHPixelDef.WriteLine(GenerateDefine(String.Format("COM{0}_PORT", comIndex),
                        String.Format("({0}_bSeg_LCD__COMMON_{1} / {0}_PINS_PER_PORT)", m_instanceName, comIndex)));
                    writerHPixelDef.WriteLine(GenerateDefine(String.Format("COM{0}_PIN", comIndex),
                        String.Format("({0}_bSeg_LCD__COMMON_{1} % {0}_PINS_PER_PORT)", m_instanceName, comIndex)));
                }
                writerHPixelDef.WriteLine("");

                for (int i = 0; i < pNumCommonLines; i++)
                {
                    int comIndex = i;
                    writerHPixelDef.WriteLine(GenerateDefine(String.Format("Com{0}", comIndex),
                        String.Format("{0}_FIND_PIXEL({0}_COM{1}_PORT,  {0}_COM{1}_PIN,  {1}u)",  m_instanceName, comIndex)));
                }
                writerHPixelDef.WriteLine("");

                //Segment Generation Sample
                for (int i = 0; i < pNumSegmentLines; i++)
                {
                    int segIndex = i;
                    writerHPixelDef.WriteLine(GenerateDefine(String.Format("SEG{0}_PORT", segIndex),
                        String.Format("({0}_bSeg_LCD__SEGMENT_{1} / {0}_PINS_PER_PORT)", m_instanceName, segIndex)));
                    writerHPixelDef.WriteLine(GenerateDefine(String.Format("SEG{0}_PIN", segIndex),
                        String.Format("({0}_bSeg_LCD__SEGMENT_{1} % {0}_PINS_PER_PORT)", m_instanceName, segIndex)));
                }
                writerHPixelDef.WriteLine("");

                List<Point> usedComSeg = new List<Point>();
                for (int i = 0; i < helpersConfig.Count; i++)
                {
                    CyHelperInfo hi = helpersConfig[i];
                    if (hi.Kind == CyHelperKind.EMPTY) continue;

                    for (int j = 0; j < hi.m_helpSegInfo.Count; j++)
                    {
                        CyHelperSegmentInfo si = hi.m_helpSegInfo[j];

                        // Define only assigned pixels
                        if ((si.Common >= 0) && (si.Segment >= 0))
                        {
                            writerHPixelDef.WriteLine(GenerateDefine(si.Name,
                                String.Format("{0}_FIND_PIXEL({0}_SEG{1}_PORT, {0}_SEG{1}_PIN, {2}u)", m_instanceName, si.Segment, si.Common)));
                            usedComSeg.Add(new Point(si.Common, si.Segment));
                        }
                    }
                }
                writerHPixelDef.WriteLine("");
                //Add pixels of Empty helper that weren't added before
                for (int i = 0; i < helpersConfig[0].m_helpSegInfo.Count; i++)
                {
                    CyHelperSegmentInfo si = helpersConfig[0].m_helpSegInfo[i];

                    if (!usedComSeg.Contains(new Point(si.Common, si.Segment)))
                    {
                        writerHPixelDef.WriteLine(GenerateDefine(si.Name,
                                String.Format("{0}_FIND_PIXEL({0}_SEG{1}_PORT,  {0}_SEG{1}_PIN,  {2}u)", m_instanceName, si.Segment, si.Common)));
                    }
                }

                for (int i = 0; i < m_parameters.PortsNumberFor_4_Coms(); i++)
                {
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA{0}_REG                (*(reg32*) CYREG_LCD_DATA0{0})", i));
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA{0}_PTR                ((reg32*) CYREG_LCD_DATA0{0})", i));
                }
                for (int i = 0; i < m_parameters.PortsNumberFor_8_Coms(); i++)
                {
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA1{0}_REG               (*(reg32*) CYREG_LCD_DATA1{0})", i));
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA1{0}_PTR               ((reg32*) CYREG_LCD_DATA1{0})", i));
                }
                for (int i = 0; i < m_parameters.PortsNumberFor_16_Coms(); i++)
                {
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA2{0}_REG               (*(reg32*) CYREG_LCD_DATA2{0})", i));
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA2{0}_PTR               ((reg32*) CYREG_LCD_DATA2{0})", i));
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA3{0}_REG               (*(reg32*) CYREG_LCD_DATA3{0})", i));
                    writerHRegistersDef.WriteLine(String.Format("#define " + m_instanceName +
                        "_DATA3{0}_PTR               ((reg32*) CYREG_LCD_DATA3{0})", i));
                }

                paramDict.Add("writerCVariables", writerCVariables.ToString().TrimEnd('\r', '\n'));
                paramDict.Add("writerHPixelDef", writerHPixelDef.ToString().TrimEnd('\r', '\n'));
                paramDict.Add("writerHRegistersDef", writerHRegistersDef.ToString().TrimEnd('\r', '\n'));
            }
            SetCharDotMatrixParam(paramDict);
        }

        private void SetCharDotMatrixParam(Dictionary<string, string> paramDict)
        {
            using (TextWriter writerCharDotMatrix = new StringWriter())
            {
                string charListParam;
                paramDict.TryGetValue(CyLCDParameters.PARAM_CUSTOMCHARSLIST, out charListParam);

                for (int i = 0; i < charListParam.Length; i++)
                {
                    int index = charListParam.IndexOf("0x", i);
                    if (index >= 0)
                    {
                        charListParam = charListParam.Insert(index + 4, "u");
                        i = index + 4;
                    }
                }

                StringBuilder dotMatrixParam =
                    new StringBuilder(string.Format("static const uint8 {0}_charDotMatrix[][{0}_DM_CHAR_WIDTH] = ", m_instanceName));
                dotMatrixParam.Append(charListParam);
                writerCharDotMatrix.WriteLine(String2DArrayLineBreaking(dotMatrixParam.ToString(), 2));

                paramDict.Add("charDotMatrix", writerCharDotMatrix.ToString().TrimEnd('\r', '\n'));
            }
        }

        public void CollectDivParams(ref Dictionary<string, string> paramDict, CyLCDParameters param)
        {
            string subfrDiv_u = ToHex2BStr((ushort)param.GetSubfrDiv(param.FrameRate, param.Contrast,
                                param.NumCommonLines));
            string deadDiv_u = ToHex2BStr((ushort)param.GetDeadDiv(param.FrameRate, param.Contrast,
                                param.NumCommonLines));
            paramDict.Add("SubfrDiv", subfrDiv_u);
            paramDict.Add("DeadDiv", deadDiv_u);
        }

        public void CollectContrastDefines(ref Dictionary<string, string> paramDict, ICyTerminalQuery_v1 termQuery,
            CyLCDParameters param)
        {
            StringBuilder contrastDefines = new StringBuilder();

            StringBuilder deadArray = new StringBuilder();
            StringBuilder subfrArray = new StringBuilder();

            UInt16 subfrDiv_u;

            for (int j = 0; j < 2; j++)
            {
                deadArray = new StringBuilder();
                subfrArray = new StringBuilder();

                param.m_desiredLCDMode = (j == 0) ? CyLCDModeType.LowSpeed :
                                                    CyLCDModeType.HighSpeed;
                List<string> contrastList = new List<string>(param.UpdateContrastRange());

                UInt16 deadDefaultValue = (j == 0) ? (ushort)0xFF : (ushort)0xFFFF;
                UInt16 subfrDefaultValue = deadDefaultValue;

                if (contrastList.Count > 0)
                {
                    deadDefaultValue = (UInt16)param.GetDeadDiv(param.FrameRate, Convert.ToByte(contrastList[0]),
                                        param.NumCommonLines);
                    subfrDefaultValue = (UInt16)param.GetSubfrDiv(param.FrameRate, Convert.ToByte(contrastList[0]),
                                         param.NumCommonLines);
                }

                for (byte i = CyParamRange.CONTRAST_MIN; i < CyParamRange.CONTRAST_MAX; i += 10)
                {
                    if (contrastList.Contains(i.ToString()) == false)
                    {
                        deadArray.AppendFormat("{0}u, ", (j == 0) ? ToHex1BStr(deadDefaultValue) :
                                                                    ToHex2BStr(deadDefaultValue));
                        subfrArray.AppendFormat("{0}u, ", (j == 0) ? ToHex1BStr(subfrDefaultValue) :
                                                                     ToHex2BStr(subfrDefaultValue));
                    }
                    else
                    {
                        UInt16 deadDiv = (UInt16)param.GetDeadDiv(param.FrameRate, i, param.NumCommonLines);
                        deadArray.AppendFormat("{0}u, ", (j == 0) ? ToHex1BStr(deadDiv) : ToHex2BStr(deadDiv));

                        subfrDiv_u = (UInt16)param.GetSubfrDiv(param.FrameRate, i, param.NumCommonLines);
                        subfrArray.AppendFormat("{0}u, ", (j == 0) ? ToHex1BStr(subfrDiv_u) : ToHex2BStr(subfrDiv_u));
                    }
                }
                deadArray.AppendFormat("0x00{0}u", (j == 1) ? "00" : "");
                subfrDiv_u = (UInt16)param.GetSubfrDiv(param.FrameRate, CyParamRange.CONTRAST_MAX,
                                param.NumCommonLines);
                subfrArray.AppendFormat("{0}u", (j == 0) ? ToHex1BStr(subfrDiv_u) : ToHex2BStr(subfrDiv_u));
                int dim = 32;
                string speed = (j == 0) ? "L" : "H";
                contrastDefines.AppendFormat("static const uint{0} {1}_dividers{2}S[][{1}_CONTRAST_DIVIDERS_NUMBER] = {{\r\n{{{3}}},\r\n{{{4}}}}};",
                                             dim,  m_instanceName, speed, deadArray, subfrArray);
                contrastDefines.AppendLine();
            }

            param.m_desiredLCDMode = param.LCDMode;
            paramDict.Add("ContrastDefines", contrastDefines.ToString().TrimEnd('\r', '\n'));
        }

        public void CollectGeneralAPIs(ref Dictionary<string, string> paramDict, ICyTerminalQuery_v1 termQuery,
            CyLCDParameters param)
        {
            StringBuilder writerWriteInvertState = new StringBuilder();
            StringBuilder writerClearDisplay = new StringBuilder();

            for (int i = 0; i < m_parameters.PortsNumberFor_4_Coms(); i++)
            {
                writerWriteInvertState.AppendFormat("        {0}_DATA{1}_REG = (uint32)(~{0}_DATA{1}_REG);", m_instanceName, i);
                writerClearDisplay.AppendFormat("    {0}_DATA{1}_REG = 0Lu;", m_instanceName, i);
                writerWriteInvertState.AppendLine();
                writerClearDisplay.AppendLine();
            }
            for (int i = 0; i < m_parameters.PortsNumberFor_8_Coms(); i++)
            {
                writerWriteInvertState.AppendFormat("        {0}_DATA1{1}_REG = (uint32)(~{0}_DATA1{1}_REG);", m_instanceName, i);
                writerClearDisplay.AppendFormat("    {0}_DATA1{1}_REG = 0Lu;", m_instanceName, i);
                writerWriteInvertState.AppendLine();
                writerClearDisplay.AppendLine();
            }
            for (int i = 0; i < m_parameters.PortsNumberFor_16_Coms(); i++)
            {
                writerWriteInvertState.AppendFormat("        {0}_DATA2{1}_REG = (uint32)(~{0}_DATA2{1}_REG);", m_instanceName, i);
                writerWriteInvertState.AppendLine();
                writerWriteInvertState.AppendFormat("        {0}_DATA3{1}_REG = (uint32)(~{0}_DATA3{1}_REG);", m_instanceName, i);
                writerWriteInvertState.AppendLine();
                writerClearDisplay.AppendFormat("    {0}_DATA2{1}_REG = 0Lu;", m_instanceName, i);
                writerClearDisplay.AppendLine();
                writerClearDisplay.AppendFormat("    {0}_DATA3{1}_REG = 0Lu;", m_instanceName, i);
                writerClearDisplay.AppendLine();
            }

            paramDict.Add("writerWriteInvertState", writerWriteInvertState.ToString().TrimEnd('\r', '\n'));
            paramDict.Add("writerClearDisplay", writerClearDisplay.ToString().TrimEnd('\r', '\n'));
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------

        #region Core
        public void CollectApiCore(ICyInstQuery_v1 inst, List<CyHelperInfo> helpersConfig,
                                   ref Dictionary<string, string> paramDict, int pNumCommonLines, int pNumSegmentLines)
        {
            #region FuncDeclarations
            using (TextWriter writerHFuncDeclarations = new StringWriter(), writerCFunctions = new StringWriter())
            {
                bool defineSegment7 = false;
                bool defineSegment14 = false;
                bool defineSegment16 = false;
                bool defineMatrix = false;
                bool defineBar = false;

                writerCFunctions.WriteLine();
                writerCFunctions.WriteLine();
                //first one is blank
                for (int i = 1; i < helpersConfig.Count; i++)
                {
                    int p = i - 1;
                    string funcDispN = WriteDispNArray(helpersConfig[i], p);
                    switch (helpersConfig[i].Kind)
                    {
                        case CyHelperKind.SEGMENT_7:
                            defineSegment7 = true;
                            Write7SegDigit_n(writerCFunctions, writerHFuncDeclarations, p, funcDispN);
                            Write7SegNumber_n(writerCFunctions, writerHFuncDeclarations, p);
                            break;
                        case CyHelperKind.SEGMENT_14:
                            defineSegment14 = true;
                            PutChar14seg_n(writerCFunctions, writerHFuncDeclarations, p, funcDispN);
                            WriteString14seg_n(writerCFunctions, writerHFuncDeclarations, p);
                            break;
                        case CyHelperKind.SEGMENT_16:
                            defineSegment16 = true;
                            PutChar16seg_n(writerCFunctions, writerHFuncDeclarations, p, funcDispN);
                            WriteString16seg_n(writerCFunctions, writerHFuncDeclarations, p);
                            break;
                        case CyHelperKind.BAR:
                            defineBar = true;
                            WriteBargraph_n(writerCFunctions, writerHFuncDeclarations, p,
                                            helpersConfig[i].SymbolsCount, funcDispN);
                            break;
                        case CyHelperKind.MATRIX:
                            defineMatrix = true;
                            PutCharDotMatrix_n(writerCFunctions, writerHFuncDeclarations, p, funcDispN);
                            WriteStringDotMatrix_n(writerCFunctions, writerHFuncDeclarations, p);
                            break;
                        default:
                            break;
                    }
                }
                // Defines
                writerHFuncDeclarations.WriteLine();                
                if (defineSegment7)
                {
                    WriteHelperKindDefine(writerHFuncDeclarations, CyHelperKind.SEGMENT_7);
                }
                if (defineSegment14)
                {
                    WriteHelperKindDefine(writerHFuncDeclarations, CyHelperKind.SEGMENT_14);
                }
                if (defineSegment16)
                {
                    WriteHelperKindDefine(writerHFuncDeclarations, CyHelperKind.SEGMENT_16);
                }
                if (defineMatrix)
                {
                    WriteHelperKindDefine(writerHFuncDeclarations, CyHelperKind.MATRIX);
                }

                paramDict.Add("writerCFunctions", writerCFunctions.ToString().TrimEnd('\r', '\n') +
                    ((defineSegment7 || defineSegment14 || defineSegment16 || defineMatrix || defineBar) ? Environment.NewLine : ""));
                paramDict.Add("writerHFuncDeclarations", writerHFuncDeclarations.ToString().TrimEnd('\r', '\n'));
            }

            #endregion

        }
        #endregion

        //------------------------------------------------------------------------------------------------------------

        #region Auxiliary Functions

        private string HelperSegToArrString(CyHelperInfo hi)
        {
            StringBuilder str = new StringBuilder();
            string[,] p = new string[hi.SymbolsCount, hi.SegmentCount];

            for (int i = 0; i < hi.m_helpSegInfo.Count; i++)
            {
                CyHelperSegmentInfo hsi = hi.m_helpSegInfo[i];
                if ((hsi.Segment >= 0) && (hsi.Common >= 0))
                    p[hsi.m_digitNum, hsi.m_relativePos] = string.Format("{0}_{1}", m_instanceName, hsi.Name);
                else
                {
                    p[hsi.m_digitNum, hsi.m_relativePos] = string.Format("{0}_NOT_CON", m_instanceName);
                }
            }
            for (int i = 0; i < hi.SymbolsCount; i++)
            {
                str.Append("{");
                for (int j = 0; j < hi.SegmentCount; j++)
                {
                    str.Append(p[i, j]);
                    if (j != hi.SegmentCount - 1) str.Append(", ");
                }
                str.Append("}");
                if (i != hi.SymbolsCount - 1) str.Append(", ");
            }
            return str.ToString();
        }

        private string WriteDispNArray(CyHelperInfo hi, int index)
        {
            string str = HelperSegToArrString(hi);
            if (hi.Kind != CyHelperKind.BAR)
            {
                str = string.Format("static const uint32 {0}_disp{1}[{2}u][{3}u] = {{{4}}};", m_instanceName,
                                     index, hi.SymbolsCount, hi.SegmentCount, str);
            }
            else
            {
                str = string.Format("static const uint32 {0}_disp{1}[{2}u][{3}u] = {{{{0u}}, {4}}};",
                                    m_instanceName, index, (hi.SymbolsCount + 1), hi.SegmentCount, str);
            }
            const int STANDARD_1_INDENT = 1;
            str = InsertTabs(StringLineBreaking(str, STANDARD_1_INDENT).ToString(), STANDARD_1_INDENT);
            str = String.Concat("".PadLeft(4), str);
            return str;
        }

        private string WriteDigitNumVar(int symbolsCount, int index)
        {
            return String.Format(GenerateDefine(String.Format("DIGIT_NUM_{0}", index), String.Format("({0}u)", symbolsCount)));
        }

        /// <summary>
        /// Breaks up string into lines with max length 120.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static StringBuilder StringLineBreaking(string str, int tabsNum)
        {
            int maxLineLen = MAX_LINE_LEN - tabsNum * 4;
            int index = maxLineLen;
            StringBuilder strBuild = new StringBuilder(str);
            while (index < strBuild.Length)
            {
                while ((strBuild[index] != ' ') && (index > 0))
                    index--;
                if (index > 0)
                {
                    // Add "\r\n" at the place of the line breaking
                    strBuild.Insert(index + 1, Environment.NewLine);
                    // Remove ' '
                    strBuild.Remove(index, 1);
                    // Add
                    index += maxLineLen + 2;
                }
            }
            return strBuild;
        }

        private static string String2DArrayLineBreaking(string str, int tabsNum)
        {
            int maxLineLen = MAX_LINE_LEN - tabsNum * 4;
            StringBuilder strBuild = new StringBuilder(str);
            int index = strBuild.ToString().IndexOf('{');
            if (index < 0)
            {
                strBuild = StringLineBreaking(strBuild.ToString(), tabsNum);
            }
            else
            {
                strBuild.Insert(index + 1, Environment.NewLine);
                index += maxLineLen;
                while (index < strBuild.Length)
                {
                    int prevIndex = index - maxLineLen;
                    while ((strBuild[index] != '}') && (index > prevIndex))
                        index--;
                    if (index <= prevIndex)
                    {
                        index = strBuild.ToString().Substring(prevIndex,
                                Math.Min(strBuild.Length - prevIndex, maxLineLen)).LastIndexOf(",") + prevIndex - 1;
                        if (index < 0)
                        {
                            break;
                        }
                    }

                    // Add NewLine after "},"
                    index = strBuild.ToString().IndexOf(", ", index);
                    if (index < 0)
                    {
                        break;
                    }
                    // Remove
                    strBuild.Remove(index + 1, 1);
                    strBuild.Insert(index + 1, Environment.NewLine);
                    // Shift index
                    index += maxLineLen + 1;

                }
                index = strBuild.ToString().IndexOf("};");
                if (index > 0)
                {
                    strBuild.Insert(index, Environment.NewLine);
                }
            }

            return InsertTabs(strBuild.ToString(), tabsNum);
        }

        private static string InsertTabs(string text, int tabsNum)
        {
            string tabs = "".PadLeft(tabsNum * 4);
            text = text.Replace("\n", "\n" + tabs);
            text = text.TrimEnd();
            return text;
        }

        private string GenerateDefine(string str1, string str2)
        {
            const int FIRST_PART_WIDTH = 29; //offset to align defines in one position in generated header file
            return String.Format("#define {0}_{1}{2}{3}", m_instanceName, str1,
                        " ".PadRight(Math.Max(1, FIRST_PART_WIDTH - (str1.Length + m_instanceName.Length + 1))), str2);
        }

        private string ToHex1BStr(UInt16 value)
        {
            return "0x" + value.ToString("X2");
        }
        private string ToHex2BStr(UInt16 value)
        {
            return "0x" + value.ToString("X4");
        }
        #endregion
    }
}
