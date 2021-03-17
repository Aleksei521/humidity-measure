/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System.IO;

namespace SegLCD_P4_v1_31
{
    partial class CyAPIGenerator
    {
        // Lines below that have more then 120 characters are not wrapped for better readability

        #region Write7SegDisp
        void Write7SegDigit_n(TextWriter writer, TextWriter writer_h, int index, string varDecl)
        {
            writer_h.WriteLine("void    {0}_Write7SegDigit_{1}(uint32 digit, uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_Write7SegDigit_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays a hexadecimal digit on an array of 7-segment display");
            writer.WriteLine("*  elements. Digits can be hexadecimal values in the range of 0 to 9 and A to F.");
            writer.WriteLine("*  The customizer Display Helpers facility must be used to define the pixel set");
            writer.WriteLine("*  associated with the 7-segment display elements. Multiple 7-segment display");
            writer.WriteLine("*  elements can be defined in the frame buffer and are addressed through the");
            writer.WriteLine("*  suffix (n) in the function name. This function is only included if a");
            writer.WriteLine("*  7-segment display element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  digit : Unsigned integer value in the range of 0 to 15 to be displayed as a");
            writer.WriteLine("*          hexadecimal digit. The ASCII numbers of a hexadecimal characters are");
            writer.WriteLine("*          also valid. In case of a invalid digit value displays 0 in position");
            writer.WriteLine("*          specified.");
            writer.WriteLine("*  position : Position of the digit as counted right to left starting at 0 on");
            writer.WriteLine("*             the right. If the position is outside the defined display area,");
            writer.WriteLine("*             the character will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_7SegDigits[] - used as a look-up table for 7 Segment helper.", m_instanceName);
            writer.WriteLine("*  Holds decoded digit's pixel reflection for the helper.");
            writer.WriteLine("*");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");

            writer.WriteLine("void {0}_Write7SegDigit_{1}(uint32 digit, uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine(varDecl);
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("");
            writer.WriteLine("    /* if digit < 16 then do nothing (we have correct data) */");
            writer.WriteLine("    if (digit <= 16u)");
            writer.WriteLine("    {");
            writer.WriteLine("        /* nothing to do, as we have correct digit value */");
            writer.WriteLine("    }");
            writer.WriteLine("    /* if digit <= 0x39 then digit is ASCII code of a number (0-9) */");
            writer.WriteLine("    else if (digit <= 0x39u)");
            writer.WriteLine("    {");
            writer.WriteLine("        digit -= 0x30u;");
            writer.WriteLine("    }");
            writer.WriteLine("    /* if digit <= 0x46 then digit is ASCII code of a hex number (A-F) */");
            writer.WriteLine("    else if (digit <= 0x46u)");
            writer.WriteLine("    {");
            writer.WriteLine("        digit -= 0x37u;");
            writer.WriteLine("    }");
            writer.WriteLine("    /* else we have invalid digit, and we will print '0' instead */");
            writer.WriteLine("    else");
            writer.WriteLine("    {");
            writer.WriteLine("        digit = 0u;");
            writer.WriteLine("    }");
            writer.WriteLine("");
            writer.WriteLine("    if ((position / {0}_DIGIT_NUM_{1}) == 0u)", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        position = ({0}_DIGIT_NUM_{1} - position) - 1u;", m_instanceName, index);
            writer.WriteLine("        for (i = 0u; i < {0}_7SEG_PIX_NUM; i++)", m_instanceName);
            writer.WriteLine("        {");
            writer.WriteLine("            (void){0}_WritePixel({1}_disp{2}[position][i],", m_instanceName, m_instanceName, index);
            writer.WriteLine("            (((uint32)((uint32){0}_7SegDigits[digit] >> i)) & {1}_PIXEL_STATE_ON));", m_instanceName, m_instanceName);
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }

        void Write7SegNumber_n(TextWriter writer, TextWriter writer_h, int index)
        {
            writer_h.WriteLine("void    {0}_Write7SegNumber_{1}(uint32 value, uint32 position, uint32 mode);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_Write7SegNumber_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays a 16-bit integer value on a 1- to 5-digit array of");
            writer.WriteLine("*  7-segment display elements. The customizer Display Helpers facility must be");
            writer.WriteLine("*  used to define the pixel set associated with the 7-segment display element/s.");
            writer.WriteLine("*  Multiple 7-segment display element groups can be defined in the frame buffer");
            writer.WriteLine("*  and are addressed through the suffix (n) in the function name. Sign ");
            writer.WriteLine("*  conversion, sign display, decimal points, and other custom features must be");
            writer.WriteLine("*  handled by application-specific user code. This function is only included if");
            writer.WriteLine("*  a 7-segment display element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  value:    Unsigned integer value to be displayed.");
            writer.WriteLine("*  position: The position of the least significant digit as counted right to");
            writer.WriteLine("*            left starting at 0 on the right. If the defined display contains ");
            writer.WriteLine("*            fewer digits then the value requires for display for the most ");
            writer.WriteLine("*            significant digit/s will not be displayed.");
            writer.WriteLine("*  mode:     Sets the display mode.");
            writer.WriteLine("*    Define                         Description");
            writer.WriteLine("*     {0}_NO_LEADING_ZEROES          No leading zeroes.", m_instanceName, index);
            writer.WriteLine("*     {0}_LEADING_ZEROES             Leading zeroes are displayed.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_Write7SegNumber_{1}(uint32 value, uint32 position, uint32 mode)", m_instanceName, index);
            writer.WriteLine("{ ");
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("    uint32 num;");
            writer.WriteLine("");
            writer.WriteLine("    position = position % {0}_DIGIT_NUM_{1};", m_instanceName, index);
            writer.WriteLine("");
            writer.WriteLine("    /* While current digit position in the range of display keep processing the output */");
            writer.WriteLine("    for (i = position; i < {0}_DIGIT_NUM_{1}; i++)", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        num = value % 10u;");
            writer.WriteLine("        if((0u == value) && (0u == mode))");
            writer.WriteLine("        {");
            writer.WriteLine("            /* In case of 'no leading zeros' each extra digit will be cleared */");
            writer.WriteLine("            {0}_Write7SegDigit_{1}({0}_7SEG_BLANK_DIG, i);", m_instanceName, index);
            writer.WriteLine("        }");
            writer.WriteLine("        else");
            writer.WriteLine("        {");
            writer.WriteLine("            /* Display subsequent digit or '0' if 'no leading zeros' mode */");
            writer.WriteLine("            {0}_Write7SegDigit_{1}(num, i);", m_instanceName, index);
            writer.WriteLine("        }");
            writer.WriteLine("        value = value / 10u;");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");

        }
        #endregion

        #region PutChar14SegDisp
        void PutChar14seg_n(TextWriter writer, TextWriter writer_h, int index, string varDecl)
        {
            writer_h.WriteLine("void    {0}_PutChar14Seg_{1}(uint8 character, uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_PutChar14Seg_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays an 8-bit character on an array of 14-segment");
            writer.WriteLine("*  alphanumeric character display elements. The customizer Display Helpers");
            writer.WriteLine("*  facility must be used to define the pixel set associated with the 14-segment");
            writer.WriteLine("*  display element. Multiple 14-segment alphanumeric display element groups can");
            writer.WriteLine("*  be defined in the frame buffer and are addressed through the suffix (n) in");
            writer.WriteLine("*  the function name. This function is only included if a 14-segment element is");
            writer.WriteLine("*  defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  character: The ASCII value of the character to display (printable characters");
            writer.WriteLine("*             with ASCII values 0 o 127).");
            writer.WriteLine("*  position:  Position of the character as counted left to right starting at 0");
            writer.WriteLine("*             on the left. If the position is outside the defined display area,");
            writer.WriteLine("*             the character will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*  {0}_14SegChars[] - used as a look-up table for 14 Segment", m_instanceName);
            writer.WriteLine("*  helper. Holds decoded character's pixel reflection for the helper.");
            writer.WriteLine("*");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_PutChar14Seg_{1}(uint8 character, uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine(varDecl);
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("");
            writer.WriteLine("    if (0u == (position / {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        for (i = 0u; i < {0}_14SEG_PIX_NUM; i++)", m_instanceName);
            writer.WriteLine("        {");
            writer.WriteLine("            (void){0}_WritePixel({1}_disp{2}[position][i],", m_instanceName, m_instanceName, index);
            writer.WriteLine("            (((uint32)((uint32){0}_14SegChars[character] >> i)) & {1}_PIXEL_STATE_ON));", m_instanceName, m_instanceName);
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }
        void WriteString14seg_n(TextWriter writer, TextWriter writer_h, int index)
        {
            writer_h.WriteLine("void    {0}_WriteString14Seg_{1}(uint8 const character[], uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_WriteString14Seg_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays a null terminated character string on an array of");
            writer.WriteLine("*  14-segment alphanumeric character display elements. The customizer Display");
            writer.WriteLine("*  Helpers facility must be used to define the pixel set associated with the 14");
            writer.WriteLine("*  segment display elements. Multiple 14-segment alphanumeric display element");
            writer.WriteLine("*  groups can be defined in the frame buffer and are addressed through the");
            writer.WriteLine("*  suffix (n) in the function name. This function is only included if a");
            writer.WriteLine("*  14-segment display element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:  ");
            writer.WriteLine("*  character: Pointer to the null terminated character string.");
            writer.WriteLine("*  position:  The Position of the first character as counted left to right");
            writer.WriteLine("*             starting at 0 on the left. If the defined display contains fewer");
            writer.WriteLine("*             characters then the string requires for display, the extra");
            writer.WriteLine("*             characters will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_WriteString14Seg_{1}(uint8 const character[], uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine("    uint8 ch;");
            writer.WriteLine("    uint32 i = 0u;");
            writer.WriteLine("");
            writer.WriteLine("    ch = character[0u];");
            writer.WriteLine("");
            writer.WriteLine("    while (((uint8)\'\\0\' != ch) && (position != {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        {0}_PutChar14Seg_{1}(ch, position);", m_instanceName, index);
            writer.WriteLine("        i++;");
            writer.WriteLine("        ch = character[i];");
            writer.WriteLine("        position++;");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }
        #endregion

        #region PutChar16segDisp
        void PutChar16seg_n(TextWriter writer, TextWriter writer_h, int index, string varDecl)
        {
            writer_h.WriteLine("void    {0}_PutChar16Seg_{1}(uint8 character, uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_PutChar16Seg_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays an 8-bit character on an array of 16-segment");
            writer.WriteLine("*  alphanumeric character display elements. The customizer Display Helpers");
            writer.WriteLine("*  facility must be used to define the pixel set associated with the 16-segment");
            writer.WriteLine("*  display element(s). Multiple 16-segment alphanumeric display element groups");
            writer.WriteLine("*  can be defined in the frame buffer and are addressed through the suffix (n)");
            writer.WriteLine("*  in the function name. This function is only included if a 16-segment display");
            writer.WriteLine("*  element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:  ");
            writer.WriteLine("*  character: The ASCII value of the character to display (printable ASCII and");
            writer.WriteLine("*             table extended characters with values 0 to 255).");
            writer.WriteLine("*  position:  Position of the character as counted left to right starting at 0");
            writer.WriteLine("*             on the left. If the position is outside the display range, the");
            writer.WriteLine("*             character will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*  {0}_16SegChars[] - used as a look-up table for 16 Segment ", m_instanceName);
            writer.WriteLine("*  helper. Holds decoded character's pixel reflection for the helper.");
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_PutChar16Seg_{1}(uint8 character, uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine(varDecl);
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("");
            writer.WriteLine("    if (0u == (position / {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        for (i = 0u; i < {0}_16SEG_PIX_NUM; i++)", m_instanceName);
            writer.WriteLine("        {");
            writer.WriteLine("            (void){0}_WritePixel({1}_disp{2}[position][i],", m_instanceName, m_instanceName, index);
            writer.WriteLine("            (((uint32)((uint32){0}_16SegChars[character] >> i)) & {1}_PIXEL_STATE_ON));", m_instanceName, m_instanceName);
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }
        void WriteString16seg_n(TextWriter writer, TextWriter writer_h, int index)
        {
            writer_h.WriteLine("void    {0}_WriteString16Seg_{1}(uint8 const character[], uint32 position);", m_instanceName, index);
            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_WriteString16Seg_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays a null terminated character string on an array of");
            writer.WriteLine("*  16-segment alphanumeric character display elements. The customizer Display");
            writer.WriteLine("*  Helpers facility must be used to define the pixel set associated with the 16");
            writer.WriteLine("*  segment display elements. Multiple 16-segment alphanumeric display element");
            writer.WriteLine("*  groups can be defined in the frame buffer and are addressed through the");
            writer.WriteLine("*  suffix (n) in the function name. This function is only included if a");
            writer.WriteLine("*  16-segment display element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:  ");
            writer.WriteLine("*  character: Pointer to the null terminated character string.");
            writer.WriteLine("*  position:  The position of the first character as counted left to right");
            writer.WriteLine("*             starting at 0 on the left. If the defined display contains fewer");
            writer.WriteLine("*             characters then the string requires for display, the extra");
            writer.WriteLine("*             characters will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_WriteString16Seg_{1}(uint8 const character[], uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine("    uint8 ch;");
            writer.WriteLine("    uint32 i = 0u;");
            writer.WriteLine("");
            writer.WriteLine("    ch = character[0u];");
            writer.WriteLine("");
            writer.WriteLine("    while (((uint8)\'\\0\' != ch) && (position != {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        {0}_PutChar16Seg_{1}(ch, position);", m_instanceName, index);
            writer.WriteLine("        i++;");
            writer.WriteLine("        ch = character[i];");
            writer.WriteLine("        position++;");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }
        #endregion

        #region WriteStringDotMatrix_n
        void PutCharDotMatrix_n(TextWriter writer, TextWriter writer_h, int index, string varDecl)
        {
            writer_h.WriteLine("void    {0}_PutCharDotMatrix_{1}(uint8 character, uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_PutCharDotMatrix_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays an 8-bit character on an array of dot-matrix");
            writer.WriteLine("*  alphanumeric character display elements. The customizer Display Helpers");
            writer.WriteLine("*  facility must be used to define the pixel set associated with the dot matrix");
            writer.WriteLine("*  display elements. Multiple dot-matrix alphanumeric display element groups can");
            writer.WriteLine("*  be defined in the frame buffer and are addressed through the suffix (n) in");
            writer.WriteLine("*  the function name. This function is only included if a dot-matrix display");
            writer.WriteLine("*   element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  character: The ASCII value of the character to display.");
            writer.WriteLine("*  position:  The Position of the character as counted left to right starting");
            writer.WriteLine("*             at 0 on the left. If the position is outside the display range,");
            writer.WriteLine("*             the character will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*  {0}_charDotMatrix[][] - used as a look-up table for Dot Matrix", m_instanceName);
            writer.WriteLine("*  helper. Holds decoded character's pixel reflection for the helper.");
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_PutCharDotMatrix_{1}(uint8 character, uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine(varDecl);
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("    uint32 j;");
            writer.WriteLine("");
            writer.WriteLine("    if (0u == (position / {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        for (j = 0u; j < {0}_DM_CHAR_WIDTH; j++)", m_instanceName);
            writer.WriteLine("        {");
            writer.WriteLine("            for (i = 0u; i < {0}_DM_CHAR_HEIGHT; i++)", m_instanceName);
            writer.WriteLine("            {");
            writer.WriteLine("                (void){0}_WritePixel({1}_disp{2}[position][j + (i * {3}_DM_CHAR_WIDTH)],", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("                (((uint32)((uint32){0}_charDotMatrix[character][j] >> i)) & {1}_PIXEL_STATE_ON));", m_instanceName, m_instanceName);
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }
        void WriteStringDotMatrix_n(TextWriter writer, TextWriter writer_h, int index)
        {
            writer_h.WriteLine("void    {0}_WriteStringDotMatrix_{1}(uint8 const character[], uint32 position);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_WriteStringDotMatrix_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays a null terminated character string on an array of");
            writer.WriteLine("*  dot-matrix alphanumeric character display elements. The customizer Display");
            writer.WriteLine("*  Helpers facility must be used to define the pixel set associated with the");
            writer.WriteLine("*  dot-matrix display elements. Multiple dot-matrix alphanumeric display element");
            writer.WriteLine("*  groups can be defined in the frame buffer and are addressed through the");
            writer.WriteLine("*  suffix (n) in the function name. This function is only included if a");
            writer.WriteLine("*  dot-matrix display element is defined in the component customizer.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  character: Pointer to the null terminated character string.");
            writer.WriteLine("*  position:  The Position of the first character as counted left to right");
            writer.WriteLine("*             starting at 0 on the left. If the defined display contains fewer");
            writer.WriteLine("*             characters then the string requires for display, the extra");
            writer.WriteLine("*             characters will not be displayed.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_WriteStringDotMatrix_{1}(uint8 const character[], uint32 position)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine("    uint8 ch;");
            writer.WriteLine("    uint32 i = 0u;");
            writer.WriteLine("");
            writer.WriteLine("    ch = character[0u];");
            writer.WriteLine("");
            writer.WriteLine("    while (((uint8)\'\\0\' != ch) && (position  != {0}_DIGIT_NUM_{1}))", m_instanceName, index);
            writer.WriteLine("    {");
            writer.WriteLine("        {0}_PutCharDotMatrix_{1}(ch, position);", m_instanceName, index);
            writer.WriteLine("        i++;");
            writer.WriteLine("        ch = character[i];");
            writer.WriteLine("        position++;");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }

        #endregion

        #region WriteBargraph_n
        void WriteBargraph_n(TextWriter writer, TextWriter writer_h, int index, int maxNumber, string varDecl)
        {
            writer_h.WriteLine("void    {0}_WriteBargraph_{1}(uint32 location, int32 mode);", m_instanceName, index);

            writer.WriteLine("/*******************************************************************************");
            writer.WriteLine("* Function Name: {0}_WriteBargraph_{1}", m_instanceName, index);
            writer.WriteLine("********************************************************************************");
            writer.WriteLine("*");
            writer.WriteLine("* Summary:");
            writer.WriteLine("*  This function displays an 8-bit integer location on a 1 to 255 segment");
            writer.WriteLine("*  bar-graph (numbered left to right). The bar graph may be any user defined");
            writer.WriteLine("*  size between 1 and 255 The bar graph may be any user defined created in a");
            writer.WriteLine("*  circle to display rotary position. The user defines what portion of the");
            writer.WriteLine("*  displays segments make up the bar-graph portion. Multiple, separate bargraph");
            writer.WriteLine("*  displays can be created in the frame buffer and are addressed through the");
            writer.WriteLine("*  index (n) in the function name.");
            writer.WriteLine("*");
            writer.WriteLine("* Parameters:");
            writer.WriteLine("*  location: Unsigned integer Location to be displayed. Valid values are from");
            writer.WriteLine("*            zero to the number of segments in the bar graph. A zero value");
            writer.WriteLine("*            turns all bar graph elements off. Values greater than the number");
            writer.WriteLine("*            of segments in the bar graph result in all elements on.");
            writer.WriteLine("*  mode:     Sets the bar graph display mode.");
            writer.WriteLine("*    Value    Description");
            writer.WriteLine("*     0        The specified Location segment is turned on.");
            writer.WriteLine("*     1        The Location segment and all segments to the left are turned on.");
            writer.WriteLine("*     -1       The Location segment and all segments to the right are turned on.");
            writer.WriteLine("*     2 to 10  Display the Location segment and 2 to 10 segments to the right.");
            writer.WriteLine("*              This mode can be used to create wide indicators.");
            writer.WriteLine("*");
            writer.WriteLine("* Return: ");
            writer.WriteLine("*  None.");
            writer.WriteLine("*");
            writer.WriteLine("* Global variables:");
            writer.WriteLine("*  {0}_DIGIT_NUM_{1} - holds the maximum digit number for the helper.", m_instanceName, index);
            writer.WriteLine("*");
            writer.WriteLine("*******************************************************************************/");
            writer.WriteLine("void {0}_WriteBargraph_{1}(uint32 location, int32 mode)", m_instanceName, index);
            writer.WriteLine("{");
            writer.WriteLine(varDecl);
            writer.WriteLine("    uint32 i;");
            writer.WriteLine("    uint32 maxValue = {0}_DIGIT_NUM_{1};", m_instanceName, index);
            writer.WriteLine("    uint32 locationInt = location;");
            writer.WriteLine("    int32 modeInt = mode;");
            writer.WriteLine("");
            writer.WriteLine("    if (locationInt != 0u)");
            writer.WriteLine("    {");
            writer.WriteLine("        /* If location greater then the number of elements in bar graph then");
            writer.WriteLine("        set location to a maxvalue and set mode to -1.");
            writer.WriteLine("        */");
            writer.WriteLine("        if (locationInt > maxValue)");
            writer.WriteLine("        {");
            writer.WriteLine("            locationInt = 1u;");
            writer.WriteLine("            modeInt = -1;");
            writer.WriteLine("        }");
            writer.WriteLine("        ");
            writer.WriteLine("        for(i = 1u; i <= ((uint32){0}_DIGIT_NUM_{1}); i++) ", m_instanceName, index);
            writer.WriteLine("        {");
            writer.WriteLine("            (void){0}_WritePixel({1}_disp{2}[i][0u], {3}_PIXEL_STATE_OFF);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("        }");
            writer.WriteLine("        ");
            writer.WriteLine("        switch (modeInt)");
            writer.WriteLine("        {");
            writer.WriteLine("            case 0:");
            writer.WriteLine("                (void){0}_WritePixel({1}_disp{2}[locationInt][0u], {3}_PIXEL_STATE_ON);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("                break;");
            writer.WriteLine("            case 1:");
            writer.WriteLine("                for(i = locationInt; i >= 1u; i--) ");
            writer.WriteLine("                {");
            writer.WriteLine("                    (void){0}_WritePixel({1}_disp{2}[i][0u], {3}_PIXEL_STATE_ON);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("                }");
            writer.WriteLine("                break;");
            writer.WriteLine("            case -1:");
            writer.WriteLine("                for(i = locationInt; i <= maxValue; i++) ");
            writer.WriteLine("                {");
            writer.WriteLine("                    (void){0}_WritePixel({1}_disp{2}[i][0u], {3}_PIXEL_STATE_ON);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("                }");
            writer.WriteLine("                break;");
            writer.WriteLine("            case 2:");
            writer.WriteLine("            case 3:");
            writer.WriteLine("            case 4:");
            writer.WriteLine("            case 5:");
            writer.WriteLine("            case 6:");
            writer.WriteLine("            case 7:");
            writer.WriteLine("            case 8:");
            writer.WriteLine("            case 9:");
            writer.WriteLine("            case 10:");
            writer.WriteLine("                #if ({0}_DIGIT_NUM_{1} > 1u) /* Doesn't make sense for bar graph with size less than 2 */", m_instanceName, index);
            writer.WriteLine("                    if (((locationInt + ((uint32)modeInt)) - 1u) <= maxValue) ");
            writer.WriteLine("                    {");
            writer.WriteLine("                        maxValue = (locationInt + ((uint32)modeInt)) - 1u;");
            writer.WriteLine("                    }");
            writer.WriteLine("                #endif /* {0}_DIGIT_NUM_{1} > 1u */", m_instanceName, index);
            writer.WriteLine("                for (i = locationInt; i <= maxValue; i++) ");
            writer.WriteLine("                {");
            writer.WriteLine("                    (void){0}_WritePixel({1}_disp{2}[i][0u], {3}_PIXEL_STATE_ON);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("                }");
            writer.WriteLine("                break;");
            writer.WriteLine("            default:");
            writer.WriteLine("                break;");
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("    else ");
            writer.WriteLine("    {");
            writer.WriteLine("        for (i = 1u; i <= maxValue; i++) ");
            writer.WriteLine("        {");
            writer.WriteLine("            (void){0}_WritePixel({1}_disp{2}[i][0u], {3}_PIXEL_STATE_OFF);", m_instanceName, m_instanceName, index, m_instanceName);
            writer.WriteLine("        }");
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("");
        }

        #endregion

        #region Helpers kind define
        void WriteHelperKindDefine(TextWriter writer_h, CyHelperKind kind)
        {
            string insertText = "";
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    insertText = "7SEG";
                    break;
                case CyHelperKind.SEGMENT_14:
                    insertText = "14SEG";
                    break;
                case CyHelperKind.SEGMENT_16:
                    insertText = "16SEG";
                    break;
                case CyHelperKind.MATRIX:
                    insertText = "DOT_MATRIX";
                    break;
                default:
                    break;
            }
            writer_h.WriteLine("#define {0}_{1}", m_instanceName, insertText);
        }
        #endregion Helpers kind define

    }
}
