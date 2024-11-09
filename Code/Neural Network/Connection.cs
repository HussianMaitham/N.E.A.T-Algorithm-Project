/// <summary>
/// the is the connection (weight) class, it connects two neurons (nodes), and has a weight value
/// used for the feed forward calculations, and the innovation number is used for the crossover
/// </summary>
public class Connection
{
    public int fromNeuronID;
    public int toNeuronID;
    public float weight;
    public int innov;
    public Connection(int fromNeuronID, int toNeuronID, float weight, int innov) // Ask for the connection crucial variables when an instance is created
    {
        fromNeuronID = this.fromNeuronID;
        toNeuronID   = this.toNeuronID;
        weight       = this.weight;
        innov        = this.innov;
    }
}
