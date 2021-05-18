using System.Threading.Tasks;
/// <summary>
///
/// </summary>
/// <remarks></remarks>
namespace dMobile.GroundWork
{
    public class Common
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
