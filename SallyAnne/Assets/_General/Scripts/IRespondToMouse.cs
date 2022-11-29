/// <summary>
///     Use this on scripts that are dependent on Mouse input (MouseDown and MouseUp),
///     but that shouldn't be on a component which has a collider.
///     Put MouseHandler.cs on the component with the collider, it will find all instances
///     of scripts with this interface attached, and call the appropriate methods.
/// </summary>
public interface IRespondToMouse
{
    public void OnMouseButtonDown();


    public void OnMouseButtonUp();
}