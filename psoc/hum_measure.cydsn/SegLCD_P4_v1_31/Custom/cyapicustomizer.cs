/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CyDesigner.Extensions.Common;
using CyDesigner.Extensions.Gde;

namespace SegLCD_P4_v1_31
{
    public partial class CyCustomizer :ICyAPICustomize_v2
    {
        #region ICyAPICustomize_v2 Members

        public IEnumerable<CyAPICustomizer> CustomizeAPIs(ICyAPICustomizeArgs_v2 args,
                                                          IEnumerable<CyAPICustomizer> apis)
        {
            List<CyAPICustomizer> customizers = new List<CyAPICustomizer>(apis);
            ICyInstQuery_v1 instQuery = args.InstQuery;
            Dictionary<string, string> paramDict = new Dictionary<string, string>();

            CyLCDParameters parameters = new CyLCDParameters(instQuery);
            if (customizers.Count > 0)
            {
                paramDict = customizers[0].MacroDictionary;
            }

            CyAPIGenerator apiGen = new CyAPIGenerator(parameters, instQuery.GetCommittedParam("INSTANCE_NAME").Value);

            apiGen.CollectApiCore(instQuery, parameters.m_helpersConfig, ref paramDict, parameters.NumCommonLines,
                                  parameters.NumSegmentLines);
            apiGen.CollectApiHeader(args, parameters.m_helpersConfig, ref paramDict, parameters.NumCommonLines,
                                    parameters.NumSegmentLines);
            apiGen.CollectDivParams(ref paramDict, parameters);
            apiGen.CollectContrastDefines(ref paramDict, args.TermQuery, parameters);
            apiGen.CollectGeneralAPIs(ref paramDict, args.TermQuery, parameters);

            for (int i = 0; i < customizers.Count; i++)
            {
                customizers[i].MacroDictionary = paramDict;
            }
            return customizers;
        }
        #endregion
    }
}
