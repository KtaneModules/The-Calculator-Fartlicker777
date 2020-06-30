using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class TheCalculator : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Numbers;
    public KMSelectable[] Operators;
    public KMSelectable[] VariablesAndShit;
    public KMSelectable[] Other;
    public TextMesh ScreenButText;

    string OperatorsString = "EPMTDLSrd";
    char OperatorChar = ' ';
    string FuckYouBlanFuckYou = "";
    string FUkcdkflajyoublan = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string FuckYouTwitchPlays = "0123456789";
    int Wefuckinglovefortnite = 0;

    float First = 0;
    float Second = 0;
    float Third = 0;
    float Fourth = 0;
    int Index = 0;

    float XFactor = 0;
    float YFactor = 0;
    float ZFactor = 0;

    bool Active = false;
    bool[] VariableActivity = {false, false, false};

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Number in Numbers) {
            Number.OnInteract += delegate () { NumberPress(Number); return false; };
        }
        foreach (KMSelectable Operator in Operators) {
            Operator.OnInteract += delegate () { OperatorPress(Operator); return false; };
        }
        foreach (KMSelectable Variable in VariablesAndShit) {
            Variable.OnInteract += delegate () { VariablePress(Variable); return false; };
        }
        foreach (KMSelectable Oth in Other) {
            Oth.OnInteract += delegate () { OthPress(Oth); return false; };
        }
    }

    void Start(){
      FuckYouBlanFuckYou = Bomb.GetSerialNumber();
      for (int j = 0; j < 6; j++) {
        for (int i = 0; i < FUkcdkflajyoublan.Length; i++) {
          if (FuckYouBlanFuckYou[j].ToString() == FUkcdkflajyoublan[i].ToString()) {
            Wefuckinglovefortnite += i;
          }
        }
      }
      Debug.LogFormat("[The Calculator #{0}] The sum of the serial number digits in base 10 is {1}.", moduleId, Wefuckinglovefortnite);
    }

    void NumberPress(KMSelectable Number){
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Number.transform);
      Number.AddInteractionPunch();
      for (int i = 0; i < 10; i++) {
        if (Number == Numbers[i]) {
          if (VariableActivity[0] == true) {
            XFactor *= 10 + 1;
            ScreenButText.text = XFactor.ToString();
          }
          else if (VariableActivity[1] == true) {
            YFactor *= 10 + 1;
            ScreenButText.text = YFactor.ToString();
          }
          else if (VariableActivity[2] == true) {
            ZFactor *= 10 + 1;
            ScreenButText.text = ZFactor.ToString();
          }
          else if (Active == false) {
            First = First * 10 + i;
            ScreenButText.text = First.ToString();
          }
          else {
            Second = Second * 10 + i;
            ScreenButText.text = Second.ToString();
          }
        }
      }
    }

    void OperatorPress(KMSelectable Operator){
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Operator.transform);
      Operator.AddInteractionPunch();
      for (int i = 0; i < 9; i++) {
        if (Operator == Operators[i]) {
          Active = true;
          OperatorChar = OperatorsString[i];
          switch (OperatorChar) {
            case 'S':
            First *= First;
            ScreenButText.text = (First).ToString();
            break;
            case 'd':
            First = (First - 1) % 9 + 1;
            ScreenButText.text = (First).ToString();
            break;
            case 'r':
            First = (float) Math.Sqrt(First);
            ScreenButText.text = First.ToString();
            break;
          }
        }
      }
    }

    void VariablePress(KMSelectable Variable){
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Variable.transform);
      Variable.AddInteractionPunch();
      /*for (int i = 0; i < 3; i++) {
        if (Variable == VariablesAndShit[i]) {
          if (First == 1) {
            switch (i) {
              case 0:
              ScreenButText.text = "X";
              VariableActivity[0] = true;
              VariableActivity[1] = false;
              VariableActivity[2] = false;
              break;
              case 1:
              ScreenButText.text = "Y";
              VariableActivity[0] = false;
              VariableActivity[1] = true;
              VariableActivity[2] = false;
              break;
              case 2:
              ScreenButText.text = "Z";
              VariableActivity[0] = false;
              VariableActivity[1] = false;
              VariableActivity[2] = true;
              break;
            }
          }
          else {
            if (i == 0) {
              ScreenButText.text = XFactor.ToString();
            }
            else if (i == 1) {
              ScreenButText.text = YFactor.ToString();
            }
            else if (i == 2) {
              ScreenButText.text = ZFactor.ToString();
            }
          }
        }
      }*/
    }

    void OthPress(KMSelectable Oth){
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Oth.transform);
      Oth.AddInteractionPunch();
      for (int i = 0; i < 5; i++) {
        if (Oth == Other[i]) {
          switch (i) {
            case 0:
            return;
            break;
            case 1:
            switch (OperatorChar) {
              /*case 'E':
              for (int j = 0; j < 3; j++) {
                VariableActivity[j] = false;
              }
              break;*/
              case 'P':
              ScreenButText.text = (First + Second).ToString();
              break;
              case 'M':
              ScreenButText.text = (First - Second).ToString();
              break;
              case 'T':
              ScreenButText.text = (First * Second).ToString();
              break;
              case 'D':
              if (Second == 0) {
                ScreenButText.text = "Error!";
              }
              else {
                ScreenButText.text = (First / Second).ToString();
              }
              break;
              case 'L':
              ScreenButText.text = (First % Second).ToString();
              break;
            }
            Active = false;
            First = 0;
            Second = 0;
            break;
            case 2:
            if (First == Wefuckinglovefortnite) {
              GetComponent<KMBombModule>().HandlePass();
            }
            First = 0;
            Second = 0;
            ScreenButText.text = "8888888888888.8";
            Active = false;
            break;
            case 3:
            if (Active == true) {
              Second = (Second / 10) - ((Second / 10) % 1);
              ScreenButText.text = Second.ToString();
            }
            else {
              First = (First / 10) - ((First / 10) % 1);
              ScreenButText.text = First.ToString();
            }
            break;
            case 4:
            First = 0;
            Second = 0;
            ScreenButText.text = "8888888888888.8";
            Active = false;
            break;
          }
        }
      }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} # to submit a number.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command){

        for (int i = 0; i < command.Length; i++) {
            if (command[i] == '0') {
              Numbers[0].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '1') {
              Numbers[1].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '2') {
              Numbers[2].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '3') {
              Numbers[3].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '4') {
              Numbers[4].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '5') {
              Numbers[5].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '6') {
              Numbers[6].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '7') {
              Numbers[7].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '8') {
              Numbers[8].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else if (command[i] == '9') {
              Numbers[9].OnInteract();
              yield return new WaitForSeconds(.1f);
            }
            else {
              yield return "sendtochaterror Not a number!";
              Other[4].OnInteract();
              yield break;
            }
        }
        Other[2].OnInteract();
    }
}
