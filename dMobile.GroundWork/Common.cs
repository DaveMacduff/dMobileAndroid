// <copyright file="Common.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
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
            var isDataExist = true;

            if (unitCount == 0 || prodCount == 0 || schemeCount == 0 || whsCount == 0 || whsLocMapTypeCount == 0)
            {
                isDataExist = false;
            }

            return isDataExist;
        }

        /// <summary>
        /// Get the ground work assembly version information.
        /// </summary>
        /// <returns><see cref="string"/> Assembly version information.</returns>
        public string GetAssemblyVersion()
        {
            var major = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }
    }
}
