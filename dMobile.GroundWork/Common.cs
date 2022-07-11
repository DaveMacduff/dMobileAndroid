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
            int prodCount = await new TableProduct().DataStatus();
            int schemeCount = await new TableProductScheme().DataStatus();
            int whsCount = await new TableLocationMapWarehouse().DataStatus();
            int unitCount = await new TableUnit().DataStatus();
            int whsLocMapTypeCount = await new TableWarehouseLocationMapType().DataStatus();
            bool isDataExist = true;

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
            int major = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
            int revision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }
    }
}
