namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IStandingsService
    {
        //Keyword o palabra reservada "object"
        //se utiliza para indicar que el método puede devolver
        //cualquier tipo de dato. En este caso, se espera que el
        //método devuelva un objeto que contenga la información de
        //la tabla de posiciones, los máximos goleadores o las estadísticas de tarjetas,
        //dependiendo del método específico.
        Task<object> GetStandingsAsync(int tournamentId); // Obtener la tabla de posiciones para un torneo específico
        Task<object> GetTopScorersAsync(int tournamentId); // Obtener la lista de los máximos goleadores para un torneo específico
        Task<object> GetCardStatsAsync(int tournamentId); // Obtener estadísticas de tarjetas para jugadores en un torneo específico
    }
}
