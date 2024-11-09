/// <summary>
/// This is the Neural Network class, this is used for the decision making and initialization of the network and holds all the mutation functions
/// but note that this is not responsible for the mutations themself but they are rather made by the simulation manager
/// things like inputs, outputs are all managed by any monobehaviour preferably called as "behaviour"
/// </summary>
public class NeuralNetwork
{
    // --------------------- Inputs and Outputs ---------------------
    public int inputLayerSize, outputLayerSize;
    public float[] inputs, outputs;

    // --------------------- Genome ---------------------
    private Neuron[] neurons;
    private Connection[] connections;

    // --------------------- Other ---------------------
    float currentNeuronID;
    float currentNeuronCount;


    // --------------------- Code starts here---------------------
    /// <summary>
    /// initializes the network, and makes a simple brain with the inputs, outputs, and set the weights and biases to zero
    /// </summary>
    public void initialize()
    {
        // Set the inputs and outputs arrays to the desired size
        inputs = new float[inputLayerSize];
        outputs = new float[outputLayerSize];

        // Generates the input and output neurons
        for (int i = 0; i < inputs.Length; i++)
        {
            currentNeuronID++;
        }

        for (int i = 0; i < outputs.Length; i ++)
        {
            currentNeuronID++;
        }
    }

    /// <summary>
    /// Adds a new neuron to the neurons array in the genome
    /// </summary>
    private void addNeuron()
    {

    }
}
