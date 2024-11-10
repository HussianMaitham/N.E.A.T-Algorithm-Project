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

    // ---------------------------------------------- Code starts here ----------------------------------------------
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
            addNeuron("input"); // Adds the input neurons to the array
        }

        for (int i = 0; i < outputs.Length; i ++)
        {
            addNeuron("output"); // Adds the output neurons to the array
        }

        // We also add an additional bias node, incase the output needs to be higer than 0 even if the inputs are all 0
        addNeuron("input");
    }

    /// <summary>
    /// This calculates the outputs based on the network inputs and hidden neurons, by using the weights of each of their connections
    /// </summary>
    public void feedForward()
    {
        // We obtain the sorted list of connections
        Connection[] _connections = sortConnections(connections);

        // Then we go through each one of the connections
        for (int i = 0; i < _connections.Length; i++)
        {
            // We access the fromNeuron and the ToNeuron that this connection has
            Neuron fromNeuron = neurons[_connections[i].fromNeuronID];
            Neuron toNeuron   = neurons[_connections[i].toNeuronID];

            // And we calculate the toNeuron value using some basic math
            toNeuron.value += tanh(fromNeuron.value * _connections[i].weight);
        }

        // After we were done with the calculations, we feed every neuron value with the type of "output" to the output float list
        for (int i = 0; i < neurons.Length; i++)
        {
            if (neurons[i].nodeType == "output") // Checks if the neuron is an output neuron
            {
                // Add output neuron value to the outputs list
                outputs.ToList().Add(neurons[i].value);
            }
        }
    }

    /// <summary>
    /// This returns the sorted array of connections using the Topological Sorting algorithm
    /// </summary>
    private Connection[] sortConnections(Connection[] _connectionsToSort)
    {
        // First we create a list that contains the sorted connections
        List<Connection> sortedList = new List<Connection>();

        // Creates a copy of the current connections so we can work with it
        List<Connection> connectionsList = _connectionsToSort.ToList();

        // We make sure we keep sorting the arrays as long the list is not empty
        while (connectionsList.Count != 0)
        {
            // Go through each neuron
            for (int i = 0; i < neurons.Length; i++)
            {
                // Go through each connection
                for (int k = 0; k < connections.Length; k++)
                {
                    // We check if that neuron has any connection with a toNeuronID the same as its own ID, We use this to check if the neuron has any
                    // incomig edge or not, if it doesn't, we add it to the sorted list first, if it does, however, we skip it
                    if (neurons[i].ID == connections[i].toNeuronID)
                    {
                        // We say that this neuron has an incoming connection, we skip those neurons when we add the first sorted connection list
                        neurons[i].hasIncomingConnection = true;
                    }
                }
            }

            // After we are done classifying each neuron on wether it had an incoming edge or not, we add those neuron connection to the sorted list
            for (int i = 0; i < neurons.Length; i++)
            {
                // Only chose the neurons that don't have an incoming connection
                if (neurons[i].hasIncomingConnection != true)
                {
                    // for the neurons that don't have an incoming connection, we search through all the connections and add the ones
                    // that connect to his neuron from the "toNeuronID" variable, and add them to the sorted list
                    for (int k = 0; k < connections.Length; k++)
                    {
                        if (neurons[i].ID == connections[k].fromNeuronID)
                        {
                            sortedList.Add(connections[k]);
                        }
                    }
                }
            }

            // After finishing adding the sorted connections, we deleted all the connections of the neurons that had no incoming edges from the "connections List" list
            for (int i = 0; i < sortedList.ToArray().Length; i++)
            {
                connectionsList.Remove(sortedList[i]);
            }

            // Set all the neurons "hasIncomingEdge" back to false so we can check them again with the new updated list after deleting the sorted neurons
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].hasIncomingConnection = false;
            }

            // We then repeat the same process until we obtain the sorted list...
        }

        // Returns the sorted connections list
        return sortedList.ToArray();
    }

    // ----------------------------------------------------------------- Basic Functions -----------------------------------------------------------------
    /// <summary>
    /// Adds a new neuron to the neurons array in the genome
    /// </summary>
    private void addNeuron(string _nodeType)
    {
        currentNeuronID++; // Increase the current neuron ID by one, this makes it easier to track the neurons in the array

        // First we turn our neurons array to a list, this makes it easier to add a new object to it
        List<Neuron> neuronsList = neurons.ToList();

        // Then we add a new neuron object, with its ID being the current neuron ID and its value being set to zero
        neuronsList.Add(new Neuron(currentNeuronID, 0, _nodeType, false));

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

    /// <summary>
    /// The tanh activation function, it maps the value from -1 to 1 and makes the output non-linear
    /// </summary>
    private float tanh(float x)
    {
        // Some epic math here
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
    }

    // ----------------------------------------------------------------- Mutation Functions -----------------------------------------------------------------

    /// <summary>
    /// Mutates a random connection's weight in the connections array in the genome
    /// </summary>
    public void mutateWeight(float learningRate)
    {
        // First we check if the connections array is empty
        if (connections.Length == 0)
        {
            // If so, we quit the operation
            return;
        }
        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // We then mutate the choosen connection by the learning rate
        connections[randomConnection].weight += Random.Range(-learningRate, learningRate);
    }

    /// <summary>
    /// Mutate a new connection, which simply means create a new random connection between two random nodes
    /// </summary>
    public void mutateConnection(float learningRate, int innov)
    {
        // The numbers we use to determine from where and to where the connection goes
        int randomFromNeuronID;
        int randomToNeuronID;

        // Booleans to make sure that the input and output don't connect to themselves, or even a neuron that connects to itself
        bool selfConnect       = false;
        bool inputSelfConnect  = false;
        bool outputSelfConnect = false;

        // Generates a random number until all conditions are met, In case of failure to meet these conditions we try again
        do
        {
            // Takes two random neurons from the array
            randomFromNeuronID = Random.Range(0, neurons.Length);
            randomToNeuronID   = Random.Range(0, neurons.Length);

            // Check if the input neurons don't connect to themselves
            if (neurons[randomFromNeuronID].nodeType == "input"  && neurons[randomToNeuronID].nodeType == "input") {inputSelfConnect = true;}

            // Check if the output neurons don't connect to themselves
            else if (neurons[randomFromNeuronID].nodeType == "output"  && neurons[randomToNeuronID].nodeType == "output") {outputSelfConnect = true;}

            // In case it wasn't neither of these situations, then they are all false
            else
            {
                inputSelfConnect = false;
                outputSelfConnect = false;
            }

            // Check if the from neuron ID and the to neuron ID are not the same, otherwise a neuron will connect to itself
            if (randomFromNeuronID == randomToNeuronID) {selfConnect = true;} else {selfConnect = false;}
        }
        // If the generated numbers don't satisfy the conditions, we generate an another number
        while (selfConnect && inputSelfConnect && outputSelfConnect);

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
        // First we check if the connections array is empty
        if (connections.Length == 0)
        {
            // If so, we quit the operation
            return;
        }

        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Takes a new copy of the connection to keep its informations
        Connection copiedConnection = connections[randomConnection];

        // then we simply remove the connection as it is no longer necessary, we do this using a list
        removeConnection(randomConnection);
        
        // Create the new hidden neuron
        addNeuron("hidden");

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
        // First we check if the connections array is empty
        if (connections.Length == 0)
        {
            // If so, we quit the operation
            return;
        }

        // Chooses a random value from between 0 (false) and 1 (true)
        int randomEnableState = Random.Range(0, 1);

        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

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
    /// Mutate (remove) a random hidden neuron from the neurons in the genome
    /// </summary>
    public void mutateRemoveNeuron()
    {
        // First we check if the neurons array is empty
        if (neurons.Length == 0)
        {
            // If so, we quit the operation
            return;
        }

        // Choses a random hidden neuron from the neurons array in the genome
        int randomNeuron = Random.Range(inputLayerSize + outputLayerSize + 1, neurons.Length);

        // Remove that random neuron
        removeNeuron(randomNeuron);
    }

    /// <summary>
    /// Mutate (remove) a random connection from the connections array in the genome
    /// </summary>
    public void mutateRemoveConnection()
    {
        // First we check if the connections array is empty
        if (connections.Length == 0)
        {
            // If so, we quit the operation
            return;
        }

        // Choses a random connection from the genome
        int randomConnection = Random.Range(0, connections.Length);

        // Remove that random connection
        removeConnection(randomConnection);
    }
}
