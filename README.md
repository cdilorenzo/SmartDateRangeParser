# 🗓️ SmartDateRangeParser

> A developer-friendly .NET library for parsing natural human date expressions like `"last 3 business days"` or `"this week"` into usable `DateTime` ranges.

## 🚀 Features

- Fluent parsing of expressions like:
  - `last 7 days`
  - `this week`, `last month`
  - `last 3 business days`
  - `today`, `yesterday`, `tomorrow`
- Returns easy-to-use `DateTime Start` and `DateTime End`
- Small, fast, and dependency-free
- Perfect for filters, dashboards, reports, and scheduled jobs

## 📦 Installation

Install from [NuGet](https://www.nuget.org/packages/SmartDateRangeParser):

```bash
dotnet add package SmartDateRangeParser
```

## 📌 Example Usage

```csharp
var range = SmartDateRange.Parse("last 3 business days");

Console.WriteLine(range.Start); // e.g. 2025-06-02
Console.WriteLine(range.End);   // e.g. 2025-06-04
```

## 💡 Supported Expressions

| Input | Description |
|-------|-------------|
| `today` | Today’s date |
| `yesterday` | Yesterday’s date |
| `last 5 days` | Previous 5 calendar days |
| `last 3 business days` | Skips weekends |
| `this week` | Monday to today |
| `this month` | First of month to today |

More expressions coming soon!

## 🛠️ Planned Roadmap

- [ ] Add support for `this quarter`, `next month`
- [ ] Fiscal/calendar year support
- [ ] Custom expression registration
- [ ] Localization support (German, French)

## 🔄 Changelog

See [CHANGELOG.md](CHANGELOG.md)

## 🧪 Run Tests

```bash
dotnet test
```

## 🤝 Contributing

Pull requests welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## 📄 License

MIT — see [LICENSE](LICENSE)

---

⭐ If you find this useful, give it a star!
