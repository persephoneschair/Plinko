using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.PlayerLoop;

public static class QuestionManager
{
    public static Pack currentPack = null;
    public static int nextQuestionIndex = 0;

    public static void DecompilePack(TextAsset tx)
    {
        currentPack = JsonConvert.DeserializeObject<Pack>(tx.text);
        nextQuestionIndex = 0;
    }

    public static int GetRoundQCount()
    {
        return currentPack.questions.Count;
    }

    public static Question GetQuestion()
    {
        nextQuestionIndex++;
        return currentPack.questions[nextQuestionIndex - 1];
    }
}
