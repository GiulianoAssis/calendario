namespace CalendarioApp.Views;

public partial class CalendarioPage : ContentPage
{
    public CalendarioPage()
    {
        InitializeComponent();
        BindingContext = new CalendarioApp.ViewModels.CalendarioViewModel();
        Loaded += CalendarioPage_Loaded;
    }

    private void CalendarioPage_Loaded(object sender, EventArgs e)
    {
        if (BindingContext is CalendarioApp.ViewModels.CalendarioViewModel vm)
        {
            PopularGridDias(vm.DiasDoMes);
        }
    }

    private void PopularGridDias(System.Collections.ObjectModel.ObservableCollection<CalendarioApp.ViewModels.CalendarioDia> dias)
    {
        DiasGrid.RowDefinitions.Clear();
        for (int i = 0; i < 6; i++)
        {
            DiasGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        DiasGrid.Children.Clear();
        int col = 0, row = 0;
        foreach (var dia in dias)
        {
            if (!dia.EPreenchido || dia.Data == null)
            {
                col++;
                if (col == 7) { col = 0; row++; }
                continue;
            }

            var labelDia = new Label
            {
                Text = dia.NumeroDia,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = dia.CorFundo,
                TextColor = dia.ESelecionado ? Colors.White : Colors.Black
            };

            if (dia.TemEvento)
            {
                var gridCelula = new Grid();
                gridCelula.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                gridCelula.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridCelula.Children.Add(labelDia, 0, 0);
                var bolinha = new Label { Text = "●", TextColor = Colors.Red, FontSize = 12, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End };
                gridCelula.Children.Add(bolinha, 1, 0);
                DiasGrid.Children.Add(gridCelula, col, row);
            }
            else
            {
                DiasGrid.Children.Add(labelDia, col, row);
            }

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, args) => SelecionarDia(dia.Data);
            labelDia.GestureRecognizers.Add(tap);

            col++;
            if (col == 7) { col = 0; row++; }
        }
    }

    private void SelecionarDia(DateTime? data)
    {
        if (data.HasValue && BindingContext is CalendarioApp.ViewModels.CalendarioViewModel vm)
        {
            vm.SelecionarDia(data.Value);
            PopularGridDias(vm.DiasDoMes);
        }
    }
}