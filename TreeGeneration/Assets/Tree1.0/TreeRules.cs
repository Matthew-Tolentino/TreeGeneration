using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRules
{
    // Axiom = +FF+F+
    Rule[] reed = new Rule[]
    {
        new Rule('R', "FR[^F/F]R[/F+F]R[&F\\F]R[\\F^F]R"),
        new Rule('F', "F+F+")
    };

    // Axiom = R
    Rule[] twistingTree = new Rule[]
    {
        new Rule('F', "F+[F+\\[\\F+/^F+/-F+]/[/F+&\\F++\\F+]C+]")
    };

    // Axiom = +FF+R
    Rule[] smallTree = new Rule[]{
        new Rule('R', "F+F+F+[F+^-/[F+&R&F+[\\F+R\\^F+C]R[/F+/+F+C]]+F+R+F+]") 
    };

    // Axiom = +FF+R
    Rule[] twistedLeaningTree = new Rule[]
    {
        new Rule('R', "F+[[++BB\\B]^B]"),
        new Rule('B', "^^---F+RF+\\\\F+RRC")
    };

    // Axiom = +FF+R
    Rule[] fanTree = new Rule[]
    {
        new Rule('R', "--FF+[[[&F+R]C[^F+RF+R]B]A]"),
        new Rule('B', "/[F\\F]F[F\\+FBF]+B"),
        new Rule('A', "\\[F\\F]F[F/+FAF]+A")
    };
}

[System.Serializable]
public struct Rule
{
    public char c;
    public string s;

    public Rule(char C, string S)
    {
        this.c = C;
        this.s = S;
    }
}