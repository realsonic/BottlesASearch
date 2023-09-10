Step startStep = new() {
    BottleSet = new BottleSet(
        Bottle10: new Bottle(Capacity: 10, Amount: 0),
        Bottle5: new Bottle(Capacity: 5, Amount: 5),
        Bottle6: new Bottle(Capacity: 6, Amount: 6))
};

PriorityQueue<Step, byte> setQueue = new(items: new [] { (Element: startStep, Priority: startStep.GoalDistance) } );

List<Step> winSteps = [];

long count = 0;

while (setQueue.TryDequeue(out Step? step, out byte _))
{
    count++;

    if (step.IsGoal)
    {
        winSteps.Add(step);
        Console.WriteLine($"Win path: {step.Image} for {step.StepsSoFar} steps.");
        continue;
    }

    IEnumerable<Step> nextSteps = step.GetNextSteps();
    setQueue.EnqueueRange(nextSteps.Select(step => (Element: step, Priority: step.GoalDistance)));
}
Console.WriteLine($"Total iterations: {count}.");

Step? bestStep = winSteps.OrderBy(step => step.StepsSoFar).FirstOrDefault();
Console.WriteLine($"The best solution: {bestStep?.Image ?? "NO"} for {bestStep?.StepsSoFar} steps:\n{bestStep.Path}");

class Step {
    public required BottleSet BottleSet { get; init; }
    public Step? PreviousStep { get; init; }
    public bool IsGoal => BottleSet.IsGoal;
    public byte GoalDistance => BottleSet.GoalDistance;
    public string Image => BottleSet.Image;

    public IEnumerable<Step> GetNextSteps() {
        foreach(BottleSet nextMove in BottleSet.GetAllPossibleMoves()) {
            if (nextMove == BottleSet) continue;
            if (HasInPrevious(nextMove)) continue;
            yield return new Step {
                BottleSet = nextMove,
                PreviousStep = this
            };
        }
    }

    protected bool HasInPrevious(BottleSet bottleSet) => PreviousStep is not null && (PreviousStep.BottleSet == bottleSet || PreviousStep.HasInPrevious(bottleSet));

    public long StepsSoFar => 1 + (PreviousStep is null ? 0 : PreviousStep.StepsSoFar);

    public string Path => $"{Image} <- {PreviousStep?.Path ?? "[Start]"}";
}

record BottleSet(Bottle Bottle10, Bottle Bottle5, Bottle Bottle6)
{
    public bool IsGoal => Bottle10.Amount == 8;
    public byte GoalDistance => (byte)Math.Abs(Bottle10.Amount - 8);
    public string Image => $"{{{Bottle10.Amount}}} {{{Bottle5.Amount}}} {{{Bottle6.Amount}}}";
    
    public IEnumerable<BottleSet> GetAllPossibleMoves() {
        Bottle newBottle10, newBottle5, newBottle6;

        (newBottle10, newBottle5) = Bottle10.Pour(Bottle5);
        yield return (this with { }) with { Bottle10 = newBottle10, Bottle5 = newBottle5 };
        (newBottle10, newBottle6) = Bottle10.Pour(Bottle6);
        yield return (this with { }) with { Bottle10 = newBottle10, Bottle6 = newBottle6 };

        (newBottle5, newBottle10) = Bottle5.Pour(Bottle10);
        yield return (this with { }) with { Bottle10 = newBottle10, Bottle5 = newBottle5 };
        (newBottle5, newBottle6) = Bottle5.Pour(Bottle6);
        yield return (this with { }) with { Bottle5 = newBottle5, Bottle6 = newBottle6 };

        (newBottle6, newBottle10) = Bottle6.Pour(Bottle10);
        yield return (this with { }) with { Bottle10 = newBottle10, Bottle6 = newBottle6 };
        (newBottle6, newBottle5) = Bottle6.Pour(Bottle5);
        yield return (this with { }) with { Bottle5 = newBottle5, Bottle6 = newBottle6 };
    }
}

record Bottle(byte Capacity, byte Amount)
{
    byte RemainCapacity => (byte)(Capacity - Amount);
    public (Bottle From, Bottle To) Pour(Bottle to)
    {
        byte pouringQuantity = Math.Min(Amount, to.RemainCapacity);
        return (
            From: this with { Amount = (byte)(Amount - pouringQuantity) },
            To: to with { Amount = (byte)(to.Amount + pouringQuantity) }
        );
    }
}