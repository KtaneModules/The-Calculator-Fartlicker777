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
   public KMSelectable[] NumberButtons;
   public KMSelectable[] BinaryOperators;
   public KMSelectable[] UnaryOperators;
   public KMSelectable[] Variables;
   public KMSelectable[] Other;
   public TextMesh ScreenButText;

   static int moduleIdCounter = 1;
   int moduleId;
   private bool moduleSolved;

   double[] numbers = new double[2]; //Number 1, number 2
   string[] numStrings = new string[2] { "", "" };
   double[] cachedVariables = new double[3]; //X Y and Z
   char cachedOperator = ' ';
   int focused;
   int solution;
   bool willClear;

   void Awake () {
      moduleId = moduleIdCounter++;
      foreach (KMSelectable number in NumberButtons)
         number.OnInteract += delegate () { ButtonPress(number); NumPress(Array.IndexOf(NumberButtons, number)); return false; };
      foreach (KMSelectable op in UnaryOperators)
         op.OnInteract += delegate () { ButtonPress(op); UnaryPress(Array.IndexOf(UnaryOperators, op)); return false; };
      foreach (KMSelectable op in BinaryOperators)
         op.OnInteract += delegate () { ButtonPress(op); BinaryPress(Array.IndexOf(BinaryOperators, op)); return false; };
      foreach (KMSelectable extra in Other)
         extra.OnInteract += delegate () { ButtonPress(extra); OtherPress(Array.IndexOf(Other, extra)); return false; };
      foreach (KMSelectable variable in Variables)
         variable.OnInteract += delegate () { ButtonPress(variable); VarPress(Array.IndexOf(Variables, variable)); return false; };
   }

   void Start () {
      foreach (char letter in Bomb.GetSerialNumber())
         solution += "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(letter);
      Debug.LogFormat("[The Calculator #{0}] The serial number sum in base-36 is {1}.", moduleId, solution);
   }

   void ButtonPress (KMSelectable button) {
      button.AddInteractionPunch(0.1f);
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
   }

   void NumPress (int num) {
      if (willClear) numStrings[focused] = "";
      numStrings[focused] = (numStrings[focused] + num).TakeLast(18).Join("");
      ScreenButText.text = numStrings[focused];
      numbers[focused] = double.Parse(numStrings[focused]);
      willClear = false;
   }

   void UnaryPress (int op) {
      cachedOperator = ' ';
      switch (op) {
         case 0: numbers[focused] = Math.Pow(numbers[focused], 2); break; //Square
         case 1: numbers[focused] = Math.Pow(numbers[focused], 0.5); break; //Sqrt
         case 2: numbers[focused] = DigiRoot(numbers[focused]); break; //DR
         case 3: numbers[focused] *= -1; break; //Negative
      }
      numStrings[focused] = numbers[focused].ToString();
      ScreenButText.text = numStrings[focused];
      if (op != 3)
         willClear = true;

   }

   void BinaryPress (int index) {
      willClear = false;
      if (focused == 1 && numStrings[1] != "") //If there's already an expression inputted, evaluate it
         EvalutateCurrent();
      cachedOperator = "+-*/%"[index];
      focused = 1;
   }

   void VarPress (int index) {
      if (cachedOperator == ' ' && numStrings[focused] != "") //If the display is not empty and we haven't entered an operator, store the display, otherwise display that variable.
      {
         Debug.LogFormat("variable {0} stored", "xyz"[index]);
         cachedVariables[index] = numbers[focused]; //Store
         willClear = true;
      }
      else {
         Debug.LogFormat("variable {0} recalled", "xyz"[index]);
         numbers[focused] = cachedVariables[index]; //Recall
         numStrings[focused] = numbers[focused].ToString();
         ScreenButText.text = numStrings[focused];
      }
   }

   void OtherPress (int index) {
      switch (index) {
         case 0: EvalutateCurrent(); break; //Equals button
         case 1: if (numbers[focused] == solution) GetComponent<KMBombModule>().HandlePass(); break; //Solve button
         case 2: //Delete button
            numStrings[focused] = numStrings[focused].Take(numStrings[focused].Length - 1).Join("");
            numbers[focused] = (numStrings[focused] == "" || numStrings[focused] == "-")
                ? 0
                : double.Parse(numStrings[focused]);
            ScreenButText.text = numStrings[focused];
            break;
         case 3: //Clear button
            numbers[0] = 0;
            numbers[1] = 0;
            numStrings[0] = "";
            numStrings[1] = "";
            ScreenButText.text = "";
            break;
         case 4: //Decimal point;
            if (numStrings[focused].Contains('.'))
               break;
            if (numStrings[focused] == "") numStrings[focused] = "0";
            numStrings[focused] = (numStrings[focused] + '.').TakeLast(18).Join("");
            ScreenButText.text = numStrings[focused];
            numbers[focused] = double.Parse(numStrings[focused]);
            willClear = false;
            break;
         default: break;
      }
   }

   void EvalutateCurrent () {
      numbers[0] = PerformOp(numbers[0], numbers[1], cachedOperator);
      focused = 0;
      cachedOperator = ' ';
      numStrings[0] = numbers[0].ToString();
      numbers[1] = 0;
      numStrings[1] = "";
      ScreenButText.text = numStrings[0];
      willClear = true;

   }

   double PerformOp (double ONE, double TWO, char op) {
      switch (op) {
         case '+': return ONE + TWO;
         case '-': return ONE - TWO;
         case '*': return ONE * TWO;
         case '/':
            if (TWO == 0) {
               StartCoroutine(DisplayError());
               return 0;
            }
            else return ONE / TWO;
         case '%':
            if (TWO == 0) {
               StartCoroutine(DisplayError());
               return 0;
            }
            else return (ONE % TWO + TWO) % TWO;
         default: return ONE;
      }
   }

   double DigiRoot (double input) {
      do {
         var digits = input.ToString().Where(x => x != '.' && x != '-').Select(x => x - '0');
         input = 0;
         foreach (int digit in digits)
            input += digit;
      } while (input > 9);
      return input;
   }

   IEnumerator DisplayError () {
      yield return null;
      numbers[0] = 0;
      numStrings[0] = "";
      numbers[1] = 0;
      numStrings[1] = "";
      ScreenButText.text = "ERROR";
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use <!{0} 63 solve> to enter 63 and press the solve button. Use <!{0} 2 + 2 = > to press those buttons. Substitute √ with sqrt, ² with sq. Square roots, digital roots, and negatives must come before their parameters.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string command) {
      command = command.Trim().ToUpperInvariant();
      List<string> parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
      List<KMSelectable> inputs = new List<KMSelectable>();
      string[] ALLCOMMANDS = "0123456789.=C+-*/%XYZ<²√".Select(x => x.ToString()).Concat(new string[] { "DR", "SQRT", "DEL", "CLR", "CLEAR", "DELETE", "SOLVE", "SUBMIT", "^2" }).ToArray();
      if (parameters.All(x => ALLCOMMANDS.Contains(x) || x.All(ch => "0123456789.-".Contains(ch)))) {
         bool sqrt = false;
         bool dr = false;
         foreach (string param in parameters) {
            if (param == "SQRT" || param == "√")
               sqrt = true;
            if (param == "DR")
               dr = true;
            if (param.All(x => "0123456789.-".Contains(x))) {
               bool negating = false;
               yield return null;
               foreach (char digit in param) {
                  if ("0123456789".Contains(digit))
                     inputs.Add(NumberButtons[digit - '0']);
                  else if (digit == '.')
                     inputs.Add(Other[4]);
                  else negating = !negating;
               }
               if (negating)
                  inputs.Add(UnaryOperators[3]);
               if (sqrt)
                  inputs.Add(UnaryOperators[1]);
               if (dr)
                  inputs.Add(UnaryOperators[2]);
               sqrt = false;
               dr = false;
            }
            else if ("+-*/%".Contains(param))
               inputs.Add(BinaryOperators["+-*/%".IndexOf(param)]);
            else {
               switch (param) {
                  case "SQ": case "^2": case "²": inputs.Add(UnaryOperators[0]); break;
                  case "=": inputs.Add(Other[0]); break;
                  case "<": case "DEL": case "D": inputs.Add(Other[2]); break;
                  case "C": case "CLR": case "CLEAR": inputs.Add(Other[3]); break;
                  case "SOLVE": case "SUBMIT": inputs.Add(Other[1]); break;
                  case "X": inputs.Add(Variables[0]); break;
                  case "Y": inputs.Add(Variables[1]); break;
                  case "Z": inputs.Add(Variables[2]); break;
                  default: break;
               }
            }
         }
         yield return null;
         Debug.Log(inputs.Select(x => x.name).Join(", "));
         foreach (KMSelectable button in inputs) {
            button.OnInteract();
            yield return new WaitForSeconds(0.1f);
         }
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      if (numbers[focused] != 0) {
         Other[3].OnInteract();
         yield return new WaitForSeconds(0.1f);
      }
      foreach (char digit in solution.ToString()) {
         NumberButtons[digit - '0'].OnInteract();
         yield return new WaitForSeconds(0.1f);
      }
      Other[1].OnInteract();
   }
}
