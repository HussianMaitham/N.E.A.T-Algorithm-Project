/// <summary>
/// the is the connection (weight) class, it connects two neurons (nodes), and has a weight value
/// used for the feed forward calculations, a bool to determine if the connection is active, and the innovation number is used for the crossover
/// </summary>

public class Connection
{
    public int fromNeuronID;
    public int toNeuronID;
    public float weight;
    public bool enabled;
    public int innov;

    // Ask for the connection crucial variables when an instance is created
    public Connection(int fromNeuronID, int toNeuronID, float weight, bool enabled, int innov)
    {
        fromNeuronID = this.fromNeuronID;
        toNeuronID   = this.toNeuronID;
        enabled      = this.enabled;
        weight       = this.weight;
        innov        = this.innov;
    }
}
