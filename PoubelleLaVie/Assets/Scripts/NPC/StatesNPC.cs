public enum DrunkState
{
    PUKER,
    LOVER,
    DANCER,
    TOTAL_DRUNK_STATES
}

enum GlobalState
{
    NEED_DRINKING = 1,
    FINE = 2,
    DRUNK = 4,
    BEING_CARRIED = 8 // When the player is taking care of this NPC
}

enum ActionState
{
    IDLE,
    WALKING,
}