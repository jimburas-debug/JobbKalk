namespace JobbKalk.Core;

public static class CalculatorEngine
{
    private const double CopperResistivity = 0.0175;

    public static CalculationResult Calculate(string id, IReadOnlyDictionary<string, double?> v) => id switch
    {
        "ohms_law" => OhmsLaw(v),
        "one_phase" => OnePhase(v),
        "three_phase" => ThreePhase(v),
        "energy_cost" => EnergyCost(v),
        "voltage_drop_1p" => VoltageDrop(v, false),
        "voltage_drop_3p" => VoltageDrop(v, true),
        "area_waste" => AreaWaste(v),
        "volume" => Volume(v),
        "concrete" => Concrete(v),
        "paint" => Paint(v),
        "flooring" => Flooring(v),
        "roof_pitch" => RoofPitch(v),
        "fuel_consumption" => FuelConsumption(v),
        "trip_cost" => TripCost(v),
        "cost_per_km" => CostPerKm(v),
        "payload" => Payload(v),
        "travel_time" => TravelTime(v),
        "percentage" => Percentage(v),
        "percent_change" => PercentChange(v),
        "decimal_time" => DecimalTime(v),
        "length" => Length(v),
        "temperature" => Temperature(v),
        "markup_margin" => MarkupMargin(v),
        _ => throw new ArgumentException("Ukjent kalkulator.", nameof(id))
    };

    private static double Get(IReadOnlyDictionary<string, double?> values, string key, bool positive = false)
    {
        if (!values.TryGetValue(key, out var value) || value is null || double.IsNaN(value.Value) || double.IsInfinity(value.Value))
            throw new CalculationException("Fyll inn alle obligatoriske felt.");
        if (positive && value <= 0) throw new CalculationException("Denne verdien må være større enn null.");
        return value.Value;
    }

    private static CalculationResult R(params ResultItem[] items) => new(items);

    private static CalculationResult OhmsLaw(IReadOnlyDictionary<string, double?> v)
    {
        var u = v.GetValueOrDefault("u"); var i = v.GetValueOrDefault("i"); var r = v.GetValueOrDefault("r");
        if (new[] { u, i, r }.Count(x => x.HasValue) != 2)
            throw new CalculationException("Fyll inn nøyaktig to av feltene U, I og R.");
        if (u.HasValue && i.HasValue) r = u.Value / NonZero(i.Value);
        else if (u.HasValue && r.HasValue) i = u.Value / NonZero(r.Value);
        else if (i.HasValue && r.HasValue) u = i.Value * r.Value;
        return R(new("Spenning", u!.Value, "V"), new("Strøm", i!.Value, "A"),
            new("Motstand", r!.Value, "Ω"), new("Effekt", u.Value * i.Value, "W"));
    }

    private static CalculationResult OnePhase(IReadOnlyDictionary<string, double?> v)
    {
        var p = Get(v, "p"); var u = Get(v, "u", true); var pf = Factor(Get(v, "pf"), "Effektfaktor");
        var eta = PercentFactor(Get(v, "eta"), "Virkningsgrad");
        return R(new("Beregnet strøm", p * 1000 / (u * pf * eta), "A"));
    }

    private static CalculationResult ThreePhase(IReadOnlyDictionary<string, double?> v)
    {
        var p = Get(v, "p"); var u = Get(v, "u", true); var pf = Factor(Get(v, "pf"), "Effektfaktor");
        var eta = PercentFactor(Get(v, "eta"), "Virkningsgrad");
        return R(new("Beregnet linjestrøm", p * 1000 / (Math.Sqrt(3) * u * pf * eta), "A"));
    }

    private static CalculationResult EnergyCost(IReadOnlyDictionary<string, double?> v)
    {
        var energy = Get(v, "p") * Get(v, "hours");
        return R(new("Energibruk", energy, "kWh"), new("Kostnad", energy * Get(v, "price"), "kr"));
    }

    private static CalculationResult VoltageDrop(IReadOnlyDictionary<string, double?> v, bool threePhase)
    {
        var length = Get(v, "length"); var current = Get(v, "current"); var area = Get(v, "area", true);
        var voltage = Get(v, "voltage", true);
        var drop = (threePhase ? Math.Sqrt(3) : 2) * length * current * CopperResistivity / area;
        return new([new("Spenningsfall", drop, "V"), new("Spenningsfall", drop / voltage * 100, "%")],
            "Veiledende resultat for kobberleder ved 20 °C.");
    }

    private static CalculationResult AreaWaste(IReadOnlyDictionary<string, double?> v)
    {
        var area = Get(v, "length") * Get(v, "width");
        return R(new("Netto areal", area, "m²"), new("Materialbehov", area * (1 + Get(v, "waste") / 100), "m²"));
    }

    private static CalculationResult Volume(IReadOnlyDictionary<string, double?> v)
    {
        var volume = Get(v, "length") * Get(v, "width") * Get(v, "height");
        return R(new("Volum", volume, "m³"), new("Volum", volume * 1000, "liter", 1));
    }

