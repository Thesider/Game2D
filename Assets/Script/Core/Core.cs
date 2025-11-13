using UnityEngine;

public class Core : MonoBehaviour {
    public Movement Movement { get => GenericNotImplementedError<Movement>.TryGet(movement, transform.parent.name);
                               private set => movement = value; }
    public CollisionSenes CollisionSenes { get => GenericNotImplementedError<CollisionSenes>.TryGet(collisionSenes, transform.parent.name);
                                           private set => collisionSenes= value; }
    public Combat Combat { get => GenericNotImplementedError<Combat>.TryGet(combat, transform.parent.name);
                            private set => combat = value; }

    private Movement movement;
    private CollisionSenes collisionSenes;
    private Combat combat;

    private void Awake() {
        Movement = GetComponentInChildren<Movement>();
        CollisionSenes = GetComponentInChildren<CollisionSenes>();
        Combat = GetComponentInChildren<Combat>();
    }

    public void LogicUpdate() {
        movement.LogicUpdate();
    }


}
