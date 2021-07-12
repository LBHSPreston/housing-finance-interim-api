using HousingFinanceInterimApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways.Interface
{

    /// <summary>
    /// The UP cash file name gateway interface.
    /// </summary>
    public interface IUPCashFileNameGateway
    {

        /// <summary>
        /// Gets the given file by the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>An instance of <see cref="UPCashDumpFileName"/> or null if no record found.</returns>
        public Task<UPCashDumpFileName> GetAsync(string fileName);

        /// <summary>
        /// Creates a UP Cash dump file name entry for the given file name asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <returns>The created instance of <see cref="UPCashDumpFileName"/></returns>
        public Task<UPCashDumpFileName> CreateAsync(string fileName, bool isSuccess = false);

        /// <summary>
        /// Sets the given file name entry to success asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>A bool determining the success of the method.</returns>
        public Task<bool> SetToSuccessAsync(long fileId);

    }

}