    private static CalculationResult Concrete(IReadOnlyDictionary<string, double?> v)
    {
        var net = Get(v, "length") * Get(v, "width") * Get(v, "depth") / 100;
        return R(new("Netto volum", net, "m³", 3), new("Bestillingsmengde", net * (1 + Get(v, "waste") / 100), "m³", 3));
    }

    private static CalculationResult Paint(IReadOnlyDictionary<string, double?> v)
    {
        var liters = Get(v, "area") * Get(v, "coats") / Get(v, "coverage", true) * (1 + Get(v, "waste") / 100);
        return R(new("Anslått maling", liters, "liter"));
    }

    private static CalculationResult Flooring(IReadOnlyDictionary<string, double?> v)
    {
        var material = Get(v, "area") * (1 + Get(v, "waste") / 100); var pack = Get(v, "pack", true);
        return R(new("Materialbehov", material, "m²"), new("Hele pakker", Math.Ceiling(material / pack - 1e-10), "stk", 0));
    }

    private static CalculationResult RoofPitch(IReadOnlyDictionary<string, double?> v)
    {
        var rise = Get(v, "rise"); var run = Get(v, "run", true);
        return R(new("Takvinkel", Math.Atan(rise / run) * 180 / Math.PI, "°", 1),
            new("Skrålengde", Math.Sqrt(rise * rise + run * run), "m"));
    }

    private static CalculationResult FuelConsumption(IReadOnlyDictionary<string, double?> v) =>
        R(new("Forbruk", Get(v, "liters") / Get(v, "distance", true) * 100, "l/100 km"));

    private static CalculationResult TripCost(IReadOnlyDictionary<string, double?> v)
    {
        var liters = Get(v, "distance") * Get(v, "consumption") / 100;
        return R(new("Drivstoff", liters, "liter"), new("Kostnad", liters * Get(v, "price"), "kr"));
    }

    private static CalculationResult CostPerKm(IReadOnlyDictionary<string, double?> v) =>
        R(new("Kostnad", Get(v, "cost") / Get(v, "distance", true), "kr/km"));

    private static CalculationResult Payload(IReadOnlyDictionary<string, double?> v)
    {
        var max = Get(v, "gross") - Get(v, "curb"); var remaining = max - Get(v, "load");
        return R(new("Maksimal nyttelast", max, "kg", 0), new("Gjenstående nyttelast", remaining, "kg", 0));
    }

    private static CalculationResult TravelTime(IReadOnlyDictionary<string, double?> v)
    {
        var hours = Get(v, "distance") / Get(v, "speed", true); var whole = Math.Floor(hours);
        return R(new("Timer", whole, "t", 0), new("Minutter", Math.Round((hours - whole) * 60), "min", 0));
    }

    private static CalculationResult Percentage(IReadOnlyDictionary<string, double?> v) =>
        R(new("Resultat", Get(v, "value") * Get(v, "percent") / 100, ""));

    private static CalculationResult PercentChange(IReadOnlyDictionary<string, double?> v) =>
        R(new("Endring", (Get(v, "new") - Get(v, "old", true)) / Get(v, "old", true) * 100, "%"));

    private static CalculationResult DecimalTime(IReadOnlyDictionary<string, double?> v)
    {
        var hours = Get(v, "hours"); var whole = Math.Floor(hours);
        return R(new("Timer", whole, "t", 0), new("Minutter", Math.Round((hours - whole) * 60), "min", 0));
    }

    private static CalculationResult Length(IReadOnlyDictionary<string, double?> v)
    {
        var m = Get(v, "meters");
        return R(new("Millimeter", m * 1000, "mm"), new("Centimeter", m * 100, "cm"), new("Kilometer", m / 1000, "km", 4));
    }

    private static CalculationResult Temperature(IReadOnlyDictionary<string, double?> v)
    {
        var c = Get(v, "c");
        return R(new("Fahrenheit", c * 9 / 5 + 32, "°F", 1), new("Kelvin", c + 273.15, "K", 2));
    }

    private static CalculationResult MarkupMargin(IReadOnlyDictionary<string, double?> v)
    {
        var cost = Get(v, "cost"); var sale = cost * (1 + Get(v, "markup") / 100);
        return R(new("Salgspris", sale, "kr"), new("Bruttofortjeneste", sale - cost, "kr"),
            new("Bruttomargin", sale == 0 ? 0 : (sale - cost) / sale * 100, "%"));
    }

    private static double NonZero(double value) => Math.Abs(value) < 1e-12
        ? throw new CalculationException("Verdien kan ikke være null.") : value;

    private static double Factor(double value, string name) => value is > 0 and <= 1 ? value
        : throw new CalculationException($"{name} må være større enn 0 og høyst 1.");

    private static double PercentFactor(double value, string name) => value is > 0 and <= 100 ? value / 100
        : throw new CalculationException($"{name} må være større enn 0 og høyst 100 %.");
}

public sealed class CalculationException(string message) : Exception(message);
