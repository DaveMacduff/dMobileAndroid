namespace dMobile.Bedrock
{
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public interface IAppArea
    {
        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        Task ScannerDisable();

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        Task ScannerEnable();

        /// <summary>
        /// Indicates whether the underlying control is in a state where the 
        /// underlying control contains uncommited changes or can be safely closed.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<bool> IsReadyToClose();

        /// <summary>
        /// Get the control to gain focus when this control is closed. Returns nothing if not 
        /// applicable.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        //UserControl GetCallingControl();

        /// <summary>
        /// Gets the controls description for display purposes within the window menu.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        string GetControlDescription();

        /// <summary>
        /// Used to perform saving of data changes.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<bool> SaveChanges();

        /// <summary>
        /// Used to cancel data changes.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<bool> CancelChanges();

        /// <summary>
        /// Determine if data is ready to be saved with validation code.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<bool> IsReadyToSave();

        /// <summary>
        /// The method should return the complex structure that needs to be passed between areas 
        /// of the application.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<XDocument> GetXML();
    }
}
