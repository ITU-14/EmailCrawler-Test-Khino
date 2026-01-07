# Web Crawler de mail

Implémenter un algorithme pour web crawler qui cherche tous les mails distincts dans le
html d'une page web (mailto dans les href) et les pages web qu'elle référence.

Il est à noter que le `Program.cs` se base sur les UseCases qui sont présents dans l'énoncé avec les fichiers HTML présents dans le dossier `EmailCrawler/HtmlPages`.

Par contre, j'ai créé des Tests Unitaires dans le projet `EmailCrawler.Tests` pour des tests un peu plus poussés.


## Prérequis
- .NET Core 8.0 (ou supérieur)
- CLI .NET (`dotnet` si vous ne possédez pas Visual Studio ou Rider)

## Exécution du projet

### 1. Avec Visual Studio ou Rider

1. Ouvrir la solution `.sln` dans Visual Studio ou Rider
2. Lancer le projet
3. Les résultats des UsesCases de l'énoncé seront affichés dans la console
4. Lancer les tests unitaires pour voir les différents TestCases

---

### 2. Avec CLI

Depuis la racine de la solution, lancez :

```bash
cd EmailCrawler
dotnet run
```

Pour les tests unitaires, depuis la racine de la solution, lancez:
```bash
dotnet test
```
Il est à noter que je recommande quand même l'utilisation de Visual Studio ou de Rider pour plus de visibilité sur les tests.

### Dev
Khino Nirijaona