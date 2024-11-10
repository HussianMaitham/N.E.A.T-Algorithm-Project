/// <summary>
/// This is the neuron (node) class, an instance of this class holds an ID number and a value
/// this is used for both the input, hidden, and output layer
/// </summary>

public class Neuron
{
    public int ID;
    public float value;
    public string nodeType;
    public bool hasIncomingConnection;

    // Ask for the neuron ID, value, type, and its incoming connection bool when an instance is created
    public Neuron(int ID, float value, string nodeType, bool hasIncomingConnection)
    {
        ID                    = this.ID;
        value                 = this.value;
        nodeType              = this.nodeType;
        hasIncomingConnection = this.hasIncomingConnection;
    }
}
