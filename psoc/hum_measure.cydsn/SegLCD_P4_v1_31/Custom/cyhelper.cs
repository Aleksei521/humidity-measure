/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections;
using System.IO;

namespace SegLCD_P4_v1_31
{
    #region Helper classes

    /// <summary>
    /// Class that represents unassigned pixels
    /// </summary>
    public class CySegmentInfo
    {
        private string m_name;
        private int m_common;
        private int m_segment;

        [XmlAttribute("Name")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [XmlAttribute("Com")]
        public int Common
        {
            get { return m_common; }
            set { m_common = value; }
        }

        [XmlAttribute("Seg")]
        public int Segment
        {
            get { return m_segment; }
            set { m_segment = value; }
        }

        public CySegmentInfo()
        {
            Name = CyLCDParameters.UNASSIGNED_PIXEL_NAME;
        }

        public CySegmentInfo(string name, int common, int segment)
        {
            Name = name;
            Common = common;
            Segment = segment;
        }
    }

    /// <summary>
    /// Class that represents pixels of the helper
    /// </summary>
    [XmlRootAttribute("Pixel")]
    public class CyHelperSegmentInfo : CySegmentInfo
    {
        [XmlAttribute("Display")]
        public int m_displayNum;
        [XmlAttribute("Digit")]
        public int m_digitNum;
        [XmlAttribute("Helper")]
        public CyHelperKind m_helperType;
        [XmlAttribute("RelPos")]
        public int m_relativePos;

        [XmlElement("GlobalDigitNum")]
        public int m_globalDigitNum;

        public CyHelperSegmentInfo()
        {
            m_helperType = CyHelperKind.EMPTY;
        }

        public CyHelperSegmentInfo(string name, int common, int segment, int displayNum, int digitNum, int relPos,
                                   CyHelperKind type, int globalDigitNum)
            : base(name, common, segment)
        {
            m_displayNum = displayNum;
            m_digitNum = digitNum;
            m_relativePos = relPos;
            m_helperType = type;

            m_globalDigitNum = globalDigitNum;
        }

        public int GetIndex(int CommonCount)
        {
            return CommonCount * Segment + Common;
        }
    }

    /// <summary>
    /// Class that represents a helper function
    /// </summary>
    [XmlType("HelperInfo")]
    public class CyHelperInfo
    {
        public const int MATRIX_WIDTH = 5;
        public const int MATRIX_HEIGHT = 8;
        public const int PIXELS_COUNT_7SEG = 7;
        public const int PIXELS_COUNT_14SEG = 14;
        public const int PIXELS_COUNT_16SEG = 16;
        public const int PIXELS_COUNT_BAR = 1;
        public const int PIXELS_COUNT_MATRIX = MATRIX_WIDTH * MATRIX_HEIGHT;

        public const int MAX_7SEG_SYMBOLS = 9;
        public const int MAX_14SEG_SYMBOLS = 20;
        public const int MAX_16SEG_SYMBOLS = 20;
        public const int MAX_BAR_SYMBOLS = 255;
        public const int MAX_MATRIX_SYMBOLS = 8;

        [XmlElement("Name")]
        public string m_name;

        private CyHelperKind m_kind;
        private int m_symbolsCount;
        private int m_maxSymbolsCount;
        private int m_segmentCount;
        private int m_displayNum;

        [XmlElementAttribute("Color")]
        public int m_helperColor;

        // m_GlobalHelperIndex is used for the names of the helpers.
        // Each kind of helpers (7-segment, 14-segment, etc.) has its own numeration.
        [XmlElement("GlobalHelperIndex")]
        public int m_globalHelperIndex;

        [XmlIgnore]
        public Color HelperColor
        {
            get { return Color.FromArgb(m_helperColor); }
            set { m_helperColor = value.ToArgb(); }
        }

        [XmlElementAttribute("Kind")]
        public CyHelperKind Kind
        {
            get { return m_kind; }
            set { m_kind = value; }
        }

        [XmlElementAttribute("MaxSymbolsCount")]
        public int MaxSymbolsCount
        {
            get { return m_maxSymbolsCount; }
            set { m_maxSymbolsCount = value; }
        }

        [XmlElementAttribute("SegmentCount")]
        public int SegmentCount
        {
            get { return m_segmentCount; }
            set { m_segmentCount = value; }
        }

        [XmlElementAttribute("DisplayNum")]
        public int DisplayNum
        {
            get { return m_displayNum; }
            set { m_displayNum = value; }
        }

        [XmlArray("HelpSegInfoArray")]
        [XmlArrayItem("HelperSegmentInfo")]
        public List<CyHelperSegmentInfo> m_helpSegInfo;

