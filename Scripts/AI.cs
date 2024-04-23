using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    List<Paciente> Pacientes = new List<Paciente>();

    [Header ("Variables:")]
    public int NumeroDePacientes = 5;
    public List<string> Problemas;
    public List<string> Razas;
    public List<string> GruposSanguineos;
    public List<string> RH;

    [Header("Estudio")]
    public string ProblemaInvestigando;
    List<Paciente> PacientesEnfermos = new List<Paciente>();
    public float PorcentajeEnfermo;
    public Vector2 RangoEdades= new Vector2(5, 90);
    public List<int> Edades;
    public List<int> RepeticionEdades;
    public List<int> EdadesEnfermasMasRepetidas;
    public List<int> EdadesMasRepetidas;
    public List<string> StringEdadesMasRepetidas;
    public bool CreandoSimulacion;

    [Header("UI:")]
    public InputField InputBuscadorPaciente;
    public InputField InputEnfermedadEstudiar;
    public InputField InputNumeroPacientes;
    public InputField InputEnfermedades;
    public Button BotonCrear;
    public Button BotonIniciarIA;
    public Text TextPoblacionActual;
    public Text TextoBuscadorPaciente;
    public Text TextoPorcentajeEnfermos;
    public Text TextoResultadoFinal;
    public Image ImagenFondo;
    public Gradient GradianteFondo;
    public float VelocidadGradiante;
    public float VelocidadFondo;
    float lerpGradiante;
    bool addingGradiante;
    bool addingPos;
    public int Checks = 10;
    int check;
    Vector2 fondoPos;
    public Image BarraCargaCrearPacientes;
    public Text PorcentajeCargaCrearPacientes;
    public GameObject BotonCancelarGeneración;

    //Función que se ejecuta en el primer frame
    void Start()
    {
        Edades.Clear();

        for (int i = (int)RangoEdades.x; i < (int)RangoEdades.y + 1; i++)
        {
            Edades.Add(i);
        }
    }

    //Función que se ejecuta cada frame
    void Update()
    {
        //Gradiante fondo
        if (lerpGradiante < 0)
        {
            addingGradiante = true;
            lerpGradiante = 0;
        }

        if (lerpGradiante > 1)
        {
            addingGradiante = false;
            lerpGradiante = 1;
        }

        if (addingGradiante)
        {
            lerpGradiante += Time.deltaTime * VelocidadGradiante;
        }
        else
        {
            lerpGradiante -= Time.deltaTime * VelocidadGradiante;
        }

        ImagenFondo.color = GradianteFondo.Evaluate(lerpGradiante);

        //Movimiento fondo
        if (addingPos)
        {
            fondoPos.x += Time.deltaTime * VelocidadFondo;
            fondoPos.y += Time.deltaTime * VelocidadFondo / 1.5f;
        }
        else
        {
            fondoPos.x -= Time.deltaTime * VelocidadFondo;
            fondoPos.y -= Time.deltaTime * VelocidadFondo / 1.5f;
        }

        if(fondoPos.x > 2000)
        {
            addingPos = false;
        }
        if (fondoPos.x < 0)
        {
            addingPos = true;
        }

        ImagenFondo.transform.position = new Vector2(fondoPos.x, fondoPos.y);

        //Actualizar Botones
        if (!CreandoSimulacion)
        {
            InputEnfermedades.interactable = true;

            InputEnfermedadEstudiar.interactable = Pacientes.Count > 0;

            InputBuscadorPaciente.interactable = Pacientes.Count > 0;

            InputNumeroPacientes.interactable = true;

            BotonCrear.interactable = (InputNumeroPacientes.text != "" || Pacientes.Count > 0) && InputEnfermedades.text != "";

            BotonIniciarIA.interactable = Pacientes.Count > 0 && InputEnfermedadEstudiar.text != "";
        }
        else
        {
            InputEnfermedades.interactable = false;
            InputEnfermedadEstudiar.interactable = false;
            InputBuscadorPaciente.interactable = false;
            InputNumeroPacientes.interactable = false;
            BotonCrear.interactable = false;
            BotonIniciarIA.interactable = false;
        }
    }

    //Crea la simulación
    public void CrearSimulacion()
    {
        CreandoSimulacion = true;

        //Resetea la Lista para no interferir con los datos de la simulación anterior
        NumeroDePacientes = int.Parse(InputNumeroPacientes.text);

        //Añadir las enfermedades a estudiar
        int lastChar = -1;
        int actualChar = 0;
        string enfermedad = "";

        Problemas = new List<string>();

        if(InputEnfermedades.text[InputEnfermedades.text.Length - 1] != " ".ToCharArray()[0])
        {
            InputEnfermedades.text += " ";
        }

        for (int i = 0; i < InputEnfermedades.text.Length; i++)
        {
            if (InputEnfermedades.text[i] == ",".ToCharArray()[0] || i == InputEnfermedades.text.Length-1)
            {
                actualChar = i;

                for (int u = lastChar+1; u < actualChar; u++)
                {
                    enfermedad += InputEnfermedades.text[u];
                }

                Problemas.Add(enfermedad);
                enfermedad = "";
                lastChar = actualChar+1;
            }
        }

        Pacientes.Clear();

        InputNumeroPacientes.gameObject.SetActive(false);
        BotonCancelarGeneración.SetActive(true);
        StartCoroutine("GenerarPacientes");
    }

    //Busca pacientes
    public void BuscarPaciente()
    {
        int id = int.Parse(InputBuscadorPaciente.text);

        if (id < Pacientes.Count)
        {
            MostrarFichero(id);
        }
        else
        {
            TextoBuscadorPaciente.text = "*No se ha encontrado al paciente";
        }
    }

    //Mostrar fichero de paciente
    public void MostrarFichero(int PacienteMostrar)
    {
        TextoBuscadorPaciente.text = "Paciente: " + Pacientes[PacienteMostrar].ID.ToString() + "\n \n" +
                    "Problema: " + Pacientes[PacienteMostrar].Problema + "\n" +
                    "Edad: " + Pacientes[PacienteMostrar].Edad.ToString() + " años" + "\n" +
                    "Raza: " + Pacientes[PacienteMostrar].Raza + "\n" +
                    "Peso: " + Pacientes[PacienteMostrar].Peso.ToString() + "kg" + "\n" +
                    "Altura: " + Pacientes[PacienteMostrar].Altura.ToString() + "m" + "\n" +
                    "Grupo Sanguíneo: " + Pacientes[PacienteMostrar].GrupoSanguineo + "\n" +
                    "RH: " + Pacientes[PacienteMostrar].RH + "\n" +
                    "Porcentaje de oxígeno en sangre: " + Pacientes[PacienteMostrar].PorcentajeDeOxigenoEnSangre.ToString() + "%";
    }

    //Ejecuta la IA
    public void GetResult()
    {
        //Resetear los resultados de la simulación anterior
        PacientesEnfermos.Clear();
        /*RepeticionEdades.Clear();
        EdadesEnfermasMasRepetidas.Clear();
        EdadesMasRepetidas.Clear();
        StringEdadesMasRepetidas.Clear();*/

        ProblemaInvestigando = InputEnfermedadEstudiar.text.ToLower();

        //Almacena
        for (int i = 0; i < EdadesEnfermasMasRepetidas.Count; i++)
        {
            EdadesEnfermasMasRepetidas[i] = 0;
        }
        for (int i = 0; i < RepeticionEdades.Count; i++)
        {
            RepeticionEdades[i] = 0;
        }

        //Obtengo los pacientes con el problema que quiero estudiar
        for (int i = 0; i < Pacientes.Count; i++)
        {
            if(Pacientes[i].Problema.ToLower() == ProblemaInvestigando)
            {
                PacientesEnfermos.Add(Pacientes[i]);
            }
        }

        PorcentajeEnfermo = (PacientesEnfermos.Count-1) * 100 / (Pacientes.Count-1);
        TextoPorcentajeEnfermos.text = "El " + PorcentajeEnfermo.ToString() + "% de la población estudiada padece " + ProblemaInvestigando.ToString();

        for (int i = 0; i < PacientesEnfermos.Count; i++)
        {
            for (int u = 0; u < RepeticionEdades.Count; u++)
            {
                if(PacientesEnfermos[i].Edad == Edades[u])
                {
                    RepeticionEdades[u]++;
                    u = RepeticionEdades.Count;
                }
            }
        }

        //Detectar los más repetidos

        //Primer puesto
        StringEdadesMasRepetidas[0] = "-De las personas de ";
        for (int i = 0; i < RepeticionEdades.Count; i++)
        {
            if(RepeticionEdades[i] > EdadesEnfermasMasRepetidas[0])
            {
                StringEdadesMasRepetidas[0] = "-De las personas de ";
                EdadesEnfermasMasRepetidas[0] = RepeticionEdades[i];
                EdadesMasRepetidas[0] = Edades[i];
                StringEdadesMasRepetidas[0] += Edades[i].ToString() + " años";
            }
            else if (RepeticionEdades[i] == EdadesEnfermasMasRepetidas[0])
            {
                StringEdadesMasRepetidas[0] += ", " + Edades[i].ToString() + " años";
            }
        }
        StringEdadesMasRepetidas[0] += " han enfermado " + EdadesEnfermasMasRepetidas[0].ToString() + " personas de cada edad.";

        if (EdadesEnfermasMasRepetidas[0] == 0)
        {
            StringEdadesMasRepetidas[0] = "*No hay personas enfermas en el primer puesto.";
        }

        //Segundo puesto
        StringEdadesMasRepetidas[1] = "-De las personas de ";
        for (int i = 0; i < RepeticionEdades.Count; i++)
        {
            if (RepeticionEdades[i] > EdadesEnfermasMasRepetidas[1] && RepeticionEdades[i] < EdadesEnfermasMasRepetidas[0])
            {
                StringEdadesMasRepetidas[1] = "-De las personas de ";
                EdadesEnfermasMasRepetidas[1] = RepeticionEdades[i];
                EdadesMasRepetidas[1]= Edades[i];
                StringEdadesMasRepetidas[1] += Edades[i].ToString() + " años";
            }
            else if (RepeticionEdades[i] == EdadesEnfermasMasRepetidas[1])
            {
                StringEdadesMasRepetidas[1] += ", " + Edades[i].ToString() + " años";
            }
        }
        StringEdadesMasRepetidas[1] += " han enfermado " + EdadesEnfermasMasRepetidas[1].ToString() + " personas de cada edad.";

        if (EdadesEnfermasMasRepetidas[1] == 0)
        {
            StringEdadesMasRepetidas[1] = "*No hay personas enfermas en el segundo puesto.";
        }

        //Tercer puesto
        StringEdadesMasRepetidas[2] = "-De las personas de ";
        for (int i = 0; i < RepeticionEdades.Count; i++)
        {
            if (RepeticionEdades[i] > EdadesEnfermasMasRepetidas[2] && RepeticionEdades[i] < EdadesEnfermasMasRepetidas[1])
            {
                StringEdadesMasRepetidas[2] = "-De las personas de ";
                EdadesEnfermasMasRepetidas[2] = RepeticionEdades[i];
                EdadesMasRepetidas[2] = Edades[i];
                StringEdadesMasRepetidas[2] += Edades[i].ToString() + " años";
            }
            else if (RepeticionEdades[i] == EdadesEnfermasMasRepetidas[2])
            {
                StringEdadesMasRepetidas[2] += ", " + Edades[i].ToString() + " años";
            }
        }
        StringEdadesMasRepetidas[2] += " han enfermado " + EdadesEnfermasMasRepetidas[2].ToString() + " personas de cada edad.";

        if (EdadesEnfermasMasRepetidas[2] == 0)
        {
            StringEdadesMasRepetidas[2] = "*No hay personas enfermas en el tercer puesto.";
        }

        //Cuarto puesto
        StringEdadesMasRepetidas[3] = "-De las personas de ";
        for (int i = 0; i < RepeticionEdades.Count; i++)
        {
            if (RepeticionEdades[i] > EdadesEnfermasMasRepetidas[3] && RepeticionEdades[i] < EdadesEnfermasMasRepetidas[2])
            {
                StringEdadesMasRepetidas[3] = "-De las personas de ";
                EdadesEnfermasMasRepetidas[3] = RepeticionEdades[i];
                EdadesMasRepetidas[3] = Edades[i];
                StringEdadesMasRepetidas[3] += Edades[i].ToString() + " años";
            }
            else if (RepeticionEdades[i] == EdadesEnfermasMasRepetidas[3])
            {
                StringEdadesMasRepetidas[3] += ", " + Edades[i].ToString() + " años";
            }
        }
        StringEdadesMasRepetidas[3] += " han enfermado " + EdadesEnfermasMasRepetidas[3].ToString() + " personas de cada edad.";

        if (EdadesEnfermasMasRepetidas[3] == 0)
        {
            StringEdadesMasRepetidas[3] = "*No hay personas enfermas en el cuarto puesto.";
        }

        TextoResultadoFinal.text = StringEdadesMasRepetidas[0] + "\n" + "\n" + StringEdadesMasRepetidas[1] + "\n" + "\n" + StringEdadesMasRepetidas[2] + "\n" + "\n" + StringEdadesMasRepetidas[3];
    }

    IEnumerator GenerarPacientes()
    {
        //Designa el número de Pacientes que habrá en ella
        if (Pacientes.Count < NumeroDePacientes)
        {
            for (int i = 0; i < NumeroDePacientes; i++)
            {
                Pacientes.Add(new Paciente());

                BarraCargaCrearPacientes.fillAmount = Pacientes.Count * 0.9f / NumeroDePacientes;
                PorcentajeCargaCrearPacientes.text = (Pacientes.Count * 90 / NumeroDePacientes).ToString() + "%";

                if(check >= Checks)
                {
                    check = 0;
                    yield return null;
                }
                else
                {
                    check += 1;
                }
            }
        }

        check = 0;

        if(Pacientes.Count == NumeroDePacientes)
        {
            //Aleatoriza sus rasgos
            for (int i = 0; i < Pacientes.Count; i++)
            {
                Pacientes[i].ID = i;
                Pacientes[i].Problema = Problemas[Random.Range(0, Problemas.Count)];
                Pacientes[i].Edad = (int)Random.Range(Pacientes[i].Edad_Aleatoria.x, Pacientes[i].Edad_Aleatoria.y);
                Pacientes[i].Raza = Razas[Random.Range(0, Razas.Count)];

                //Peso *Comprobar
                if (Pacientes[i].Edad >= 5 && Pacientes[i].Edad <= 10)
                {
                    Pacientes[i].Peso = Random.Range(35, 40);
                }
                if (Pacientes[i].Edad >= 11 && Pacientes[i].Edad <= 20)
                {
                    Pacientes[i].Peso = Random.Range(45, 70);
                }
                if (Pacientes[i].Edad >= 21 && Pacientes[i].Edad <= 50)
                {
                    Pacientes[i].Peso = Random.Range(60, 90);
                }

                //Altura *Comprobar
                if (Pacientes[i].Edad >= 5 && Pacientes[i].Edad <= 10)
                {
                    Pacientes[i].Altura = Random.Range(1, 1.4f);
                }
                if (Pacientes[i].Edad >= 11 && Pacientes[i].Edad <= 20)
                {
                    Pacientes[i].Altura = Random.Range(1.5f, 1.8f);
                }
                if (Pacientes[i].Edad >= 21 && Pacientes[i].Edad <= 50)
                {
                    Pacientes[i].Altura = Random.Range(1.7f, 2.1f);
                }

                Pacientes[i].GrupoSanguineo = GruposSanguineos[Random.Range(0, GruposSanguineos.Count)];
                Pacientes[i].RH = RH[Random.Range(0, RH.Count)];
                Pacientes[i].PorcentajeDeOxigenoEnSangre = (int)Random.Range(Pacientes[i].PorcentajeDeOxigenoEnSangre_Aleatoria.x, Pacientes[i].PorcentajeDeOxigenoEnSangre_Aleatoria.y);

                BarraCargaCrearPacientes.fillAmount = 0.9f + (i * 0.1f / NumeroDePacientes);
                PorcentajeCargaCrearPacientes.text = (90 + (i * 10 / NumeroDePacientes)).ToString() + "%";

                if (check >= Checks)
                {
                    check = 0;
                    yield return null;
                }
                else
                {
                    check += 1;
                }
            }
        }

        BarraCargaCrearPacientes.fillAmount = 1;
        PorcentajeCargaCrearPacientes.text = "100%";
        TextPoblacionActual.text = "Población actual: " + Pacientes.Count.ToString() + " personas";
        InputNumeroPacientes.gameObject.SetActive(true);
        BotonCancelarGeneración.SetActive(false);
        CreandoSimulacion = false;
    }

    public void CancelarGeneracion()
    {
        StopCoroutine("GenerarPacientes");
        Pacientes = new List<Paciente>();
        TextPoblacionActual.text = "Población actual: (no hay)";
        InputNumeroPacientes.gameObject.SetActive(true);
        BotonCancelarGeneración.SetActive(false);
        CreandoSimulacion = false;
    }

    public void Salir()
    {
        Application.Quit();
    }
}
