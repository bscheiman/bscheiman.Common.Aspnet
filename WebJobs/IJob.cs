#region
using System.Threading.Tasks;

#endregion

namespace bscheiman.Common.Aspnet.WebJobs {
    public interface IWebJob {
        string Descrition { get; }
        double Priority { get; }
        Task Update();
    }
}