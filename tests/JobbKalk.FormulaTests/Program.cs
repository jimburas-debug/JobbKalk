using JobbKalk.Core;

var tests = new (string Name, Action Run)[]
{
    ("Ohms lov", () => Expect("ohms_law", new() { ["u"] = 230, ["r"] = 46, ["i"] = null }, 0, 230)),
    ("1-fase strøm", () => Expect("one_phase", new() { ["p"] = 2.3, ["u"] = 230, ["pf"] = 1, ["eta"] = 100 }, 0, 10)),
    ("3-fase strøm", () => Expect("three_phase", new() { ["p"] = 11, ["u"] = 400, ["pf"] = 1, ["eta"] = 100 }, 0, 15.877)),
    ("Energikostnad", () => Expect("energy_cost", new() { ["p"] = 2, ["hours"] = 3, ["price"] = 1.5 }, 1, 9)),
    ("Spenningsfall 1-fase", () => Expect("voltage_drop_1p", new() { ["length"] = 20, ["current"] = 10, ["area"] = 2.5, ["voltage"] = 230 }, 0, 2.8)),
    ("Areal med svinn", () => Expect("area_waste", new() { ["length"] = 5, ["width"] = 4, ["waste"] = 10 }, 1, 22)),
    ("Betong", () => Expect("concrete", new() { ["length"] = 5, ["width"] = 4, ["depth"] = 10, ["waste"] = 5 }, 1, 2.1)),
    ("Maling", () => Expect("paint", new() { ["area"] = 40, ["coats"] = 2, ["coverage"] = 8, ["waste"] = 5 }, 0, 10.5)),
    ("Gulvpakker", () => Expect("flooring", new() { ["area"] = 20, ["waste"] = 10, ["pack"] = 2.2 }, 1, 10)),
    ("Drivstofforbruk", () => Expect("fuel_consumption", new() { ["liters"] = 45, ["distance"] = 600 }, 0, 7.5)),
    ("Turkostnad", () => Expect("trip_cost", new() { ["distance"] = 300, ["consumption"] = 8, ["price"] = 22 }, 1, 528)),
    ("Nyttelast", () => Expect("payload", new() { ["gross"] = 3500, ["curb"] = 2200, ["load"] = 400 }, 1, 900)),
    ("Prosentvis endring", () => Expect("percent_change", new() { ["old"] = 100, ["new"] = 125 }, 0, 25)),
    ("Temperatur", () => Expect("temperature", new() { ["c"] = 20 }, 0, 68)),
    ("Påslag", () => Expect("markup_margin", new() { ["cost"] = 100, ["markup"] = 25 }, 2, 20)),
    ("Null i nevner avvises", () => ExpectThrows("cost_per_km", new() { ["cost"] = 100, ["distance"] = 0 }))
};

var failed = 0;
foreach (var test in tests)
{
    try { test.Run(); Console.WriteLine($"PASS  {test.Name}"); }
    catch (Exception ex) { failed++; Console.WriteLine($"FAIL  {test.Name}: {ex.Message}"); }
}
Console.WriteLine($"\n{tests.Length - failed}/{tests.Length} tester bestått.");
return failed == 0 ? 0 : 1;

static void Expect(string id, Dictionary<string, double?> input, int index, double expected)
{
    var actual = CalculatorEngine.Calculate(id, input).Items[index].Value;
    if (Math.Abs(actual - expected) > 0.001) throw new Exception($"forventet {expected}, fikk {actual}");
}

static void ExpectThrows(string id, Dictionary<string, double?> input)
{
    try { CalculatorEngine.Calculate(id, input); }
    catch (CalculationException) { return; }
    throw new Exception("forventet CalculationException");
}
