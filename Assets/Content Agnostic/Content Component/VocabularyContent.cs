using UnityEngine;
using System.Collections;

public class VocabularyContent : Content {

    public string[,] functions;
    public VocabularyContent(string name, string description)
    {
        this.name = name;
        this.description = description;
        functions = new string[,] { { "grumpy", "huffy" }, { "acme", "summit" }, { "spree", "razzle" }, { "morass", "bog" } };
    }

    public override string[,] getData()
    {
        return functions;
    }
}
