using PainelPlenario.Core.Enums;
using PainelPlenario.Core.Models;

namespace PainelPlenario.Core.Services
{
    public class PlenarioStateService
    {
        // EVENTO C#: Sempre que algo mudar (voto, presença, tela), este evento avisa o Telão
        public event Action? OnMudancaEstado;
        public Sessao? SessaoAtiva { get; private set; }
        public List<Parlamentar> Parlamentares { get; private set; } = new();
        public List<Parlamentar> ParlamentaresAtivos { get; private set; } = new();
        public ModoTelao ModoAtual { get; private set; } = ModoTelao.AguardandoInicio;
        public ItemPauta? MateriaAtual { get; private set; }
        public Parlamentar? OradorAtual { get; private set; }
        public Parlamentar? ApartanteAtual { get; private set; }
        public int TempoRestanteSegundos { get; private set; } = 0;
        public bool CronometroRodando { get; private set; } = false;
        private System.Timers.Timer? _timer;
        public Dictionary<Guid, TipoVoto> VotosMatériaAtual { get; private set; } = new();
        // Guarda o veredito oficial proclamado pelo Presidente
        public ResultadoVotacao? ResultadoAtual { get; private set; }
        public ConfiguracaoCamara Configuracao { get; set; } = new();

        // -------------------------------------------------------------
        // MÉTODOS DE CONTROLE (Chamados pelos botões do Operador)
        // -------------------------------------------------------------

        public void DefinirSessao(Sessao sessao, List<Parlamentar> parlamentares)
        {
            SessaoAtiva = sessao;
            ParlamentaresAtivos = parlamentares.Where(p => p.Ativo).ToList();
            Parlamentares = parlamentares;
            Notificar();
        }
        public void MudarModoTelao(ModoTelao novoModo)
        {
            ModoAtual = novoModo;
            Notificar();
        }
        public void DefinirMateriaAtual(ItemPauta materia)
        {
            MateriaAtual = materia;
            VotosMatériaAtual.Clear(); // Zera os votos para a nova matéria
            Notificar();
        }
        public void RegistrarVoto(Guid parlamentarId, TipoVoto voto)
        {
            VotosMatériaAtual[parlamentarId] = voto;
            Notificar();
        }
        // Atalho: Marca "SIM" para todos os vereadores presentes
        public void VotacaoSimbolicaAprovarTodos()
        {
            if (SessaoAtiva == null) return;
            foreach (var p in ParlamentaresAtivos)
            {
                var presenca = SessaoAtiva.Presencas.FirstOrDefault(pr => pr.ParlamentarId == p.Id);
                if (presenca != null && presenca.Presente)
                {
                    VotosMatériaAtual[p.Id] = TipoVoto.Sim;
                }
            }
            Notificar();
        }

        // -------------------------------------------------------------
        // CONFIGURAÇÕES
        // -------------------------------------------------------------

        // Atualiza a configuração institucional e notifica todas as telas
        public void DefinirConfiguracao(ConfiguracaoCamara config)
        {
            Configuracao = config;
            Notificar();
        }

        // Atualiza a lista de parlamentares na memória quando um vereador for adicionado ou editado
        public void AtualizarParlamentares(List<Parlamentar> parlamentares)
        {
            ParlamentaresAtivos = parlamentares.Where(p => p.Ativo).ToList();
            Parlamentares = parlamentares;
            Notificar();
        }

        // -------------------------------------------------------------
        // CONTROLES DE PRESENÇA
        // -------------------------------------------------------------

        public void AlternarPresenca(Guid parlamentarId)
        {
            if (SessaoAtiva == null) return;
            var presenca = SessaoAtiva.Presencas.FirstOrDefault(p => p.ParlamentarId == parlamentarId);
            if (presenca == null)
            {
                presenca = new RegistroPresenca { SessaoId = SessaoAtiva.Id, ParlamentarId = parlamentarId, Presente = true };
                SessaoAtiva.Presencas.Add(presenca);
            }
            else
            {
                presenca.Presente = !presenca.Presente;
            }
            Notificar();
        }
        public int TotalPresentes => SessaoAtiva?.Presencas.Count(p => p.Presente) ?? 0;

        // -------------------------------------------------------------
        // CONTADORES DE VOTOS EM TEMPO REAL
        // -------------------------------------------------------------
        public int TotalSim => VotosMatériaAtual.Values.Count(v => v == TipoVoto.Sim);
        public int TotalNao => VotosMatériaAtual.Values.Count(v => v == TipoVoto.Nao);
        public int TotalAbstencao => VotosMatériaAtual.Values.Count(v => v == TipoVoto.Abstencao);
        public int TotalVotantes => TotalSim + TotalNao + TotalAbstencao;