        public int SymbolsCount
        {
            get { return m_symbolsCount; }
            set { m_symbolsCount = value; }
        }

        public CyHelperInfo()
        {
            m_helpSegInfo = new List<CyHelperSegmentInfo>();

            m_name = "Empty";
            m_kind = CyHelperKind.EMPTY;
            SymbolsCount = 0;
            m_maxSymbolsCount = 0;
            m_segmentCount = 0;
        }

        public CyHelperInfo(string name, CyHelperKind kind, int maxSymbolsCount, int segmentCount, Color color,
                          int helperIndex)
        {
            m_name = name;
            m_kind = kind;
            m_maxSymbolsCount = maxSymbolsCount;
            m_segmentCount = segmentCount;
            SymbolsCount = 0;
            m_helpSegInfo = new List<CyHelperSegmentInfo>();

            HelperColor = color;
            m_globalHelperIndex = helperIndex;
        }

        //--------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return m_name;
        }

        public string GetPixelNameByCommonSegment(int common, int segment)
        {
            string name = "";
            for (int i = 0; i < m_helpSegInfo.Count; i++)
            {
                if ((m_helpSegInfo[i].Common == common) &&
                    (m_helpSegInfo[i].Segment == segment))
                {
                    name = m_helpSegInfo[i].Name;
                    break;
                }
            }

            return name;
        }

        public CySegmentInfo GetPixelByCommonSegment(int common, int segment)
        {
            CySegmentInfo pixel = null;
            for (int i = 0; i < m_helpSegInfo.Count; i++)
            {
                if ((m_helpSegInfo[i].Common == common) &&
                    (m_helpSegInfo[i].Segment == segment))
                {
                    pixel = m_helpSegInfo[i];
                    break;
                }
            }

            return pixel;
        }

        public CySegmentInfo GetPixelByName(string name)
        {
            CySegmentInfo pixel = null;
            for (int i = 0; i < m_helpSegInfo.Count; i++)
            {
                if (m_helpSegInfo[i].Name == name)
                {
                    pixel = m_helpSegInfo[i];
                    break;
                }
            }

            return pixel;
        }

        public CyHelperSegmentInfo GetPixelBySymbolSegment(int symbol, int segment)
        {
            CyHelperSegmentInfo pixel = null;
            for (int i = 0; i < m_helpSegInfo.Count; i++)
            {
                if ((m_helpSegInfo[i].m_digitNum == symbol) &&
                    (m_helpSegInfo[i].m_relativePos == segment))
                {
                    pixel = m_helpSegInfo[i];
                    break;
                }
            }
            return pixel;
        }

        public string GetDefaultSymbolName(int symbol)
        {
            string nameTemplate = "H*" + symbol + "_";
            string name = nameTemplate;
            switch (Kind)
            {
                case CyHelperKind.SEGMENT_7:
                    name = nameTemplate.Replace("*", "7seg");
                    break;
                case CyHelperKind.SEGMENT_14:
                    name = nameTemplate.Replace("*", "14seg");
                    break;
                case CyHelperKind.SEGMENT_16:
                    name = nameTemplate.Replace("*", "16seg");
                    break;
                case CyHelperKind.BAR:
                    name = "HBar";
                    break;
                case CyHelperKind.MATRIX:
                    name = nameTemplate.Replace("*", "Dot");
                    break;
                default:
                    break;
            }
            return name.ToUpper();
        }

