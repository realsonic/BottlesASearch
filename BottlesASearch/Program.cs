BottleSet startSet = new(
    Bottle10: new Bottle(Capacity: 10, Amount: 0),
    Bottle5: new Bottle(Capacity: 5, Amount: 5),
    Bottle6: new Bottle(Capacity: 6, Amount: 6));

PriorityQueue<BottleSet, int> setQueue = new(items: new [] { (Element: startSet, Priority: startSet.GoalDistance) } );

List<BottleSet> winSets = [];

while (setQueue.TryDequeue(out BottleSet? set, out int _))
{
    Console.WriteLine($"Checking set: {set.Image}...");
    if (set.IsGoal)
    {
        winSets.Add(set);
        Console.WriteLine($"Win set: {set.Image}");
        continue;
    }

    IEnumerable<BottleSet> nextMoves = set.GetNextMoves();
    setQueue.EnqueueRange(nextMoves.Select(set => (Element: set, Priority: set.GoalDistance)));
}

record Bottle(byte Capacity, byte Amount)
{
    byte RemainCapacity => (byte)(Capacity - Amount);
    public (Bottle From, Bottle To) Pour(Bottle to)
    {
        byte pouringQuantity = Math.Min(Amount, to.RemainCapacity);
        return (
            From: this with { Amount = (byte)(Amount - pouringQuantity) },
            To: to with { Amount = (byte)(Amount + pouringQuantity) }
        );
    }
}

record BottleSet(Bottle Bottle10, Bottle Bottle5, Bottle Bottle6)
{
    public BottleSet? PreviousSet {get; init;} = null;
    public bool IsGoal => Bottle10.Amount == 8;
    public int GoalDistance => Math.Abs(Bottle10.Amount - 8);
    public string Image => $"{{{Bottle10.Amount}}} {{{Bottle5.Amount}}} {{{Bottle6.Amount}}}";
    public IEnumerable<BottleSet> GetNextMoves() {
        (Bottle newBottle10, Bottle newBottle5) = Bottle10.Pour(Bottle5);
        yield return this with { Bottle10 = newBottle10, Bottle5 = newBottle5, PreviousSet = this };
    }
}

