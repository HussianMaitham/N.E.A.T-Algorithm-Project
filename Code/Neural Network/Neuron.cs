/// <summary>
/// This is the neuron (node) class, an instance of this class holds an ID number and a value
/// this is used for both the input, hidden, and output layer
/// </summary>

public class Neuron
{
    public int ID;
    public float value;

    public Neuron(int ID, float value) // Ask for the neuron ID and value when an instance is created
    {
        ID    = this.ID;
        value = this.value;
    }
}
