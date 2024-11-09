using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the Neural Network class, it is used for the decision making and initialization of the network and holds all the mutation functions
/// but note that this is not responsible for the mutations themself but they are rather made by the simulation manager. things 
/// like inputs, outputs are all managed by any monobehaviour script preferably called as "behaviour"
/// </summary>

public class NeuralNetwork : MonoBehaviour
{
    // --------------------- Main ---------------------
    [Header("Main")]
    public string networkName;
    // --------------------- Inputs and Outputs ---------------------
    public int inputLayerSize, outputLayerSize;
    [HideInInspector] public float[] inputs, outputs;

    // --------------------- Genome ---------------------
    private Neuron[] neurons;
    private Connection[] connections;

    // --------------------- Other ---------------------
    [HideInInspector] public float currentFitness;
    int currentNeuronID;

    // ------------------------------- Code starts here -------------------------------
    /// <summary>
    /// initializes the network, and makes a simple brain with the inputs, outputs, and set the weights and biases to zero
    /// </summary>
    public void initialize()
    {
        // Set the inputs and outputs arrays to the desired size
        inputs  = new float[inputLayerSize];
        outputs = new float[outputLayerSize];

        // Generates the input and output neurons
        for (int i = 0; i < inputs.Length; i++)
        {
            addNeuron(); // Adds the input neurons to the array
        }

        for (int i = 0; i < outputs.Length; i ++)
        {
            addNeuron(); // Adds the output neurons to the array
        }

        // We also add an additional bias node, incase the output needs to be higer than 0 even if the inputs are all 0
        addNeuron();
    }

    // ----------------------------------------------------------------- Basic Functions -----------------------------------------------------------------
    /// <summary>
    /// Adds a new neuron to the neurons array in the genome
    /// </summary>
    private void addNeuron()
    {
        currentNeuronID++; // Increase the current neuron ID by one, this makes it easier to track the neurons in the array

        // First we turn our neurons array to a list, this makes it easier to add a new object to it
        List<Neuron> neuronsList = neurons.ToList();

        // Then we add a new neuron object, with its ID being the current neuron ID and its value being set to zero
        neuronsList.Add(new Neuron(currentNeuronID, 0));

        // After that we set the neurons array to our new list of neurons we just created
        neurons = neuronsList.ToArray();
    }

    /// <summary>
    /// Adds a new connection to the connections array in the genome, 
    /// It links two neurons together
    /// </summary>
    private void addConnection(int fromNeuronID, int toNeuronID, float weight, bool enabled, int innov)
    {
        // First we turn our connections array to a list, this makes it easier to add a new object to it
        List<Connection> connectionsList = connections.ToList();

        // Then we add a new connection object, with all the variables needed for it
        connectionsList.Add(new Connection(fromNeuronID, toNeuronID, weight, enabled, innov));

        // After that we set the connections array to our new list of connections we just created
        connections = connectionsList.ToArray();
    }

    /// <summary>
    /// Removes a neuron from the neuron array in the genome
    /// </summary>
    private void removeNeuron(int index)
    {
        // First we turn our neurons array to a list, this makes it easier to remove an object from it
        List<Neuron> neuronsList = neurons.ToList();

        // Then we remove that neuron based on the required index number
        neuronsList.RemoveAt(index);

        // After that we set the neurons array to our new list of neurons we just modified
        neurons = neuronsList.ToArray();
    }

    /// <summary>
    /// Removes a connection from the connections array in the genome
    /// </summary>
    private void removeConnection(int index)
    {
        // First we turn our connections array to a list, this makes it easier to remove an object from it
        List<Connection> connectionsList = connections.ToList();

        // Then we remove that connection based on the required index number
        connectionsList.RemoveAt(index);

        // After that we set the connections array to our new list of connections we just modified
        connections = connectionsList.ToArray();
    }

    // ----------------------------------------------------------------- Mutation Functions -----------------------------------------------------------------

    /// <summary>
    /// Mutates a random connection's weight in the connections array in the genome
    /// </summary>
    public void mutateWeight(float learningRate)
    {
        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Check if the random connection we generated is null
        if (connections[randomConnection] == null)
        {
            // If so, we quit the operation
            return;
        }

        // We then mutate the choosen connection by the learning rate
        connections[randomConnection].weight += Random.Range(-learningRate, learningRate);
    }

    /// <summary>
    /// Mutate a new connection, which simply means create a new random connection between two random nodes
    /// </summary>
    public void mutateConnection(float learningRate, int innov)
    {
        // Takes two random neurons from the array
        int randomFromNeuronID = Random.Range(0, neurons.Length);
        int randomToNeuronID   = Random.Range(0, neurons.Length);

        // Make sure that they are not both the same, otherwise a neuron will somehow connect to itself
        if (randomFromNeuronID == randomToNeuronID)
        {
            // If so, we quit the operation
            return;
        }

        // Generates a random weight value within the learning rate range
        float randomWeight = Random.Range(-learningRate, learningRate);

        // Finally we create the new random connection and add it to the connection list in the genome
        addConnection(randomFromNeuronID, randomToNeuronID, randomWeight, true, innov);
    }

    /// <summary>
    /// Splits a random connection into two and add a hidden neuron in between of them
    /// </summary>
    public void addHiddenNeuron(float learningRate, int innov)
    {
        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Check if the random connection we generated is null
        if (connections[randomConnection] == null)
        {
            // If so, we quit the operation
            return;
        }

        // Takes a new copy of the connection to keep its informations
        Connection copiedConnection = connections[randomConnection];

        // then we simply remove the connection as it is no longer necessary, we do this using a list
        removeConnection(randomConnection);
        
        // Create the new hidden neuron
        addNeuron();

        // We create two connections, the first is fromNeuronID to the hidden neuron
        addConnection(copiedConnection.fromNeuronID, connections.Length, 1, true, innov);

        // The second is from the new hidden neuron to the toNeuronID
        addConnection(connections.Length, copiedConnection.toNeuronID, copiedConnection.weight, true, innov);
    }

    /// <summary>
    /// Mutate a connection is_enabled boolean and set it to either true or false
    /// </summary>
    public void mutateIsEnabled()
    {
        // Chooses a random value from between 0 (false) and 1 (true)
        int randomEnableState = Random.Range(0, 1);

        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Check if the random connection we generated is null
        if (connections[randomConnection] == null)
        {
            // If so, we quit the operation
            return;
        }

        // If that random number is 0 then we set the enabled boolean to false
        if (randomEnableState == 0)
        {
            connections[randomConnection].enabled = false;
        }
        // If it was otherwise 1, then we set the enabled boolean to true
        else if (randomEnableState == 1)
        {
            connections[randomConnection].enabled = true;
        }
    }

    /// <summary>
    /// Mutate (remove) a random neuron from the neurons in the genome
    /// </summary>
    public void mutateRemoveNeuron()
    {
        // Choses a random neuron from the neurons array in the genome
        int randomNeuron = Random.Range(0, neurons.Length);

        // Check if the random connection we generated is null
        if (neurons[randomNeuron] == null)
        {
            // If so, we quit the operation
            return;
        }

        // Remove that random neuron
        removeNeuron(randomNeuron);
    }

    /// <summary>
    /// Mutate (remove) a random connection from the connections array in the genome
    /// </summary>
    public void mutateRemoveConnection()
    {
        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Check if the random connection we generated is null
        if (connections[randomConnection] == null)
        {
            // If so, we quit the operation
            return;
        }

        // Remove that random connection
        removeConnection(randomConnection);
    }
}
