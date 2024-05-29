using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    // Es folgen globale Einstellungen f�r die App
    // Diese k�nnen nicht via Code ge�ndert werden.
    public static readonly int NumQuestionsPerRound = 6;

    // Falls True -> Wir sind im Gameloop und "AktuelleFragerunde" ist valide.
    // Falls False -> Wir sind "irgendwo" und "AktuelleFragerunde" ist default initialisiert (nutzlos).
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;

    // Das hier ist die "Referenzgr��e" f�r 16:9 (Gr��e in Pixeln des Canvas)
    // Ausgehend davon wird alles auf die tats�chlich vorhandene Aufl�sung skaliert.
    // Das gilt aber nur f�r 16:9 -> TODO UI-Design-Team. damit es auch z.B. bei 21:9 geht.
    public static float width = 304;
    public static float height = 544;

}

