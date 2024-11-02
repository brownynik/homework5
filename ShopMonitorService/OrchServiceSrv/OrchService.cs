namespace ShopMonitorService.OrchServiceSrv
{
    /// <summary>
    /// Оркестрация процессингов по товарам.
    /// </summary>
    public class OrchService
    {
        static HttpClient client = new HttpClient();

        private static OrchService? srvInstance;
        
        // блокировка на момент создания экземпляра сервиса.
        private static readonly object o = new object();

        public OrchService(string url)
        {
            client.BaseAddress = new Uri(url);
        }
        
        /// <summary>
        /// Синглетон. Обеспечиваем единственный экземпляр оркестратора.
        /// </summary>
        /// <param name="url">url сервиса оркестрации.</param>
        /// <returns>Экземпляр сервиса оркестрации.</returns>
        public static OrchService GetSingleSrv(string url)
        {
            if (srvInstance == null)
            {
                lock (o)
                {
                    if (srvInstance == null)
                    {
                        srvInstance = new OrchService(url);
                    }
                }
            }
            return srvInstance;
        }

        public async void ReserveProductToUser(int userId)
        {
            await client.PostAsJsonAsync("api/reserve-product", userId);
        }
    }
}
