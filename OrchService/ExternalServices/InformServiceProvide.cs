
using InformService;

namespace OrchService.ExternalServices
{
    /// <summary>
    /// Взаимодействие с сервисом информирования/уведомления пользователей.
    /// </summary>
    public class InformServiceProvide
    {
        static HttpClient client = new HttpClient();

        private static InformServiceProvide? informSrv;
        private static readonly object o = new object();

        public InformServiceProvide(string url)
            => client.BaseAddress = new Uri(url);

        /// <summary>
        /// Синглетон. Обеспечиваем единственный экземпляр оркестратора.
        /// </summary>
        /// <param name="url">url сервиса оркестрации.</param>
        /// <returns>Экземпляр сервиса уведомления.</returns>
        public static InformServiceProvide GetSingleSrv(string url)
        {
            if (informSrv == null)
            {
                lock (o)
                {
                    if (informSrv == null)
                    {
                        informSrv = new InformServiceProvide(url);
                    }
                }
            }

            return informSrv;
        }

        /// <summary>
        /// Информирование о событии.
        /// </summary>
        /// <param name="msg">Информационное сообщение.</param>
        public async void publicateInformation(InformationMessage msg)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/msg", msg);
        }
    }

}
