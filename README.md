ToDo

# Template des applications C# utilisant la Clean Architecture

## Introduction

Dans le but de standardiser le développement des applications C# ce template basé sur la Clean Architecture "Architecture Propre" sera utilisé.

## Exigences

Le template se base sur les outils techniques suivants:

| Outils      | Version |
|-------------|---------|
| dotnet      | 6.0     |
| C#          | 10      |

## Prise en main

### *Forker* le projet

- Au démarrage du projet, il suffit de créer un fork du template dans le namespace adéquat en cliquant sur le bouton `Fork`.

- Cloner le projet en local et procéder aux configurations nécessaires au développement.

### Installer les outils

- Installer dotnet Core (voir [Site Officiel](https://dotnet.microsoft.com/en-us/download))
- Vérifier la version de dotnet core installée :

```bash
dotnet --version
```

- Installer Git (voir [Site officiel](https://git-scm.com)))
- Cloner le projet

```bash
git clone slido03/CleanArchitectureTemplate.git

- Installer Entity Framework

```
dotnet tool install --global dotnet-ef
```

### Configurations
- Ouvrir la solution
- Configurer les paramètres du projet
  * Dupliquer le fichier de configuration `src/Server/appsettings.json.default` et le renommer
    en `src/Server/appsettings.json`
  * Spécifier les paramètres de la base de données : remplacer `<SERVER_ADDRESS_INSTANCE>` et `<DATABASE_NAME>` par
    leurs valeurs respectives.
  * Effectuer les migrations (se déplacer dans le projet `Server` et exécuter :


  ```
  dotnet ef database update
  ```

### Exécuter

Sur visual Studio, ouvrir la solution puis cliquer sur le bouton Exécuter.

Aussi, dans la racine de la solution, exécuter la commande suivante :

```bash
dotnet run
```

## Contributeurs


## Comment contribuer
