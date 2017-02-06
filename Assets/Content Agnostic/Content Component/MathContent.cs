using UnityEngine;
using System.Collections;

public class MathContent : Content {

    public string[,] functions;
    public MathContent(string name, string description) {
        this.name = name;
        this.description = description;
        functions = new string[,]{{ "Cos45", "Sin45"}, { "Cos30", "Sin60"}, { "Cos60", "Sin30"},{ "Sin72", "Cos18"} };
    }

    public override string[,] getData() {
        return functions;
    }

}
