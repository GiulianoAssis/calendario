using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalendarioApp.ViewModels;

public partial class CalendarioViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime mesAno = DateTime.Now;

    [ObservableProperty]
    private string mesAnoString;

    [ObservableProperty]
    private ObservableCollection<CalendarioDia> diasDoMes = new();

    [ObservableProperty]
    private ObservableCollection<Evento> eventosDia = new();

    [ObservableProperty]
    private DateTime? diaSelecionado;

    public ICommand ComandoMesAnterior { get; }
    public ICommand ComandoMesProximo { get; }
    public ICommand ComandoAdicionarEvento { get; }

    public CalendarioViewModel()
    {
        ComandoMesAnterior = new RelayCommand(MesAnterior);
        ComandoMesProximo = new RelayCommand(MesProximo);
        ComandoAdicionarEvento = new RelayCommand(AdicionarEvento);

        AtualizarMesAnoString();
        GerarDiasDoMes();
        SelecionarDia(DateTime.Now);
    }

    private void MesAnterior()
    {
        MesAno = MesAno.AddMonths(-1);
        AtualizarMesAnoString();
        GerarDiasDoMes();
        if (DiaSelecionado?.Month != MesAno.Month) SelecionarDia(MesAno);
    }

    private void MesProximo()
    {
        MesAno = MesAno.AddMonths(1);
        AtualizarMesAnoString();
        GerarDiasDoMes();
        if (DiaSelecionado?.Month != MesAno.Month) SelecionarDia(MesAno);
    }

    private void AtualizarMesAnoString()
    {
        MesAnoString = MesAno.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("pt-BR"));
    }

    private void GerarDiasDoMes()
    {
        DiasDoMes.Clear();
        var primeiroDia = new DateTime(MesAno.Year, MesAno.Month, 1);
        var diasNaSemana = (int)primeiroDia.DayOfWeek;
        var ultimoDia = DateTime.DaysInMonth(MesAno.Year, MesAno.Month);

        // Células vazias antes do 1º dia
        for (int i = 0; i < diasNaSemana; i++)
        {
            DiasDoMes.Add(new CalendarioDia { Data = null, EPreenchido = false });
        }

        // Dias do mês
        for (int dia = 1; dia <= ultimoDia; dia++)
        {
            var data = new DateTime(MesAno.Year, MesAno.Month, dia);
            var temEvento = TemEventoNoDia(data);
            DiasDoMes.Add(new CalendarioDia
            {
                Data = data,
                NumeroDia = dia.ToString(),
                TemEvento = temEvento,
                EPreenchido = true,
                EHoje = data.Date == DateTime.Today,
                ESelecionado = data.Date == DiaSelecionado?.Date
            });
        }

        // Preenche até 42 células (6 semanas)
        while (DiasDoMes.Count < 42)
        {
            DiasDoMes.Add(new CalendarioDia { Data = null, EPreenchido = false });
        }
    }

    private void SelecionarDia(DateTime data)
    {
        DiaSelecionado = data;
        CarregarEventosDoDia(data);
    }

    private void CarregarEventosDoDia(DateTime data)
    {
        EventosDia.Clear();
        if (data.Day == 21 && data.Month == 10 && data.Year == 2025)
        {
            EventosDia.Add(new Evento { NomeEvento = "Painelão do Norte", HorarioEvento = "08:30 - 17:30" });
        }
        else
        {
            EventosDia.Add(new Evento { NomeEvento = "Nenhum evento marcado", HorarioEvento = "" });
        }
    }

    private bool TemEventoNoDia(DateTime data)
    {
        return data.Day % 5 == 0;
    }

    private async void AdicionarEvento()
    {
        var nome = await Application.Current.MainPage.DisplayPromptAsync("Novo Evento", "Nome do Evento:", "OK", "Cancelar");
        if (!string.IsNullOrEmpty(nome))
        {
            // Simulado: Adiciona evento no dia atual
            EventosDia.Add(new Evento { NomeEvento = nome, HorarioEvento = "09:00 - 17:00" });
            GerarDiasDoMes(); // Refresh bolinha
        }
    }
}

public class CalendarioDia : ObservableObject
{
    public DateTime? Data { get; set; }
    public string NumeroDia { get; set; } = "";
    public bool EPreenchido { get; set; }
    public bool TemEvento { get; set; }
    public bool EHoje { get; set; }
    public bool ESelecionado { get; set; }

    [ObservableProperty]
    private Color corFundo = Colors.White;

    partial void OnESelecionadoChanged(bool value)
    {
        CorFundo = value ? Color.FromArgb("#42046582") : (EHoje ? Colors.LightBlue : Colors.White);
    }
}

public class Evento
{
    public string NomeEvento { get; set; } = "";
    public string HorarioEvento { get; set; } = "";
}