# JobbKalk

JobbKalk er et norsk, lokalt Windows-verktøy med praktiske kalkulatorer for arbeid og DIY. Programmet er laget for Windows 10/11 (64-bit), krever ingen konto og bruker ikke internett.

## Innhold i første versjon

| Område | Kalkulatorer |
|---|---|
| Elektro | Ohms lov, 1-fase strøm, 3-fase strøm, energibruk/kostnad, spenningsfall 1-fase og 3-fase |
| Bygg og material | Areal med svinn, volum, betongmengde, maling, gulv/fliser og takvinkel |
| Transport | Drivstofforbruk, turkostnad, kostnad per km, nyttelast og kjøretid |
| Generelt | Prosent av tall, prosentvis endring, desimaltid, lengde, temperatur og påslag/margin |

Totalt: 23 kalkulatorer.

## Brukeropplevelse

- fagområder i venstremenyen
- søk i alle kalkulatorer
- tydelige felt med enheter
- validering av ugyldige og manglende verdier
- resultat med norsk tallformat
- kopiering av resultat til utklippstavlen
- ingen nettleser, WebView, database, sporing eller nettilgang

## Lokal utvikling

Krav: Windows 10/11 og .NET 10 SDK.

```powershell
dotnet run --project .\src\JobbKalk\JobbKalk.csproj
```

Kjør formeltestene:

```powershell
dotnet run --project .\tests\JobbKalk.FormulaTests\JobbKalk.FormulaTests.csproj -c Release
```

## Lag bærbar EXE

Kjør fra PowerShell i prosjektmappen:

```powershell
.\scripts\publish.ps1
```

Dette lager `artifacts\portable\JobbKalk.exe`. Filen er en selvstendig 64-bit Windows-utgave og krever ikke separat .NET-installasjon.

## Lag installasjonsfil

Installer [NSIS](https://nsis.sourceforge.io/), kjør først publiseringsskriptet og deretter:

```powershell
& "C:\Program Files (x86)\NSIS\makensis.exe" .\installer\JobbKalk.nsi
```

Resultatet blir `artifacts\JobbKalk-Setup-x64.exe`. Installeringen oppretter snarvei på skrivebordet og i startmenyen, og registrerer avinstallering i Windows.

Alternativt kan arbeidsflyten `Bygg Windows-versjoner` kjøres i GitHub Actions. Den tester formlene og lager begge EXE-filene som én nedlastbar byggepakke.

## Viktig om elektro

Elektroberegningene er veiledende. Programmet utfører ikke normativ kabeldimensjonering og har ikke tabeller for strømføringsevne, vern, forlegningsmåte, temperatur, gruppering eller kortslutningsytelse. Slike valg må verifiseres mot gjeldende regelverk og av kvalifisert personell.

Se [FORMULAS.md](FORMULAS.md) for formler og forutsetninger.