        // -------------------------------------------------------------
        // CONTROLES DO CRONÔMETRO DE DISCURSO
        // -------------------------------------------------------------
        public void IniciarCronometro(int segundos, Parlamentar? orador = null)
        {
            PararTimerInterno();
            TempoRestanteSegundos = segundos;

            if (orador != null)
                OradorAtual = orador;

            CronometroRodando = true;
            // Configura o relógio interno para disparar a cada 1 segundo (1000ms)
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (sender, e) =>
            {
                if (TempoRestanteSegundos > 0)
                {
                    TempoRestanteSegundos--;
                    Notificar();
                }
                else
                {
                    CronometroRodando = false;
                    PararTimerInterno();
                    Notificar();
                }
            };
            _timer.Start();
            Notificar();
        }
        public void PausarOuRetomarCronometro()
        {
            if (_timer == null) return;
            if (CronometroRodando)
            {
                _timer.Stop();
                CronometroRodando = false;
            }
            else
            {
                _timer.Start();
                CronometroRodando = true;
            }
            Notificar();
        }
        public void AdicionarTempo(int segundosAdicionais)
        {
            TempoRestanteSegundos += segundosAdicionais;
            Notificar();
        }

        public void SubtrairTempo(int segundosAdicionais)
        {
            TempoRestanteSegundos -= segundosAdicionais;
            if (TempoRestanteSegundos <= 0)
                TempoRestanteSegundos += segundosAdicionais; //volta ao que estava
            Notificar();
        }
        public void ZerarCronometro()
        {
            PararTimerInterno();
            TempoRestanteSegundos = 0;
            CronometroRodando = false;
            Notificar();
        }
        private void PararTimerInterno()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        public void DefinirOrador(Parlamentar? orador)
        {
            OradorAtual = orador;
            Notificar();
        }

        public void DefinirApartante(Parlamentar? apartante)
        {
            ApartanteAtual = apartante;
            Notificar();
        }
        public void LimparApartante()
        {
            ApartanteAtual = null;
            Notificar();
        }
        public void LimparOrador()
        {
            OradorAtual = null;
            ApartanteAtual = null;
            Notificar();
        }

        // -------------------------------------------------------------
        // CONTROLES DO RESULTADO DA VOTAÇÃO
        // -------------------------------------------------------------

        public void ProclamarResultado(ResultadoVotacao resultado)
        {
            ResultadoAtual = resultado;

            if (MateriaAtual != null)
            {
                MateriaAtual.Status = StatusItemPauta.Concluido;
                MateriaAtual.Resultado = resultado;
            }

            ModoAtual = ModoTelao.ResultadoProclamado;
            Notificar();
        }

        public void ReabrirVotacao()
        {
            ResultadoAtual = null;
            if (MateriaAtual != null)
            {
                MateriaAtual.Status = StatusItemPauta.EmVotacao;
                MateriaAtual.Resultado = null;
            }
            ModoAtual = ModoTelao.VotacaoNominal;
            Notificar();
        }

        // -------------------------------------------------------------
        // GESTÃO DE PAUTA
        // -------------------------------------------------------------

        public void AdicionarMateria(string tipo, string numero, string autor, string ementa)
        {
            if (SessaoAtiva == null) return;

            var novaOrdem = SessaoAtiva.ItensPauta.Count + 1;
            var materia = new ItemPauta
            {
                SessaoId = SessaoAtiva.Id,
                Ordem = novaOrdem,
                Tipo = tipo,
                Numero = numero,
                Autor = autor,
                Ementa = ementa,
                Status = StatusItemPauta.Pendente
            };

            SessaoAtiva.ItensPauta.Add(materia);
            Notificar();
        }

        public void IniciarVotacaoMateria(ItemPauta materia)
        {
            MateriaAtual = materia;
            MateriaAtual.Status = StatusItemPauta.EmVotacao;
            ResultadoAtual = null;
            VotosMatériaAtual.Clear(); // Limpa os votos anteriores
            ModoAtual = ModoTelao.VotacaoNominal;
            Notificar();
        }

        // Dispara a atualização para as telas
        private void Notificar()
        {
            OnMudancaEstado?.Invoke();
        }
    }
}
