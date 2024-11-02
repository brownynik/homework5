using ShopMonitorService.ProductProcessingData;

namespace OrchService.ExternalServices
{
    /// <summary>
    /// Взаимодействие с сервисом мониторинга продуктов (на сайтах продавца).
    /// </summary>
    public class ShopMonitorServiceProvide
    {
        private static ShopMonitorServiceProvide? monitorSrv;
        private static readonly object o = new object();
        static HttpClient client = new HttpClient();

        public ShopMonitorServiceProvide(string url)
            => client.BaseAddress = new Uri(url);

        /// <summary>
        /// Синглетон. Обеспечиваем единственный экземпляр сервиса мониторинга.
        /// </summary>
        /// <param name="url">url сервиса мониторинга.</param>
        /// <returns>Экземпляр сервиса оркестрации.</returns>
        public static ShopMonitorServiceProvide GetSingleSrv(string url)
        {
            if (monitorSrv == null)
            {
                lock (o)
                {
                    if (monitorSrv == null)
                    {
                        monitorSrv = new ShopMonitorServiceProvide(url);
                    }
                }
            }
            return monitorSrv;
        }

        /// <summary>
        /// Обновление процессинга.
        /// </summary>
        /// <param name="productProcessing">Именованная процессинговая очередь обработки продукта.</param>
        public async void SetProductProcessing(ProductProcessing processing)
        {
            await client.PostAsJsonAsync("api/runprocessing", processing);
        }
    }
}
