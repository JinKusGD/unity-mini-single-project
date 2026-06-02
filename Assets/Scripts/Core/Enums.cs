public enum InputCallbackType
{
    None = 0,
    Started,
    Performed,
    Canceled
}

public enum InputActionType
{
    PlayerMove,
    PlayerDash
}

public enum UIRoot
{
    Hud,
    Main,
    Content,
    Popup,
    System,
    TopMost
}

public enum UIType
{
    DamagePopup
}

public enum SkillPattern
{
    Projectile,     // 직선 발사
    Homing,         // 유도
    Orbiting,       // 공전
    Area,           // 범위형
    RandomTarget,    // 랜덤 위치 지정
    ArcZone,
    Sweep
}

public enum UnitType
{
    None,
    Player,
    Enemy
}