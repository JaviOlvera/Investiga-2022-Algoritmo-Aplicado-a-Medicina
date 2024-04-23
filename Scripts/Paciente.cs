using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paciente : MonoBehaviour
{
	public int ID = 1;                             // ID del paciente (0, 1, 2, 3...)
	public string Problema = "Cancer";             // Enfermedad / síntoma...
	public int Edad = 20;                          // años
	public Vector2 Edad_Aleatoria = new Vector2(5, 90);
	public string Raza = "Negro";                  // Blanco / Negro
	public float Peso = 60;                          // kilogramos
	public Vector2 Peso_Aleatorio = new Vector2(14, 70);
	public float Altura = 1.7f;                       // metros
	public Vector2 Altura_Aleatoria = new Vector2(1.2f, 2.7f);
	public string GrupoSanguineo = "A";            // A / B / 0 / AB
	public string RH = "positivo";                  // positivo / negativo
	public int PorcentajeDeOxigenoEnSangre = 99;   // x/100
	public Vector2 PorcentajeDeOxigenoEnSangre_Aleatoria = new Vector2(95, 100);
}
