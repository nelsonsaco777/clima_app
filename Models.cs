using System.Collections.Generic;

namespace AppMeteo
{
    public class DadosMeteo
    {
        public double Temperatura { get; set; }
        public double TemperaturaMax { get; set; }
        public double TemperaturaMin { get; set; }
        public int Humidade { get; set; }
        public double VelocidadeVento { get; set; }
        public string DirecaoVento { get; set; }
        public int CodigoTempo { get; set; }
        public string DescricaoTempo { get; set; }
        public string IconeTempo { get; set; }
        public string Cidade { get; set; }
        public List<PrevisaoDia> Previsao { get; set; }
    }

    public class PrevisaoDia
    {
        public string Data { get; set; }
        public string DiaSemana { get; set; }
        public double TempMax { get; set; }
        public double TempMin { get; set; }
        public int CodigoTempo { get; set; }
        public string IconeTempo { get; set; }
        public string Descricao { get; set; }
    }
}
