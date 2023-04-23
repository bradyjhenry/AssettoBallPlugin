using JetBrains.Annotations;
using System.Numerics;

namespace AssettoBallPlugin;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]

public class AssettoBallConfiguration
{
    public GameBallConfiguration GameBall { get; init; } = new();
    public GameStageConfiguration GameStage { get; init; } = new();
}

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]

public class GameBallConfiguration
{
    public int Radius { get; set; } = 1;
    public Vector3 StartingPosition { get; set; } = new Vector3(0, 50, 0);
}

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]

public class GameStageConfiguration 
{ 
    public int Width { get; set; } = 200;
    public int Length { get; set; } = 200;
    public int Height { get; set; } = 20;

}

public class GameStateConfiguration 
{
    public int MinPlayers { get; set; } = 1;
    public int MaxScore { get; set; } = 10;
}