        public void AddSymbol(int symbolGlobalNum)
        {
            if ((SymbolsCount >= MaxSymbolsCount) || (Kind == CyHelperKind.EMPTY))
                return;

            string nameTemplate = GetDefaultSymbolName(symbolGlobalNum);
            string name = "SEG";
            char letter = 'A';
            for (int i = 0; i < SegmentCount; i++)
            {
                switch (Kind)
                {
                    case CyHelperKind.SEGMENT_7:
                        name = nameTemplate + ((char)('A' + i));
                        break;
                    case CyHelperKind.SEGMENT_14:
                        name = nameTemplate + ((char)('A' + i));
                        break;
                    case CyHelperKind.SEGMENT_16:
                        if ((i == 1))
                        {
                            name = nameTemplate + "A_";
                        }
                        else if (i == 4)
                        {
                            name = nameTemplate + "D_";
                        }
                        else
                        {
                            name = nameTemplate + letter;
                            letter = (char)(letter + 1);
                        }
                        break;
                    case CyHelperKind.BAR:
                        name = nameTemplate + symbolGlobalNum;
                        break;
                    case CyHelperKind.MATRIX:
                        name = nameTemplate + (i % MATRIX_WIDTH) + (i / MATRIX_WIDTH);
                        break;
                    default:
                        break;
                }
                m_helpSegInfo.Add(new CyHelperSegmentInfo(name.ToUpper(), -1, -1, 0, SymbolsCount, i, Kind,
                                                          symbolGlobalNum));
            }

            SymbolsCount++;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Static functions

        public static void CreateHelper(CyHelperKind kind, CyLCDParameters parameters)
        {
            int num = AddNextHelperIndex(kind, parameters);
            CyHelperInfo helper;
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    helper = new CyHelperInfo("Helper_7Segment_" + num, kind, MAX_7SEG_SYMBOLS, PIXELS_COUNT_7SEG,
                                            parameters.m_colorChooser.PopCl(), num);
                    break;
                case CyHelperKind.SEGMENT_14:
                    helper = new CyHelperInfo("Helper_14Segment_" + num, kind, MAX_14SEG_SYMBOLS, PIXELS_COUNT_14SEG,
                                            parameters.m_colorChooser.PopCl(), num);
                    break;
                case CyHelperKind.SEGMENT_16:
                    helper = new CyHelperInfo("Helper_16Segment_" + num, kind, MAX_16SEG_SYMBOLS, PIXELS_COUNT_16SEG,
                                            parameters.m_colorChooser.PopCl(), num);
                    break;
                case CyHelperKind.BAR:
                    helper = new CyHelperInfo("Helper_Bar_" + num, kind, MAX_BAR_SYMBOLS, PIXELS_COUNT_BAR,
                                            parameters.m_colorChooser.PopCl(), num);
                    break;
                case CyHelperKind.MATRIX:
                    helper = new CyHelperInfo("Helper_Matrix_" + num, kind, MAX_MATRIX_SYMBOLS, PIXELS_COUNT_MATRIX,
                                            parameters.m_colorChooser.PopCl(), num);
                    break;
                default:
                    helper = new CyHelperInfo();
                    break;
            }
            helper.DisplayNum = parameters.m_helpersConfig.Count - 1;
            parameters.m_helpersConfig.Add(helper);

            //Pixels management
            int NumCommonLines = Convert.ToInt32(parameters.NumCommonLines);
            int NumSegmentLines = Convert.ToInt32(parameters.NumSegmentLines);
            if (kind == CyHelperKind.EMPTY)
            {
                for (int j = 0; j < NumSegmentLines; j++)
                    for (int i = 0; i < NumCommonLines; i++)
                    {
                        helper.m_helpSegInfo.Add(
                            new CyHelperSegmentInfo(CyLCDParameters.UNASSIGNED_PIXEL_NAME + (j * NumCommonLines + i), i,
                                                    j, -1, -1, -1, kind, -1));
                    }
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public static void UpdateEmptyHelper(CyLCDParameters parameters)
        {
            int NumCommonLines = Convert.ToInt32(parameters.NumCommonLines);
            int NumSegmentLines = Convert.ToInt32(parameters.NumSegmentLines);
            parameters.m_helpersConfig[0].m_helpSegInfo.Clear();
            for (int j = 0; j < NumSegmentLines; j++)
                for (int i = 0; i < NumCommonLines; i++)
                {
                    parameters.m_helpersConfig[0].m_helpSegInfo.Add(new CyHelperSegmentInfo(
                        CyLCDParameters.UNASSIGNED_PIXEL_NAME + (j * NumCommonLines + i), i, j, -1, -1, -1,
                        CyHelperKind.EMPTY, -1));
                }
        }

        //--------------------------------------------------------------------------------------------------------------

        public static int GetTotalPixelNumber(CyLCDParameters parameters)
        {
            int totalPixels = 0;
            for (int i = 1; i < parameters.m_helpersConfig.Count; i++)
            {
                totalPixels += parameters.m_helpersConfig[i].m_helpSegInfo.Count;
            }
            return totalPixels;
        }

        public static bool CheckPixelUniqueName(CyLCDParameters parameters, string name, int n)
        {
            // false - if there are matches
            int times = 0;
            for (int i = 0; i < parameters.m_helpersConfig.Count; i++)
                for (int j = 0; j < parameters.m_helpersConfig[i].m_helpSegInfo.Count; j++)
                {
                    if (parameters.m_helpersConfig[i].m_helpSegInfo[j].Name.ToUpper() == name.ToUpper())
                    {
                        times++;
                        if (times > n)
                            return false;
                    }
                }
            return true;
        }

        public static bool CheckHelperUniqueName(CyLCDParameters parameters, string name)
        {
            // false - if there are matches
            int times = 0;
            for (int i = 1; i < parameters.m_helpersConfig.Count; i++)
            {
                if (parameters.m_helpersConfig[i].m_name == name)
                {
                    times++;
                    if (times > 0)
                        return false;
                }
            }
            return true;
        }

        public static int AddNextHelperIndex(CyHelperKind kind, CyLCDParameters parameters)
        {
            int index = 0;
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    while (parameters.m_helperIndexes_7SEG.Contains(index))
                        index++;
                    parameters.m_helperIndexes_7SEG.Add(index);
                    break;
                case CyHelperKind.SEGMENT_14:
                    while (parameters.m_helperIndexes_14SEG.Contains(index))
                        index++;
                    parameters.m_helperIndexes_14SEG.Add(index);
                    break;
                case CyHelperKind.SEGMENT_16:
                    while (parameters.m_helperIndexes_16SEG.Contains(index))
                        index++;
                    parameters.m_helperIndexes_16SEG.Add(index);
                    break;
                case CyHelperKind.BAR:
                    while (parameters.m_helperIndexes_BAR.Contains(index))
                        index++;
                    parameters.m_helperIndexes_BAR.Add(index);
                    break;
                case CyHelperKind.MATRIX:
                    while (parameters.m_helperIndexes_MATRIX.Contains(index))
                        index++;
                    parameters.m_helperIndexes_MATRIX.Add(index);
                    break;
                default:
                    break;
            }
            return index;
        }

        public static void RemoveHelperIndex(int index, CyHelperKind kind, CyLCDParameters parameters)
        {
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    parameters.m_helperIndexes_7SEG.Remove(index);
                    break;
                case CyHelperKind.SEGMENT_14:
                    parameters.m_helperIndexes_14SEG.Remove(index);
                    break;
                case CyHelperKind.SEGMENT_16:
                    parameters.m_helperIndexes_16SEG.Remove(index);
                    break;
                case CyHelperKind.BAR:
                    parameters.m_helperIndexes_BAR.Remove(index);
                    break;
                case CyHelperKind.MATRIX:
                    parameters.m_helperIndexes_MATRIX.Remove(index);
                    break;
                default:
                    break;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public static string SerializeHelpers(List<CyHelperInfo> list, XmlSerializer s,
                                              XmlSerializerNamespaces customSerNamespace)
        {
            ArrayList helpers = new ArrayList(list);
            string serializedXml = "";
            using (StringWriter sw = new StringWriter())
            {
                s.Serialize(sw, helpers, customSerNamespace);
                serializedXml = sw.ToString();
            }
            return serializedXml;
        }

        public static List<CyHelperInfo> DeserializeHelpers(string serializedXml)
        {
            Type[] theExtraTypes = new Type[2];
            theExtraTypes[0] = typeof(CyHelperInfo);
            theExtraTypes[1] = typeof(ArrayList);
            XmlSerializer s = new XmlSerializer(typeof(ArrayList), theExtraTypes);
            List<CyHelperInfo> newList = new List<CyHelperInfo>();
            using (StringReader sr = new StringReader(serializedXml))
            {
                ArrayList helpers = (ArrayList)s.Deserialize(sr);
                for (int i = 0; i < helpers.Count; i++)
                {
                    newList.Add((CyHelperInfo)helpers[i]);
                }
            }
            return newList;
        }

        #endregion Static functions
    }

    #endregion Helper classes

    #region CyColorList Class

    /// <summary>
    /// Class that is used for painting the Helper List in different colors
    /// </summary>
    public class CyColorList
    {
        private readonly List<Color> m_colorsStack = new List<Color>();

        public CyColorList()
        {
            m_colorsStack.Add(Color.FromArgb(226, 147, 147));
            m_colorsStack.Add(Color.FromArgb(226, 200, 147));
            m_colorsStack.Add(Color.FromArgb(200, 226, 147));
            m_colorsStack.Add(Color.FromArgb(147, 226, 200));
            m_colorsStack.Add(Color.FromArgb(147, 200, 226));
            m_colorsStack.Add(Color.FromArgb(147, 147, 226));
            m_colorsStack.Add(Color.FromArgb(200, 147, 226));
            m_colorsStack.Add(Color.FromArgb(226, 147, 200));
        }

        public void PushCl(Color clr)
        {
            if (clr != Color.MintCream)
            {
                m_colorsStack.Add(clr);
            }
        }

        public Color PopCl()
        {
            Color res = Color.MintCream;
            if (m_colorsStack.Count > 0)
            {
                res = m_colorsStack[m_colorsStack.Count - 1];
                m_colorsStack.Remove(res);
            }
            return res;
        }

        public void PopCl(Color clr)
        {
            m_colorsStack.Remove(clr);
        }
    }

    #endregion
}
