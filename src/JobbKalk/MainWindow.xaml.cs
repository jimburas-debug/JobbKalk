using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JobbKalk.Core;

namespace JobbKalk;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, TextBox> _inputs = [];
    private CalculatorDefinition? _active;
    private CalculationResult? _lastResult;
    private readonly CultureInfo _norwegian = CultureInfo.GetCultureInfo("nb-NO");

    public MainWindow()
    {
        InitializeComponent();
        foreach (var category in CalculatorCatalog.Categories) CategoryList.Items.Add(category);
        CategoryList.SelectedIndex = 0;
        RenderCards();
    }

    private void RenderCards()
    {
        if (CalculatorCards is null || CategoryList is null) return;
        CalculatorCards.Children.Clear();
        var category = CategoryList.SelectedItem?.ToString() ?? "Alle";
        var query = SearchBox?.Text.Trim() ?? "";
        var shown = CalculatorCatalog.All.Where(c =>
            (category == "Alle" || c.Category == category) &&
            (query.Length == 0 || c.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
             c.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
             c.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase))).ToList();

        foreach (var calc in shown) CalculatorCards.Children.Add(CreateCard(calc));
        PageTitle.Text = category == "Alle" ? "Alle kalkulatorer" : category;
        PageSubtitle.Text = $"{shown.Count} praktiske verktøy · fungerer uten internett";
    }

    private Button CreateCard(CalculatorDefinition calc)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = calc.Category.ToUpper(_norwegian), Foreground = (Brush)FindResource("BrandBrush"), FontSize = 10, FontWeight = FontWeights.Bold });
        panel.Children.Add(new TextBlock { Text = calc.Title, FontSize = 18, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 9, 0, 6) });
        panel.Children.Add(new TextBlock { Text = calc.Description, Foreground = (Brush)FindResource("MutedBrush"), FontSize = 12, TextWrapping = TextWrapping.Wrap, LineHeight = 18 });
        panel.Children.Add(new TextBlock { Text = "Åpne  →", Foreground = (Brush)FindResource("BrandBrush"), FontSize = 12, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 17, 0, 0) });

        var button = new Button
        {
            Content = panel, Tag = calc, Width = 246, MinHeight = 166, Padding = new Thickness(18), Margin = new Thickness(0, 0, 14, 14),
            Background = Brushes.White, BorderBrush = (Brush)FindResource("LineBrush"), BorderThickness = new Thickness(1),
            HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Top, Cursor = Cursors.Hand
        };
        button.Template = CardTemplate();
        button.Click += Card_Click;
        return button;
    }

    private static ControlTemplate CardTemplate()
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.Name = "CardBorder";
        border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
        border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
        border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(11));
        border.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Button.PaddingProperty));
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        border.AppendChild(presenter);
        var template = new ControlTemplate(typeof(Button)) { VisualTree = border };
        var hover = new Trigger { Property = IsMouseOverProperty, Value = true };
        hover.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(30, 107, 79)), "CardBorder"));
        hover.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(249, 252, 250)), "CardBorder"));
        template.Triggers.Add(hover);
        return template;
    }

    private void Card_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: CalculatorDefinition calc }) OpenCalculator(calc);
    }

    private void OpenCalculator(CalculatorDefinition calc)
    {
        _active = calc; _lastResult = null; _inputs.Clear(); InputGrid.Children.Clear(); ResultItems.Children.Clear();
        DetailCategory.Text = calc.Category.ToUpper(_norwegian);
        DetailTitle.Text = calc.Title; DetailDescription.Text = calc.Description;
        foreach (var field in calc.Fields)
        {
            var stack = new StackPanel { Margin = new Thickness(0, 0, 14, 16) };
            stack.Children.Add(new TextBlock { Text = field.Label, FontSize = 12, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 6) });
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition()); grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var box = new TextBox { Style = (Style)FindResource("InputBox"), Text = field.DefaultValue?.ToString("0.##", _norwegian) ?? "", Tag = field.Key };
            box.KeyDown += Input_KeyDown;
            grid.Children.Add(box);
            if (!string.IsNullOrWhiteSpace(field.Unit))
            {
                var unit = new Border { Background = new SolidColorBrush(Color.FromRgb(243, 245, 241)), BorderBrush = (Brush)FindResource("LineBrush"), BorderThickness = new Thickness(0, 1, 1, 1), Padding = new Thickness(11, 0, 11, 0), Child = new TextBlock { Text = field.Unit, Foreground = (Brush)FindResource("MutedBrush"), VerticalAlignment = VerticalAlignment.Center } };
                Grid.SetColumn(unit, 1); grid.Children.Add(unit);
            }
            stack.Children.Add(grid); InputGrid.Children.Add(stack); _inputs[field.Key] = box;
        }
        NoteBorder.Visibility = string.IsNullOrWhiteSpace(calc.Note) ? Visibility.Collapsed : Visibility.Visible;
        NoteText.Text = calc.Note ?? "";
        ElectricalDisclaimer.Visibility = calc.Category == "Elektro" ? Visibility.Visible : Visibility.Collapsed;
        ErrorText.Visibility = Visibility.Collapsed; ResultBorder.Visibility = Visibility.Collapsed;
        HeaderPanel.Visibility = Visibility.Collapsed; CatalogScroll.Visibility = Visibility.Collapsed; DetailScroll.Visibility = Visibility.Visible;
        DetailScroll.ScrollToTop(); _inputs.Values.FirstOrDefault()?.Focus();
    }

    private void Calculate_Click(object sender, RoutedEventArgs e) => Calculate();
    private void Input_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Calculate(); }

    private void Calculate()
    {
        if (_active is null) return;
        try
        {
            var values = new Dictionary<string, double?>();
            foreach (var field in _active.Fields)
            {
                var text = _inputs[field.Key].Text.Trim();
                if (text.Length == 0) { values[field.Key] = null; continue; }
                if (!double.TryParse(text, NumberStyles.Float, _norwegian, out var number) &&
                    !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
                    throw new CalculationException($"«{field.Label}» må være et gyldig tall.");
                values[field.Key] = number;
            }
            _lastResult = CalculatorEngine.Calculate(_active.Id, values);
            RenderResult(_lastResult); ErrorText.Visibility = Visibility.Collapsed;
        }
        catch (CalculationException ex) { ShowError(ex.Message); }
        catch (Exception) { ShowError("Kunne ikke beregne resultatet. Kontroller verdiene."); }
    }

    private void RenderResult(CalculationResult result)
    {
        ResultItems.Children.Clear();
        foreach (var item in result.Items)
        {
            var row = new Grid { Margin = new Thickness(0, 4, 0, 4) };
            row.ColumnDefinitions.Add(new ColumnDefinition()); row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.Children.Add(new TextBlock { Text = item.Label, Foreground = (Brush)FindResource("MutedBrush"), FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
            var format = item.Decimals == 0 ? "N0" : $"N{item.Decimals}";
            var value = item.Value.ToString(format, _norwegian) + (item.Unit.Length > 0 ? $" {item.Unit}" : "");
            var output = new TextBlock { Text = value, FontSize = 21, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetColumn(output, 1); row.Children.Add(output); ResultItems.Children.Add(row);
        }
        ResultExplanation.Text = result.Explanation ?? "";
        ResultExplanation.Visibility = string.IsNullOrWhiteSpace(result.Explanation) ? Visibility.Collapsed : Visibility.Visible;
        ResultBorder.Visibility = Visibility.Visible;
    }

    private void ShowError(string message) { ErrorText.Text = message; ErrorText.Visibility = Visibility.Visible; ResultBorder.Visibility = Visibility.Collapsed; }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        if (_active is null) return;
        foreach (var field in _active.Fields) _inputs[field.Key].Text = field.DefaultValue?.ToString("0.##", _norwegian) ?? "";
        ErrorText.Visibility = Visibility.Collapsed; ResultBorder.Visibility = Visibility.Collapsed; _lastResult = null;
        _inputs.Values.FirstOrDefault()?.Focus();
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        if (_active is null || _lastResult is null) return;
        var text = new StringBuilder(_active.Title).AppendLine();
        foreach (var item in _lastResult.Items)
            text.Append(item.Label).Append(": ").Append(item.Value.ToString($"N{item.Decimals}", _norwegian)).Append(' ').AppendLine(item.Unit);
        Clipboard.SetText(text.ToString().Trim()); CopyButton.Content = "Kopiert";
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        DetailScroll.Visibility = Visibility.Collapsed; HeaderPanel.Visibility = Visibility.Visible; CatalogScroll.Visibility = Visibility.Visible;
        CopyButton.Content = "Kopier";
    }

    private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e) => RenderCards();
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => RenderCards();
}
