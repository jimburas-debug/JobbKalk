namespace JobbKalk.Core;

public static class CalculatorCatalog
{
    public static readonly IReadOnlyList<string> Categories =
        ["Alle", "Elektro", "Bygg og material", "Transport", "Generelt"];

    private static CalculatorField F(string key, string label, string unit, double? value = null,
        bool required = true, string placeholder = "") =>
        new(key, label, unit, placeholder, required, value);

    public static readonly IReadOnlyList<CalculatorDefinition> All =
    [
        new("ohms_law", "Elektro", "Ohms lov", "Finn manglende verdi når to av U, I og R er kjent.",
            [F("u", "Spenning U", "V", required: false), F("i", "Strøm I", "A", required: false), F("r", "Motstand R", "Ω", required: false)],
            "Fyll inn nøyaktig to felt. Effekt beregnes også."),
        new("one_phase", "Elektro", "1-fase strøm", "Beregn strøm fra effekt, spenning, effektfaktor og virkningsgrad.",
            [F("p", "Aktiv effekt", "kW"), F("u", "Spenning", "V", 230), F("pf", "Effektfaktor cos φ", "", 1), F("eta", "Virkningsgrad", "%", 100)]),
        new("three_phase", "Elektro", "3-fase strøm", "Beregn linjestrøm i et balansert trefasesystem.",
            [F("p", "Aktiv effekt", "kW"), F("u", "Linjespenning", "V", 400), F("pf", "Effektfaktor cos φ", "", 1), F("eta", "Virkningsgrad", "%", 100)]),
        new("energy_cost", "Elektro", "Energibruk og kostnad", "Beregn energiforbruk og pris for en last over tid.",
            [F("p", "Effekt", "kW"), F("hours", "Driftstid", "timer"), F("price", "Total energipris", "kr/kWh")]),
        new("voltage_drop_1p", "Elektro", "Spenningsfall 1-fase", "Veiledende spenningsfall i kobberleder ved 20 °C.",
            [F("length", "Énveis kabellengde", "m"), F("current", "Strøm", "A"), F("area", "Ledertverrsnitt", "mm²"), F("voltage", "Nominell spenning", "V", 230)],
            "Bruker kobberresistivitet 0,0175 Ω·mm²/m. Temperatur, reaktans og korreksjonsfaktorer er ikke med."),
        new("voltage_drop_3p", "Elektro", "Spenningsfall 3-fase", "Veiledende spenningsfall i kobberleder ved 20 °C.",
            [F("length", "Énveis kabellengde", "m"), F("current", "Strøm", "A"), F("area", "Ledertverrsnitt", "mm²"), F("voltage", "Linjespenning", "V", 400)],
            "Forutsetter balansert last og cos φ ≈ 1. Temperatur, reaktans og korreksjonsfaktorer er ikke med."),

        new("area_waste", "Bygg og material", "Areal med svinn", "Beregn rektangulært areal og materialbehov med svinn.",
            [F("length", "Lengde", "m"), F("width", "Bredde", "m"), F("waste", "Svinn", "%", 10)]),
        new("volume", "Bygg og material", "Volum", "Beregn volum for et rektangulært rom eller objekt.",
            [F("length", "Lengde", "m"), F("width", "Bredde", "m"), F("height", "Høyde", "m")]),
        new("concrete", "Bygg og material", "Betongmengde", "Beregn nødvendig betongvolum med tillegg.",
            [F("length", "Lengde", "m"), F("width", "Bredde", "m"), F("depth", "Tykkelse", "cm"), F("waste", "Tillegg", "%", 5)]),
        new("paint", "Bygg og material", "Maling", "Beregn liter maling fra areal, strøk og oppgitt dekkevne.",
            [F("area", "Flate", "m²"), F("coats", "Antall strøk", "", 2), F("coverage", "Dekkevne", "m²/l", 8), F("waste", "Tillegg", "%", 5)]),
        new("flooring", "Bygg og material", "Gulv og fliser", "Beregn materialareal og antall pakker.",
            [F("area", "Gulvareal", "m²"), F("waste", "Svinn", "%", 10), F("pack", "Dekning per pakke", "m²")]),
        new("roof_pitch", "Bygg og material", "Takvinkel", "Beregn takvinkel og skrålengde fra høyde og horisontal lengde.",
            [F("rise", "Høydeforskjell", "m"), F("run", "Horisontal lengde", "m")]),

        new("fuel_consumption", "Transport", "Drivstofforbruk", "Beregn liter per 100 km fra fylt mengde og kjørt avstand.",
            [F("liters", "Drivstoff brukt", "liter"), F("distance", "Kjørt avstand", "km")]),
        new("trip_cost", "Transport", "Turkostnad", "Beregn drivstoffmengde og kostnad for en kjøretur.",
            [F("distance", "Avstand", "km"), F("consumption", "Forbruk", "l/100 km"), F("price", "Drivstoffpris", "kr/l")]),
        new("cost_per_km", "Transport", "Kostnad per km", "Fordel samlede kostnader på kjørt avstand.",
            [F("cost", "Samlede kostnader", "kr"), F("distance", "Kjørt avstand", "km")]),
        new("payload", "Transport", "Nyttelast", "Beregn maksimal og gjenstående nyttelast.",
            [F("gross", "Tillatt totalvekt", "kg"), F("curb", "Egenvekt med fører", "kg"), F("load", "Last om bord", "kg", 0)]),
        new("travel_time", "Transport", "Kjøretid", "Beregn tid fra avstand og gjennomsnittsfart.",
            [F("distance", "Avstand", "km"), F("speed", "Gjennomsnittsfart", "km/t")]),

        new("percentage", "Generelt", "Prosent av tall", "Finn en prosentandel av en verdi.",
            [F("value", "Verdi", ""), F("percent", "Prosent", "%")]),
        new("percent_change", "Generelt", "Prosentvis endring", "Finn prosentvis økning eller reduksjon.",
            [F("old", "Gammel verdi", ""), F("new", "Ny verdi", "")]),
        new("decimal_time", "Generelt", "Desimaltid", "Gjør desimaltimer om til timer og minutter.",
            [F("hours", "Desimaltimer", "timer")]),
        new("length", "Generelt", "Lengdeomregning", "Gjør meter om til millimeter, centimeter og kilometer.",
            [F("meters", "Lengde", "m")]),
        new("temperature", "Generelt", "Temperatur", "Gjør grader Celsius om til Fahrenheit og Kelvin.",
            [F("c", "Temperatur", "°C")]),
        new("markup_margin", "Generelt", "Påslag og margin", "Beregn salgspris og bruttomargin fra kostpris og påslag.",
            [F("cost", "Kostpris", "kr"), F("markup", "Påslag", "%")])
    ];
}
