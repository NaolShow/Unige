<img src="https://github.com/NaolShow/Unige/raw/readme/Assets/unige-large.png">

---

<h4 align="center">« Gestion des services des IUTs de l'Université de Bourgogne »</h4>

<div align="center">
  
  <a href="https://github.com/NaolShow/Unige/blob/main/LICENCE"><img alt="GitHub license" src="https://img.shields.io/github/license/NaolShow/Unige?style=flat-square"></a>  
  
</div>
<div align="center">

  <a href="https://github.com/NaolShow/Unige/issues"><img alt="GitHub issues" src="https://img.shields.io/github/issues/NaolShow/Unige?style=flat-square"></a>
  <a href="https://github.com/NaolShow/Unige/pulls"><img alt="GitHub pull requests" src="https://img.shields.io/github/issues-pr/NaolShow/Unige?style=flat-square"/></a>
  <a href="https://github.com/NaolShow/Unige/commits/main"><img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/NaolShow/Unige?style=flat-square"/></a>

</div>

---

# Sommaire

- [Unige](https://github.com/NaolShow/Unige/main/README.md#Unige)
    - [À savoir](https://github.com/NaolShow/Unige/main/README.md#À-savoir)
- [Logiciel de calcul de la moyenne](https://github.com/NaolShow/Unige/main/README.md#Logiciel-de-calcul-de-la-moyenne)

---

# Unige

Unige est avant-tous une bibliothèque en C# qui contient des outils qui permettent d'interagir avec les services des IUTs.

Pour l'instant, Unige a plusieurs fonctionnalités:
- [Oge](https://github.com/NaolShow/Unige/blob/main/OgeSharp/Oge.cs)
    - [Emploi du temps](https://github.com/NaolShow/Unige/tree/main/OgeSharp/Schedule)
        - Récupération de l'emploi du temps de la semaine
        - Récupération de l'emploi du temps sur une journée
        - Récupération de l'emploi du temps (entre deux dates données)
    - [Notes](https://github.com/NaolShow/Unige/tree/main/OgeSharp/Grades)
        - Récupération de toutes les notes (avec leur nom, coefficient...)

Comme vous pouvez le voir, Unige reste (pour l'instant) assez simple, mais d'autres fonctionnalités vont être rajoutées par la suite.

## À savoir

Il faut tout de même savoir qu'Unige peut casser à **TOUT MOMENT**.
Et malheureusement, je ne peux pas savoir à l'avance quand cela va se produire (c'est surtout dû à la modification des sites de l'IUT qui casse la récupération d'informations)

Si cela arrive, hésitez pas à faire [un bug report sur Github](https://github.com/NaolShow/Unige/issues) en expliquant le problème. Je vais le résoudre rapidement.
(Ou alors contactez-moi directement: **NaolShow#7243**)

# Logiciel de calcul de la moyenne

Pour l'instant, le seul logiciel qui découle d'Unige est un petit programme (console) qui vous permet de calculer votre moyenne.
Vous pouvez le télécharger [sur la page release du projet Github](https://github.com/NaolShow/Unige/releases).
