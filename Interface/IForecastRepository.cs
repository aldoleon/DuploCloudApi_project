using DuploCloudApi.Models;

namespace DuploCloudApi.Interface
{
    public interface IForecastRepository
    {
        Task <IList<Forecast>> GetForecasts();
        Task<Forecast?> GetForecastById(long id);
        Task<Forecast?> AddForecast(ForecastDTO forecast);
        Task<bool> DeleteForecast(long id);
        Task<bool> UpdateForecast(long id, Forecast forecast);
    }
}
