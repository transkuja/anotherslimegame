using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Custom/DifficultySo", order = 2)]
public class DiffcultySO : ScriptableObject
{
   public DifficultyParameters[] difficultyParameters;
}
