using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppMeteo
{
    public class ServicoMeteo
    {
        private readonly HttpClient _httpClient;

        public ServicoMeteo()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        // Geocoding: converte nome de cidade em coordenadas
        public async Task<(double lat, double lon, string nomeCidade)> ObterCoordenadas(string cidade)
        {
            string url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(cidade)}&count=1&language=pt&format=json";
            var resposta = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(resposta);

            var resultados = json["results"];
            if (resultados == null || !resultados.HasValues)
                throw new Exception($"Cidade '{cidade}' não encontrada.");

            double lat = (double)resultados[0]["latitude"];
            double lon = (double)resultados[0]["longitude"];
            string nome = resultados[0]["name"]?.ToString() ?? cidade;
            string pais = resultados[0]["country"]?.ToString() ?? "";
            return (lat, lon, $"{nome}, {pais}");
        }

        // Busca dados meteorológicos completos
        public async Task<DadosMeteo> ObterMeteo(string cidade)
        {
            var (lat, lon, nomeCidade) = await ObterCoordenadas(cidade);

            string url = $"https://api.open-meteo.com/v1/forecast?" +
                         $"latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                         $"&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                         $"&current=temperature_2m,relative_humidity_2m,wind_speed_10m,wind_direction_10m,weather_code" +
                         $"&daily=temperature_2m_max,temperature_2m_min,weather_code" +
                         $"&timezone=auto&forecast_days=7";

            var resposta = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(resposta);

            var atual = json["current"];
            double temp = (double)atual["temperature_2m"];
            int humidade = (int)atual["relative_humidity_2m"];
            double vento = (double)atual["wind_speed_10m"];
            double dirVento = (double)atual["wind_direction_10m"];
            int codigoTempo = (int)atual["weather_code"];

            var daily = json["daily"];
            var datas = daily["time"].ToObject<List<string>>();
            var tempsMax = daily["temperature_2m_max"].ToObject<List<double>>();
            var tempsMin = daily["temperature_2m_min"].ToObject<List<double>>();
            var codigosDia = daily["weather_code"].ToObject<List<int>>();

            var previsao = new List<PrevisaoDia>();
            for (int i = 0; i < datas.Count; i++)
            {
                var data = DateTime.Parse(datas[i]);
                previsao.Add(new PrevisaoDia
                {
                    Data = data.ToString("dd/MM"),
                    DiaSemana = ObterDiaSemana(data.DayOfWeek),
                    TempMax = tempsMax[i],
                    TempMin = tempsMin[i],
                    CodigoTempo = codigosDia[i],
                    IconeTempo = ObterIcone(codigosDia[i]),
                    Descricao = ObterDescricao(codigosDia[i])
                });
            }

            return new DadosMeteo
            {
                Temperatura = temp,
                TemperaturaMax = tempsMax[0],
                TemperaturaMin = tempsMin[0],
                Humidade = humidade,
                VelocidadeVento = vento,
                DirecaoVento = ObterDirecaoVento(dirVento),
                CodigoTempo = codigoTempo,
                DescricaoTempo = ObterDescricao(codigoTempo),
                IconeTempo = ObterIcone(codigoTempo),
                Cidade = nomeCidade,
                Previsao = previsao
            };
        }

        private string ObterDiaSemana(DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Monday: return "Seg";
                case DayOfWeek.Tuesday: return "Ter";
                case DayOfWeek.Wednesday: return "Qua";
                case DayOfWeek.Thursday: return "Qui";
                case DayOfWeek.Friday: return "Sex";
                case DayOfWeek.Saturday: return "Sáb";
                case DayOfWeek.Sunday: return "Dom";
                default: return "";
            }
        }

        private string ObterDirecaoVento(double graus)
        {
            string[] direcoes = { "N", "NE", "E", "SE", "S", "SO", "O", "NO" };
            int idx = (int)Math.Round(graus / 45.0) % 8;
            return direcoes[idx];
        }

        public static string ObterIcone(int codigo)
        {
            if (codigo == 0) return "☀️";
            if (codigo <= 2) return "⛅";
            if (codigo == 3) return "☁️";
            if (codigo <= 49) return "🌫️";
            if (codigo <= 59) return "🌧️";
            if (codigo <= 69) return "🌧️";
            if (codigo <= 79) return "❄️";
            if (codigo <= 84) return "🌦️";
            if (codigo <= 94) return "⛈️";
            return "🌩️";
        }

        public static string ObterDescricao(int codigo)
        {
            if (codigo == 0) return "Céu limpo";
            if (codigo == 1) return "Principalmente limpo";
            if (codigo == 2) return "Parcialmente nublado";
            if (codigo == 3) return "Nublado";
            if (codigo <= 49) return "Nevoeiro";
            if (codigo <= 59) return "Chuva fraca";
            if (codigo <= 69) return "Chuva";
            if (codigo <= 79) return "Neve";
            if (codigo <= 84) return "Aguaceiros";
            if (codigo <= 94) return "Trovoada";
            return "Trovoada forte";
        }
    }
}
