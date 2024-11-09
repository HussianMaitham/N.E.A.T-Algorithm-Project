using UnityEngine;

/// <summary>
/// This script is responsible for the entire simulation, it simply is responsible for creating the N.E.A.T algorithm itself
/// It handles the population, mutation, evaluation, crossover, and literally everything about the algorithm
/// </summary>

public class SimulationManager : MonoBehaviour
{
    // --------------------------------- Inspector Variables ---------------------------------
    [Header("Simulation Settings")]
    public string targetNetworks;
    public int populationSize;
    [Header("Training Configuration")]
    public SessionMode sessionMode;
    public float learningRate;
    [Header("Other")]
    public bool showReports;

    // --------------------------------- Private Variables ---------------------------------
    public enum SessionMode
    {
        Training,
        TrainedModel,
        ManualControl
    }

    int currentInnovationNumber;
    int generationNumber;

    // ------------------------------- Code starts here -------------------------------
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
