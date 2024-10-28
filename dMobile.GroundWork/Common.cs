// <copyright file="Common.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Bedrock;

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Common" /> Ground work common library.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Check if there is existing data in the database.
        /// </summary>
        /// <returns><see cref="bool"/> True if existing data is found else false.</returns>
        public static async Task<bool> IsDataBaseDataExist()
        {
            var prodCount = await new TableProduct().DataStatus();
            var schemeCount = await new TableProductScheme().DataStatus();
            var whsCount = await new TableLocationMapWarehouse().DataStatus();
            var unitCount = await new TableUnit().DataStatus();
            var whsLocMapTypeCount = await new TableWarehouseLocationMapType().DataStatus();
            var isDataExist = !(unitCount == 0 ||
                prodCount == 0 ||
                schemeCount == 0 ||
                whsCount == 0 ||
                whsLocMapTypeCount == 0);
            return isDataExist;
        }

        /// <summary>
        /// Convert enum SysCommon.ActSubTypes value to sub type string.
        /// </summary>
        /// <param name="subType">SysCommon.ActSubTypes enum value.</param>
        /// <returns>Status name string.</returns>
        public static string SubTypeEnum2String(SysCommon.ActSubTypes subType)
        {
            return Enum.GetName(typeof(SysCommon.ActSubTypes), subType);
        }

        /// <summary>
        /// Convert status name string to enum SysCommon.ActSubTypes value.
        /// </summary>
        /// <param name="subType">Sub type name string.</param>
        /// <returns>SysCommon.ActSubTypes enum.</returns>
        public static SysCommon.ActSubTypes TypeString2SubTypeEnum(string subType)
        {
            return (SysCommon.ActSubTypes)Enum.Parse(typeof(SysCommon.ActSubTypes), subType);
        }

        /// <summary>
        /// Get the groundwork assembly version information.
        /// </summary>
        /// <returns><see cref="string"/> Assembly version information.</returns>
        public string GetAssemblyVersion()
        {
            var major = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}
