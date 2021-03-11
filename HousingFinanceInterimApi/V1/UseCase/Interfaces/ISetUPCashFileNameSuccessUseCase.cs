using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.UseCase.Interfaces
{

    /// <summary>
    /// The set UP cash file name success use case interface.
    /// </summary>
    public interface ISetUPCashFileNameSuccessUseCase
    {

        /// <summary>
        /// Executes the instance asynchronous.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public Task ExecuteAsync(long fileId);

    }

}
