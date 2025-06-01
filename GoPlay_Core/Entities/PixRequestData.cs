namespace GoPlay_Core.Entities
{
    public class PixRequestData
    {
        public Calendario calendario { get; set; } = new Calendario();
        public Devedor devedor { get; set; } = new Devedor();
        public Valor valor { get; set; } = new Valor();
        public string chave { get; set; } = string.Empty;
        public string solicitacaoPagador { get; set; } = "Pagamento da inscrição GoPlay";
    }

    public class Calendario
    {
        public int expiracao { get; set; } = 3600;
    }

    public class Devedor
    {
        public string cpf { get; set; } = string.Empty;
        public string nome { get; set; } = string.Empty;
    }

    public class Valor
    {
        public string original { get; set; } = string.Empty;
    }
}
