using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.DocumentManagement.Services
{
    /// <summary>
    /// SharePoint Document Management Service Interface
    /// </summary>
    public interface ISharePointManagementService
    {
        /// <summary>
        /// Copies file to Share Point and provides a link to the newly created file
        /// </summary>
        /// <param name="fileName">Name of the file to be copied in SharePoint</param>
        /// <returns>Link to file in SharePoint</returns>
        string CopyFileToSharePoint(string fileName);
    }
}
